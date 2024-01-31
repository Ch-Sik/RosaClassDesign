using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : SerializedMonoBehaviour
{
    public static TilemapManager Instance;

    [SerializeField, ReadOnly]
    public List<Tilemap> defaultTilemaps;

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
        // 맵에 존재하는 모든 타일맵 가져오기 (active GameObject Only)
        List<Tilemap> tilemaps = FindObjectsOfType<Tilemap>().ToList();
        // "Ground","HiddenRoom","TmpTerrain" 레이어를 가진 것들만 필터링
        int layerMask = LayerMask.GetMask("Ground", "HiddenRoom", "TmpTerrain");
        foreach (var tilemap in tilemaps)
        {
            if((1 << tilemap.gameObject.layer & layerMask) == 0)
            {
                tilemaps.Remove(tilemap);
            }
        }
        // 기본 타일맵 설정
        defaultTilemaps = tilemaps;
    }

    public TileData GetTileDataByCellPosition(Vector3Int cellPosition)
    {
        Vector3 cellOffset = new Vector3(0.5f, 0.5f);

        // 우선 기본 타일맵에서 가져오기 시도 (최적화)
        TileBase selectedTile = null;
        if(defaultTilemaps != null)
        {
            for(int i=0; i<defaultTilemaps.Count; i++)
            {
                // 삭제된 타일맵이 있다면 리스트에서 제거
                if (defaultTilemaps[i] == null)
                {
                    defaultTilemaps.RemoveAt(i);
                    i--;
                    continue;
                }
                // 타일맵이 유효하다면 그 타일맵에서 타일 가져오기 시도
                selectedTile = defaultTilemaps[i].GetTile(cellPosition);
                if (selectedTile != null)
                    break;
            }
        }

        // 기본 타일맵에 없다면 Overlap 사용해서 해당 위치에 다른 타일맵이 있는지 검사
        if(selectedTile == null)
        {
            // 기본 타일맵에서 없다면... 가능성은
            // 1. 보스 등의 몬스터 패턴으로 인한 지형 소환
            Collider2D subTilemapCollider = Physics2D.OverlapPoint((Vector3)cellPosition + cellOffset, LayerMask.GetMask("Ground", "HiddenRoom", "TmpTerrain"));
            //if(subTilemapCollider == null)
            //{
            //    Debug.Log("no subtilemap");
            //}
            //else
            //    Debug.Log($"subTilemap: {subTilemapCollider.gameObject.name}");
            if (subTilemapCollider != null)
            {
                selectedTile = subTilemapCollider.gameObject.GetComponentInChildren<Tilemap>().GetTile(cellPosition);
            }
            else return null;
        }

        // 타일을 찾았다면 해당 타일의 데이터 가져오기
        try
        {
            return dataFromTiles[selectedTile];
        }
        catch(KeyNotFoundException)     // 혹시라도 타일데이터 설정을 빼먹은 경우
        {
            Debug.LogWarning($"타일 데이터가 설정되어있지 않음! 기본 설정을 사용합니다\n위치: {cellPosition}");
            return defaultTileData;
        }
    }

    public TileData GetTileDataByWorldPosition(Vector3 worldPosition)
    {
        return GetTileDataByCellPosition(WorldToCell(worldPosition));
    }

    public TileData GetTileDataByTileBase(TileBase tile)
    {
        TileData result;
        try
        {
            result = dataFromTiles[tile];
        }
        catch(KeyNotFoundException)
        {
            Debug.LogWarning($"타일 데이터가 설정되어있지 않음! 기본 설정을 사용합니다\n타일: {tile}");
            return defaultTileData;
        }
        return result;
    }

    public bool GetIfSubstanceTileExist(Vector3Int cellPosition)
    {
        Vector3 cellOffset = new Vector3(0.5f, 0.5f);

        TileBase selectedTile;
        for (int i = 0; i < defaultTilemaps.Count; i++)
        {
            // 삭제된 타일맵이 있다면 리스트에서 제거
            if (defaultTilemaps[i] == null)
            {
                defaultTilemaps.RemoveAt(i);
                i--;
                continue;
            }
            // 타일맵이 유효하다면 그 타일맵에서 타일 가져오기 시도
            selectedTile = defaultTilemaps[i].GetTile(cellPosition);
            if(selectedTile != null && dataFromTiles[selectedTile].isSubstance) 
                return true;
        }
        return false;

        //Collider2D hiddenRoomCurtain = Physics2D.OverlapPoint((Vector3)cellPosition + cellOffset, LayerMask.GetMask("HiddenRoom"));
        //if (hiddenRoomCurtain != null)
        //    return true;
        //else 
        //    return false;
    }

    public bool GetTilePlantable(Vector3Int cellPosition)
    {
        TileData td = GetTileDataByCellPosition(cellPosition);
        if (td == null)
        {
            Debug.LogError("해당 위치에 타일이 존재하지 않음");
        }
        return td.magicAllowed;
    }

    public Vector3Int WorldToCell(Vector2 worldPosition)
    {
        return defaultTilemaps[0].WorldToCell(worldPosition);
    }
}
