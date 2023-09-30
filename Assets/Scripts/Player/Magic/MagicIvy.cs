using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class MagicIvy : MonoBehaviour
{
    [SerializeField]
    private EdgeCollider2D edgeCol;
    [SerializeField]
    private SpriteRenderer sprite;
    [Tooltip("덩굴이 성장할 때의 틱 속도")]
    public float tickTime = 0.1f;
    [Tooltip("덩굴 성장 속도 (유닛/초)")]
    public float vineSpeed = 1f;
    [Tooltip("덩굴 스프라이트가 벽과 떨어진 정도 (만약 음수면 벽을 파고들어감)")]
    public float offset;

    private LR side;
    private float bottomY, topY;
    private float vinePerTick;
    private WaitForSeconds tick;

    public void Init(EdgeCollider2D wallEdge)
    {
        // 전달받은 EdgeCollider2D의 위쪽 점과 아래쪽 점 좌표를 가져옴
        float y1 = wallEdge.gameObject.transform.position.y;
        float y2 = y1;
        y1 += wallEdge.points[0].y;
        y2 += wallEdge.points[wallEdge.pointCount-1].y;
        bottomY = Mathf.Min(y1, y2);
        topY = Mathf.Max(y2, y1);

        // 왼쪽 벽인지 오른쪽 벽인지 파악
        if (wallEdge.gameObject.layer == LayerMask.NameToLayer("Wall_left"))
        {
            Debug.Log("오른쪽에서 왼쪽을 보는 벽");
            side = LR.LEFT;
            // 좌우 반전은 PlayerMagic::DoMagic에서 이미 되었으니, 위치 조절만 해주면 됨
            sprite.gameObject.transform.position -= new Vector3(offset, 0, 0);
        }
        else
        {
            Debug.Log("왼쪽에서 오른쪽을 보는 벽");
            side = LR.RIGHT;
            sprite.gameObject.transform.position += new Vector3(offset, 0, 0);
        }

        // 숫자 계산
        vinePerTick = tickTime * vineSpeed;

        // 최적화
        tick = new WaitForSeconds(tickTime);

        // 초기화가 끝나면 코루틴 실행
        StartCoroutine(GrowVineUpway());
        StartCoroutine(GrowVineDownway());
    }

    IEnumerator GrowVineUpway()
    {
        float cursor = 0f;
        Vector2[] points;
        while(gameObject.transform.position.y + cursor + vinePerTick < topY)
        {
            // 엣지콜라이더 업데이트
            cursor += vinePerTick;
            points = edgeCol.points;
            points[0] = new Vector2(0, cursor);     // 0번 포인트가 위쪽, 1번 포인트가 아래쪽
            edgeCol.points = points;

            // 스프라이트 업데이트 (콜라이더와 동기화)
            sprite.size = new Vector2(sprite.size.x, points[0].y - points[1].y);
            sprite.gameObject.transform.position =
                new Vector3(sprite.gameObject.transform.position.x,
                            transform.position.y + (points[0].y + points[1].y) / 2,
                            sprite.gameObject.transform.position.z);
            yield return tick;
        }
        points = edgeCol.points;
        points[0] = new Vector2(0, topY - gameObject.transform.position.y);
        edgeCol.points = points;

        sprite.size = new Vector2(sprite.size.x, points[0].y - points[1].y);
        sprite.gameObject.transform.position =
            new Vector3(sprite.gameObject.transform.position.x,
                        transform.position.y + (points[0].y + points[1].y) / 2,
                        sprite.gameObject.transform.position.z);
    }

    IEnumerator GrowVineDownway()
    {
        float cursor = 0f;
        Vector2[] points;
        while (gameObject.transform.position.y + cursor - vinePerTick > bottomY)
        {
            // 엣지콜라이더 업데이트
            cursor -= vinePerTick;
            points = edgeCol.points;
            points[1] = new Vector2(0, cursor);     // 0번 포인트가 위쪽, 1번 포인트가 아래쪽
            edgeCol.points = points;

            // 스프라이트 업데이트 (콜라이더와 동기화)
            sprite.size = new Vector2(sprite.size.x, points[0].y - points[1].y);
            sprite.gameObject.transform.position =
                new Vector3(sprite.gameObject.transform.position.x,
                            transform.position.y + (points[0].y + points[1].y) / 2,
                            sprite.gameObject.transform.position.z);
            yield return tick;
        }
        points = edgeCol.points;
        points[1] = new Vector2(0, bottomY - gameObject.transform.position.y);
        edgeCol.points = points;

        sprite.size = new Vector2(sprite.size.x, points[0].y - points[1].y);
        sprite.gameObject.transform.position =
            new Vector3(sprite.gameObject.transform.position.x,
                        transform.position.y + (points[0].y + points[1].y) / 2,
                        sprite.gameObject.transform.position.z);
    }
    
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
