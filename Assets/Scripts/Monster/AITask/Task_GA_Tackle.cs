using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

[RequireComponent(typeof(MonsterDamageInflictor))]
public class Task_GA_Tackle : Task_A_Base
{
    // 컴포넌트 레퍼런스
    [SerializeField]
    protected new Rigidbody2D rigidbody;
    [SerializeField]
    protected MonsterDamageInflictor damageComponent;

    [Header("공격 관련")]
    [SerializeField, Tooltip("돌진 패턴 공격력")]
    protected int tackleAttackPower;
    [SerializeField, Tooltip("돌진 속도")]
    protected float tackleSpeed = 5;

    [Header("벽에 박았을 때 관련")]
    [SerializeField, Tooltip("돌진 중 벽에 박았을 때 스턴 활성화")]
    protected bool wallStunEnabled;
    [SerializeField, Tooltip("돌진 중 벽에 박았을 떄 스턴 시간")]
    protected float wallStunDuration;

    protected Timer stunTimer = null;
    protected Vector2 tackleDir;
    protected int defaultCollideDamage;    // 몸체 충돌 판정이 기본적으로 가지고 있던 데미지


    private void Start()
    {
        if (blackboard == null)
        {
            blackboard = GetComponent<Blackboard>();
            if (blackboard == null)
                Debug.LogError($"{gameObject.name}: Blackboard를 찾을 수 없음!");
        }
        if (rigidbody == null)
        {
            rigidbody = GetComponent<Rigidbody2D>();
            if (rigidbody == null)
                Debug.LogError($"{gameObject.name}: Rigidbody2D를 찾을 수 없음!");
        }
        if(damageComponent == null)
        {
            damageComponent = GetComponent<MonsterDamageInflictor>();
            if(damageComponent == null)
                Debug.LogError($"{gameObject.name}: damageComponent를 찾을 수 없음!");
        }
        defaultCollideDamage = damageComponent.damage;
    }

    [Task]
    protected void TackleAttack()
    {
        ExecuteAttack();
    }

    // 공격 패턴을 구체적으로 지정하지 않고 대충 Attack()으로 뭉뚱그려 작성된 BT 스크립트 호환용
    [Task]
    protected void Attack()
    {
        ExecuteAttack();
    }

    protected override void OnStartupBegin()
    {
        // 방향 계산
        CalculateAttackDirection();
    }

    protected override void OnActiveBegin()
    {
        // 기존 공격력은 저장해두고 몸통 접촉 시의 데미지를 tackleAttackPower로 대체
        defaultCollideDamage = damageComponent.damage;
        damageComponent.damage = tackleAttackPower;
    }

    protected override void OnActiveLast()
    {
        // 블랙보드에서 벽에 박음 정보 가져오기
        bool isStuckAtWall;
        blackboard.TryGet(BBK.StuckAtWall, out isStuckAtWall);

        // 벽에 박았을 경우 스턴
        if (wallStunEnabled && isStuckAtWall)
        {
            // 스턴 첫 프레임
            if (stunTimer == null)
            {
                stunTimer = Timer.StartTimer();
                blackboard.Set(BBK.isStunned, true);    // 애니메이션을 위한 블랙보드 설정
                Debug.Log("벽에다 대가리 꽁!!!");
            }
            // 스턴 중간 프레임
            else if (stunTimer.duration < wallStunDuration)
            {
                ThisTask.debugInfo = $"stun: {wallStunDuration - stunTimer.duration}";
                return;
            }
            // 스턴 마지막 프레임
            else
            {
                stunTimer = null;
                blackboard.Set(BBK.isStunned, false);
                // Debug.Log("스턴 끝");
                Succeed();
                return;
            }
        }

        // 실제 돌진 수행
        DoTackle();
    }

    protected override void OnRecoveryBegin()
    {
        // 기존 공격력으로 복구
        damageComponent.damage = defaultCollideDamage;
    }

    protected virtual void CalculateAttackDirection()
    {
        GameObject enemy;
        if (!blackboard.TryGet(BBK.Enemy, out enemy) || enemy == null)
        {
            Debug.LogError($"{gameObject.name}: PrepareAttack에서 적을 찾을 수 없음!");
        }
        tackleDir = (enemy.transform.position.x - transform.position.x) < 0 ? Vector2.left : Vector2.right;

        // 공격 방향에 따라 좌우 반전하기
        LookAt2D(enemy.transform.position);
    }

    protected virtual void DoTackle()
    {
        Vector2 velocity = tackleDir * tackleSpeed;
        velocity.y = rigidbody.velocity.y;
        rigidbody.velocity = velocity;
    }


    private bool isTargetBehind(GameObject enemy)
    {
        Vector2 toEnemy = (enemy.transform.position - transform.position).normalized;
        float cosDist = Vector2.Dot(toEnemy, tackleDir);
        return cosDist < -0.1f;     // 아주 약간 정도는 뒤로 가도 봐줌.
    }
}
