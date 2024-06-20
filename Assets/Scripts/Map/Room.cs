using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using Unity.VisualScripting;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Room : MonoBehaviour
{
    [Title("RoomDatas")]
    public SORoom roomData;
    public Tilemap tilemap;

    [Title("PreDatas"), FoldoutGroup("PreDatas")]
    public TileBase portTile;
    public GameObject triggerParent;
    public GameObject trigger;

    [Button]
    public void Init()
    {
        SetTriggers(roomData.topPorts);
        SetTriggers(roomData.botPorts);
        SetTriggers(roomData.rigPorts);
        SetTriggers(roomData.lefPorts);
    }

    public void SetTriggers(List<RoomPort> roomPorts)
    {
        foreach (var port in roomPorts)
            SetTrigger(port);
    }

    public void SetTrigger(RoomPort roomPort)
    {
        Vector2Int size = Vector2Int.one;
        Vector2 mid;
        if (roomPort.direction == PortDirection.Top ||
            roomPort.direction == PortDirection.Bot)
        {
            int min = roomPort.ports.Min(vec => vec.x);
            int max = roomPort.ports.Max(vec => vec.x);

            size = new Vector2Int(max - min + 1, 1);
            mid = new Vector2((max + min) / 2 + 0.5f + (roomPort.ports.Count % 2 == 0 ? 0.5f : 0.0f),
                               roomPort.ports[0].y + 0.5f);
        }
        else
        {
            int min = roomPort.ports.Min(vec => vec.y);
            int max = roomPort.ports.Max(vec => vec.y);
            size = new Vector2Int(1, max - min + 1);
            mid = new Vector2(roomPort.ports[0].x + 0.5f,
                              (max + min) / 2 + 0.5f + (roomPort.ports.Count % 2 == 0 ? 0.5f : 0.0f));
        }

        GameObject tri = Instantiate(trigger, mid, Quaternion.identity);
        tri.transform.SetParent(triggerParent.transform);
        tri.transform.localPosition = mid;
        //tri.transform.position = mid;
        tri.GetComponent<BoxCollider2D>().size = size;
        tri.GetComponent<RoomPortObject>().SetRoomToPort(this);
    }

    //방의 데이터를 모두 정리함.
    #region BakeRoom
    [Button]
    public void BakeRoom()
    {
        HashSet<Vector2Int> tilePosition = new HashSet<Vector2Int>(GetTilesPositionInTilemap(tilemap));
        Vector2Int size;
        int minX = int.MaxValue; int maxX = int.MinValue;
        int minY = int.MaxValue; int maxY = int.MinValue;

        HashSet<Vector2Int> tempPort = new HashSet<Vector2Int>();

        //타일 크기 측정 및 Port 추적
        foreach (Vector2Int tile in tilePosition)
        {
            if (minX > tile.x)
                minX = tile.x;
            if (maxX < tile.x)
                maxX = tile.x;
            if (minY > tile.y)
                minY = tile.y;
            if (maxY < tile.y)
                maxY = tile.y;

            if (tilemap.GetTile((Vector3Int)tile) == portTile)
                tempPort.Add(tile);
        }

        Debug.Log(tempPort.Count);

        roomData.tiles = new HashSet<Vector2Int>(tilePosition);
        List<RoomPort> top = new List<RoomPort>();
        List<RoomPort> bot = new List<RoomPort>();
        List<RoomPort> rig = new List<RoomPort>();
        List<RoomPort> lef = new List<RoomPort>();
        (top, bot, rig, lef) = GetPorts(tempPort, minX, maxX, minY, maxY);
        roomData.SetRoomPort(top, bot, rig, lef);
        roomData.offset = new Vector2Int(minX, minY);
        roomData.size = new Vector2Int(maxX - minX + 1, maxY - minY + 1);

    #if UNITY_EDITOR
        EditorUtility.SetDirty(roomData);
        AssetDatabase.SaveAssets();
    #endif
    }

    private List<Vector2Int> GetTilesPositionInTilemap(Tilemap tileMap)
    {
        List<Vector2Int> availablePlaces = new List<Vector2Int>();
        for (int n = tileMap.cellBounds.xMin; n < tileMap.cellBounds.xMax; n++)
        {
            for (int p = tileMap.cellBounds.yMin; p < tileMap.cellBounds.yMax; p++)
            {
                Vector3Int localPlace = (new Vector3Int(n, p, (int)tileMap.transform.position.y));
                if (tileMap.HasTile(localPlace))
                    availablePlaces.Add((Vector2Int)localPlace);
            }
        }

        return availablePlaces;
    }

    private (List<RoomPort>, List<RoomPort>, List<RoomPort>, List<RoomPort>) GetPorts(HashSet<Vector2Int> positions, int minX, int maxX, int minY, int maxY)
    {
        List<List<Vector2Int>> ports = new List<List<Vector2Int>>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        foreach (var point in positions)
        {
            if (!visited.Contains(point))
            {
                List<Vector2Int> port = new List<Vector2Int>();
                Queue<Vector2Int> queue = new Queue<Vector2Int>();

                queue.Enqueue(point);
                visited.Add(point);

                while (queue.Count > 0)
                {
                    Vector2Int current = queue.Dequeue();
                    port.Add(current);

                    foreach (var neighbor in GetNeighbors(current))
                    {
                        if (positions.Contains(neighbor) && !visited.Contains(neighbor))
                        {
                            queue.Enqueue(neighbor);
                            visited.Add(neighbor);
                        }
                    }
                }

                ports.Add(port);
            }
        }

        Debug.Log(ports.Count);
        List<RoomPort> topPort = new List<RoomPort>();
        List<RoomPort> botPort = new List<RoomPort>();
        List<RoomPort> rigPort = new List<RoomPort>();
        List<RoomPort> lefPort = new List<RoomPort>();
        int index = 0;
        foreach (var port in ports)
        {
            if (port[0].x == minX)
                lefPort.Add(new RoomPort(PortDirection.Lef, port, index));
            else if (port[0].x == maxX)
                rigPort.Add(new RoomPort(PortDirection.Rig, port, index));
            else if (port[0].y == minY)
                botPort.Add(new RoomPort(PortDirection.Bot, port, index));
            else if (port[0].y == maxY)
                topPort.Add(new RoomPort(PortDirection.Top, port, index));

            index++;
        }

        return (topPort, botPort, rigPort, lefPort);
    }

    List<Vector2Int> GetNeighbors(Vector2Int point)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>
        {
            new Vector2Int(point.x + 1, point.y),
            new Vector2Int(point.x - 1, point.y),
            new Vector2Int(point.x, point.y + 1),
            new Vector2Int(point.x, point.y - 1)
        };

        return neighbors;
    }
    #endregion

    #region Event
    public void Enter()
    {
        Debug.Log($"[{roomData.title}] 플레이어 입장");
    }

    public void Exit()
    {
        Debug.Log($"[{roomData.title}] 플레이어 퇴장");
    }
    #endregion
}