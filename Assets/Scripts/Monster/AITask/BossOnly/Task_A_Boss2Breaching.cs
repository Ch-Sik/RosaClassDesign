using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using DG.Tweening;
using Sirenix.OdinInspector;

public class Task_A_Boss2Breaching : Task_A_Base
{
    [Title("컴포넌트 레퍼런스")]
    protected new Rigidbody2D rigidbody;

    [Title("잠수 동작 관련 파라미터")]
    [SerializeField, Tooltip("얼마나 깊숙히 잠수해야 하는지")]
    private float diveInDepth;
    [SerializeField, Tooltip("잠수 들어갈 때 걸리는 시간")]
    private float diveInDuration = 0.5f;

    [Title("튀어나오기 동작 관련 파라미터")]
    [SerializeField, Tooltip("튀어나오는 높이와 전진하는 거리 (패턴 시작하기 전 위치 기준)")]
    private Vector2 breachMovement;

    [Title("복귀 동작 관련 파라미터")]
    [SerializeField, Tooltip("튀어오른 최고 높이에서 다시 내려오는 시간")]
    private float fallingDuration = 0.5f;
    [SerializeField, Tooltip("떨어지는 힘으로 인해 내려갔다 올라오는 반동 거리")]
    private float fallingDiveDepth = 0.5f;
    [SerializeField, Tooltip("떨어지는 힘으로 인해 내려갔다 '올라오는' 시간")]
    private float fallingDiveDuration = 0.5f;

    Vector2 originPosition; // 패턴 시작하기 전의 위치 저장용

    private void Start()
    {
        if (rigidbody == null)
        {
            rigidbody = GetComponent<Rigidbody2D>();
            Debug.Assert(rigidbody != null);
        }
        if (blackboard == null)
        {
            blackboard = GetComponent<Blackboard>();
            Debug.Assert(blackboard != null);
        }
    }

    [Task]
    public void Breaching()
    {
        ExecuteAttack();
    }

    // 물 아래로 잠수
    protected override void OnStartupBegin()
    {
        originPosition = transform.position;

        // 혹시나 좌우 관성이 남아있을 경우를 대비해 속도 0으로 설정
        rigidbody.velocity = Vector2.zero;

        Debug.Assert(diveInDuration < startupDuration, "diveInDuration은 startupDuration보다 짧아야 함!");
        rigidbody.DOMove((Vector2)transform.position + Vector2.down * diveInDepth, diveInDuration)
            .SetEase(Ease.OutQuart);
        // 다이브할 때 공격 판정이 없어져야 할까?
    }

    // 물 위로 튀어나오기
    protected override void OnActiveBegin()
    {
        // 플레이어 방향 바라보기
        GameObject enemy;
        blackboard.TryGet(BBK.Enemy, out enemy);
        Debug.Assert(enemy != null, "Dive 패턴: 적을 찾을 수 없음");
        LookAt2D(enemy.transform.position); // flip();

        // 움직임 계산
        Vector2 targetPosition = Vector2.Scale(breachMovement, GetCurrentDir().toVector2() + Vector2.up);

        // 실제 움직이기
        DOTween.Sequence()
            .Append(
                rigidbody.DOMove(originPosition + targetPosition, activeDuration)
                    .SetEase(Ease.OutQuart)
            );
    }

    // 다시 수면으로 되돌아가기
    protected override void OnRecoveryBegin()
    {
        Vector2 diveInPosition;
        Vector2 finalPosition;

        // 움직임 계산
        finalPosition = originPosition + GetCurrentDir().toVector2() * breachMovement.x;
        diveInPosition = finalPosition + Vector2.down * fallingDiveDepth;

        // 실제 움직이기
        DOTween.Sequence()
            .Append(
                rigidbody.DOMove(diveInPosition, fallingDuration)
                    .SetEase(Ease.InOutCubic)
            ).Append(
                // 낙하 속도로 인해 수면에서 바로 멈추지 않고 약간의 반동 움직임.
                rigidbody.DOMove(finalPosition, fallingDiveDuration)
                    .SetEase(Ease.InCubic)
            );
    }
}
