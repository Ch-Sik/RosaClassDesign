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

    [Title("잠수 한 상태 관련 파라미터")]
    [SerializeField, Tooltip("좌우 이동 속도")]
    private float maxHorizontalSpeed;
    [SerializeField, Tooltip("좌우 이동을 허용하는 시간 길이")]
    private float horizontalMoveDuration;

    [Title("잠수 빠져나오기 관련 파라미터")]
    [SerializeField]
    private float diveOutDuration = 0.5f;

    float originHeight;
    Transform enemyTransform;

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
        Debug.Assert(diveInDuration < startupDuration, "diveInDuration은 startupDuration보다 짧아야 함!");

        // 필드 값 설정
        GameObject enemy;
        blackboard.TryGet(BBK.Enemy, out enemy);    // 필드 상에 적(PC)가 반드시 존재한다고 가정
        Debug.Assert(enemy != null, $"{GetType().Name} 보스가 적을 인식할 수 없음!");
        enemyTransform = enemy.transform;
        originHeight = transform.position.y;

        // 혹시나 좌우 관성이 남아있을 경우를 대비해 속도 0으로 설정
        rigidbody.velocity = Vector2.zero;

        // 잠수 모션 시작
        rigidbody.DOMove((Vector2)transform.position + Vector2.down * diveInDepth, diveInDuration)
            .SetEase(Ease.OutQuart);

        // 고민: 하강 모션 시작 즉시 공격 판정이 없어져야 할까?
    }

    protected override void OnStartupLast()
    {
        // 아직 물에 완전히 잠기기 전일 경우 즉시 리턴
        if (startupTimer.duration < diveInDuration)
            return;

        // 좌우 이동 허용 시간이 초과하였을 때에도 즉시 리턴
        if (startupTimer.duration > diveInDuration + horizontalMoveDuration)
            return;

        // 물에 완전히 잠겼을 경우 좌우 방향으로 플레이어 추격 시작
        Vector2 toEnemyDir = Vector2.right * (enemyTransform.position - transform.position);
        rigidbody.velocity = Vector2.ClampMagnitude(toEnemyDir, maxHorizontalSpeed);
    }

    protected override void OnActiveBegin()
    {
        Debug.Assert(diveOutDuration < activeDuration, "diveOutDuration은 activeDuration보다 짧아야 함!");

        // OnStartupLast에서 바꿔둔 velocity 초기화
        rigidbody.velocity = Vector2.zero;

        // 적 방향 바라보기
        LookAt2D(enemyTransform.position); // flip();

        // 튀어나오기
        rigidbody.DOMoveY(originHeight, diveOutDuration)
            .SetEase(Ease.OutQuart);
    }

}
