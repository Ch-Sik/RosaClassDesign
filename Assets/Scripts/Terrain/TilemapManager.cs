using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : SerializedMonoBehaviour
{
    public static TilemapManager Instance;

    [SerializeField]
    public Tilemap map;

    public List<TileData> tileDatas;
    [SerializeField]
    private Dictionary<TileBase, TileData> dataFromTiles;

    [SerializeField]
    private TileData defaultTileData;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;

        // 딕셔너리가 컴파일타임에 값 할당이 안되서 우선 리스트에 데이터를 담아두고 런타임에 이를 옮겨야 함.
        dataFromTiles = new Dictionary<TileBase, TileData>();       
        foreach(var tileData in tileDatas)
        {
            foreach(var tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData);
            }
        }
    }

    private void Start()
    {

    }

    public TileData GetTileDataByCellPosition(Vector3Int tilePosition)
    {
        TileBase selectedTile = map.GetTile(tilePosition);
        if(selectedTile == null)
        {
            Debug.LogError($"그 위치에는 타일이 존재하지 않습니다\n위치: {tilePosition}");
            return null;
        }
        try
        {
            return dataFromTiles[selectedTile];
        }
        catch(KeyNotFoundException)     // 혹시라도 타일데이터 설정을 빼먹은 경우
        {
            // Debug.LogWarning($"타일 데이터가 설정되어있지 않음! 기본 설정을 사용합니다\n위치: {tilePosition}");
            return defaultTileData;
        }
    }

    public TileData GetTileDataByWorldPosition(Vector3 worldPosition)
    {
        TileData result =  GetTileDataByCellPosition(map.WorldToCell(worldPosition));
        if(result == null)
        {
            Debug.LogError($"그 위치: {worldPosition}");
        }
        else if(!result.magicAllowed)
        {
            Debug.Log("그곳은 식물마법을 설치 불가능한 지형");
        }
        return result;
    }
}
