using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using Sirenix.OdinInspector;
using DG.Tweening;

/// <summary>
/// Startup: 아래로 내려감 & 남은 Startup 시간동안 대기
/// Active: 위로 올라옴
/// Recovery: 투사체들 떨어질 때까지 대기
/// </summary>
public class Task_A_Dive : Task_A_Base
{
    [Title("컴포넌트 레퍼런스")]
    protected new Rigidbody2D rigidbody;

    [Title("잠수 들어가기 관련 파라미터")]
    [SerializeField, Tooltip("얼마나 깊숙히 잠수해야 하는지")]
    private float diveInDepth;
    [SerializeField, Tooltip("잠수 들어갈 때 걸리는 시간")]
    private float diveInDuration = 0.5f;

    [Title("잠수 빠져나오기 관련 파라미터")]
    [SerializeField]
    private float diveOutDuration = 0.5f;

    Sequence diveSequence;
    Vector2 originPosition;

    private void Start()
    {
        if(rigidbody == null)
        {
            rigidbody = GetComponent<Rigidbody2D>();
            Debug.Assert(rigidbody != null);
        }
        if(blackboard == null)
        {
            blackboard = GetComponent<Blackboard>();
            Debug.Assert(blackboard != null);
        }
    }

    [Task]
    private void Dive()
    {
        ExecuteAttack();
    }

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

    protected override void OnActiveBegin()
    {
        Debug.Assert(diveOutDuration < activeDuration, "diveOutDuration은 activeDuration보다 짧아야 함!");

        GameObject enemy;
        blackboard.TryGet(BBK.Enemy, out enemy);
        Debug.Assert(enemy != null, "Dive 패턴: 적을 찾을 수 없음");

        LookAt2D(enemy.transform.position); // flip();
        rigidbody.DOMove(originPosition, diveOutDuration)
            .SetEase(Ease.OutQuart);
    }

}
