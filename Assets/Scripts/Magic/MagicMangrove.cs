using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class MagicMangrove : MonoBehaviour
{
    [SerializeField]
    private EdgeCollider2D edgeCol;
    [SerializeField]
    private SpriteRenderer spriteLeft;
    [SerializeField]
    private SpriteRenderer spriteRight;

    [SerializeField, Tooltip("좌우 크기에 제한 둘 것인지 말건지")]
    private bool hasSizeLimit;
    [SerializeField, DrawIf("hasSizeLimit", true)]
    [Tooltip("좌우 크기가 제한이 있을 경우의 그 제한 크기 (크기는 오른쪽/왼쪽 절반 따로 계산해서 제한함)")]
    private float sizeLimit = 3f;
    [SerializeField, Tooltip("뿌리가 성장할 때의 틱 속도")]
    private float tickTime = 0.1f;
    [SerializeField, Tooltip("뿌리가 좌우로 성장하는 속도 (유닛/초)")]
    private float growSpeed = 1f;
    [SerializeField, Tooltip("스프라이트가 벽과 떨어진 정도 (만약 음수면 지형을 파고들어감)")]
    private float offset;

    private float leftendX, rightendX;
    private float rootPerTick;
    private WaitForSeconds tick;
    

    public void Init(EdgeCollider2D ceilEdge)
    {
        // 전달받은 EdgeCollider2D의 왼쪽 점과 오른쪽 점 좌표를 가져옴
        float x1 = ceilEdge.gameObject.transform.position.x + ceilEdge.offset.x;
        float x2 = x1;
        x1 += ceilEdge.points[0].x * ceilEdge.transform.lossyScale.x;
        x2 += ceilEdge.points[ceilEdge.pointCount - 1].x * ceilEdge.transform.lossyScale.x;
        leftendX = Mathf.Min(x1, x2);
        rightendX = Mathf.Max(x1, x2);

        // 숫자 계산
        rootPerTick = tickTime * growSpeed;

        // 최적화
        tick = new WaitForSeconds(tickTime);

        // 초기화가 끝나면 코루틴 실행
        StartCoroutine(GrowRootLeft());
        StartCoroutine(GrowRootRight());
    }

    IEnumerator GrowRootLeft()
    {
        float cursor = 0f;
        float leftLimit;
        Vector2[] points;

        // 왼쪽 한계점 계산
        if (hasSizeLimit)
            leftLimit = Mathf.Max(leftendX, transform.position.x - sizeLimit);
        else
            leftLimit = leftendX;

        // 왼쪽 한계점에 도달하기 직전까지 rootPerTick만큼씩 성장
        while (transform.position.x + cursor - rootPerTick > leftLimit)
        {
            // 엣지콜라이더 업데이트
            cursor -= rootPerTick;
            points = edgeCol.points;
            points[0] = new Vector2(cursor, 0);     // 0번 포인트가 왼쪽, 1번 포인트가 오른쪽
            edgeCol.points = points;
            // 스프라이트 업데이트 (콜라이더와 동기화)
            // 스프라이트가 flip되어있으므로 왼쪽으로 자라더라도 size.x는 양수여야 함
            spriteLeft.size = new Vector2(-cursor, spriteLeft.size.y);

            yield return tick;
        }
        // 마지막으로 왼쪽 한계점까지 성장
        points = edgeCol.points;
        points[0] = new Vector2(leftLimit - transform.position.x, 0);
        edgeCol.points = points;

        spriteLeft.size = new Vector2(-points[0].x, spriteLeft.size.y);
    }

    IEnumerator GrowRootRight()
    {
        float cursor = 0f;
        float rightLimit;
        Vector2[] points;

        // 오른쪽 한계점 계산
        if (hasSizeLimit)
            rightLimit = Mathf.Min(rightendX, transform.position.x + sizeLimit);
        else
            rightLimit = rightendX;

        // 오른쪽 한계점에 도달하기 직전까지 rootPerTick만큼씩 성장
        while (transform.position.x + cursor + rootPerTick < rightLimit)
        {
            // 엣지콜라이더 업데이트
            cursor += rootPerTick;
            points = edgeCol.points;
            points[1] = new Vector2(cursor, 0);     // 0번 포인트가 왼쪽, 1번 포인트가 오른쪽
            edgeCol.points = points;
            // 스프라이트 업데이트 (콜라이더와 동기화)
            spriteRight.size = new Vector2(cursor, spriteLeft.size.y);

            yield return tick;
        }
        // 마지막으로 오른쪽 한계점까지 성장
        points = edgeCol.points;
        points[1] = new Vector2(rightLimit - transform.position.x, 0);
        edgeCol.points = points;

        spriteRight.size = new Vector2(points[1].x, spriteLeft.size.y);
    }
}
