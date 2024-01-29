using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MagicIvy : MonoBehaviour
{
    [SerializeField]
    private EdgeCollider2D edgeCol;     // 0번 점이 위, 1번 점이 아래
    [SerializeField]
    private TilemapManager tilemapManager;
    [SerializeField]
    private Vector3Int originCellPos;   // 담쟁이가 설치된 타일 위치

    [SerializeField]
    private SpriteRenderer spriteUp;
    [SerializeField]
    private SpriteRenderer spriteDown;

    [SerializeField, Tooltip("덩굴이 성장할 때의 틱 속도")]
    private float tickTime = 0.1f;
    [SerializeField, Tooltip("덩굴 스프라이트가 벽과 떨어진 정도 (만약 음수면 벽을 파고들어감)")]
    private float offset;

    private LR wallDirection;   // 현재 담쟁이가 자라는 벽이 바라보는 방향

    private WaitForSeconds tick;

    public void Init(Vector2 magicPos)
    {
        // 필드 값 세팅
        if(edgeCol == null) edgeCol = GetComponent<EdgeCollider2D>();
        tilemapManager = TilemapManager.Instance;

        // 오른쪽 벽에 부딪혔는지 왼쪽 벽에 부딪혔는지 판단
        // 0.5의 오프셋을 주어서 오른쪽 셀 검사
        originCellPos = tilemapManager.WorldToCell(magicPos + new Vector2(0.5f, 0));   
        if(TilemapManager.Instance.GetIfSubstanceTileExist(originCellPos))
        {
            wallDirection = LR.LEFT;
            Debug.Log("왼쪽 보는 벽에 담쟁이 설치");
        }
        else
        {
            wallDirection = LR.RIGHT;
            Debug.Log("오른쪽 보는 벽에 담쟁이 설치");
        }

        // y축 방향 위치 보정
        magicPos.y = Mathf.Round(magicPos.y - 0.5f) + 0.5f;
        transform.position = magicPos;
        originCellPos = tilemapManager.WorldToCell(magicPos + new Vector2(0.5f, 0));

        // 최적화
        tick = new WaitForSeconds(tickTime);

        // 초기화가 끝나면 코루틴 실행
        StartCoroutine(GrowVineUpway());
        StartCoroutine(GrowVineDownway());
    }

    IEnumerator GrowVineUpway()
    {
        int cursor = 0;
        Vector2[] points;

        while(true)
        {
            // 한칸씩 위쪽으로 가면서 타일 검사
            cursor++;

            TileData innerTile = tilemapManager.GetTileDataByCellPosition(originCellPos
                    + new Vector3Int(wallDirection.isRIGHT() ? -1 : 0, cursor, 0)
                    );
            TileData outerTile = tilemapManager.GetTileDataByCellPosition(originCellPos
                    + new Vector3Int(wallDirection.isRIGHT() ? 0 : -1, cursor, 0)
                    );

            bool isInnerTileMagicAllowed = innerTile != null && innerTile.magicAllowed;
            bool isOuterTileNonSubstance = !outerTile || !outerTile.isSubstance;
            if (isInnerTileMagicAllowed && isOuterTileNonSubstance)
            {
                points = edgeCol.points;
                points[0] = new Vector2(-offset, cursor);     // 0번 포인트가 위쪽, 1번 포인트가 아래쪽
                edgeCol.points = points;
                // 스프라이트 업데이트 (콜라이더와 동기화)
                spriteUp.size = new Vector2(spriteUp.size.x, cursor);
            }
            else
            {
                cursor--;
                break;
            }
            yield return tick;
        }

        // 마지막으로 끝까지 성장
        points = edgeCol.points;
        points[0] = new Vector2(-offset, cursor + 0.5f);
        edgeCol.points = points;
        // 스프라이트 업데이트 (콜라이더와 동기화)
        spriteUp.size = new Vector2(spriteUp.size.x, cursor + 0.5f);
    }

    IEnumerator GrowVineDownway()
    {
        int cursor = 0;
        Vector2[] points;

        // 한칸씩 위쪽으로 가면서 프로브
        while (true)
        {
            // 한칸씩 아래쪽으로 가면서 타일 검사
            cursor--;

            TileData innerTile = tilemapManager.GetTileDataByCellPosition(originCellPos
                    + new Vector3Int(wallDirection.isRIGHT() ? -1 : 0, cursor, 0)
                    );
            TileData outerTile = tilemapManager.GetTileDataByCellPosition(originCellPos
                    + new Vector3Int(wallDirection.isRIGHT() ? 0 : -1, cursor, 0)
                    );

            bool isInnerTileMagicAllowed = innerTile != null && innerTile.magicAllowed;
            bool isOuterTileNonSubstance = !outerTile || !outerTile.isSubstance;
            if (isInnerTileMagicAllowed && isOuterTileNonSubstance)
            {
                points = edgeCol.points;
                points[1] = new Vector2(-offset, cursor);     // 0번 포인트가 위쪽, 1번 포인트가 아래쪽
                edgeCol.points = points;
                // 스프라이트 업데이트 (콜라이더와 동기화)
                spriteDown.size = new Vector2(spriteDown.size.x, -cursor);
            }
            else
            {
                cursor++;
                break;
            }
            yield return tick;
        }

        // 마지막으로 끝까지 성장
        points = edgeCol.points;
        points[1] = new Vector2(-offset, cursor - 0.5f);
        edgeCol.points = points;
        // 스프라이트 업데이트 (콜라이더와 동기화)
        spriteDown.size = new Vector2(spriteDown.size.x, -(cursor - 0.5f));
    }
    
    // TODO: Destroy/OnDestroy 대신 별도의 함수를 사용하여 '사라지는 연출' 구현
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
