using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using Sirenix.OdinInspector;
using DG.Tweening;

// Task_GA_Tackle(지상 몬스터 태클 공격 스크립트)에서 공격 방향 계산하는 거랑 공격 시의 velocity 설정하는 거만 override함.
public class Task_FA_Tackle : Task_A_Base
{
    [Title("컴포넌트 레퍼런스")]
    [SerializeField]
    protected new Rigidbody2D rigidbody;
    [SerializeField]
    protected MonsterDamageInflictor damageComponent;

    [Header("공격 관련")]
    [SerializeField, Tooltip("돌진 패턴 공격력")]
    protected int tackleAttackPower;
    [SerializeField, Tooltip("돌진 속도 (m/s)")]
    protected float tackleSpeed = 5;
    [SerializeField, Tooltip("돌진 가속도 (m/s^2)")]
    protected float tackleAccel = 1000;
    [SerializeField, Tooltip("돌진 후 브레이크 계수")]
    protected float recoveryDrag = 3.0f;

    [Title("지형 감지 관련")]
    [SerializeField]
    protected GameObject terrainSensor;
    [SerializeField]
    protected float terrainSensorOffset;

    [Title("벽 충돌 후의 행동 관련")]
    [SerializeField, Tooltip("지형에 들이박은 후 물러나는 정도")]
    protected float reboundPower;
    [SerializeField, Tooltip("지형에 들이박은 후 행동 불능 시간")]
    protected float reboundStunTime;

    protected bool isRebounded;
    protected Timer reboundTimer = null;
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
        if (damageComponent == null)
        {
            damageComponent = GetComponent<MonsterDamageInflictor>();
            if (damageComponent == null)
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
        // 플래그 설정
        isRebounded = false;
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

        // 벽에 박았을 경우
        if(isStuckAtWall || isRebounded)
        {
            isRebounded = true;  // 넉백으로 인해 다시 벽에 박지 않았을 상태의 루틴 실행 방지
            HandleRebound();
        }
        else if(!isRebounded)
        {
            // 벽에 박지 않았을 경우 실제 돌진 수행
            DoTackle();
        }
    }

    protected void HandleRebound()
    {
        if(reboundTimer == null)
        {
            reboundTimer = Timer.StartTimer();
            blackboard.Set(BBK.isStunned, true);
            Debug.Log($"{gameObject.name}: 벽에 충돌");
            activeTimer.Reset();    // Task_A_Base에 의해 스턴 도중에 패턴 종료되는 것 방지

            // 속도 초기화 & 뒤로 밀려나기
            rigidbody.velocity = Vector2.zero;
            rigidbody.AddForce(-tackleDir * reboundPower);

            return;
        }
        else if(reboundTimer.duration < reboundStunTime)
        {
            ThisTask.debugInfo = $"stun: {reboundStunTime - reboundTimer.duration}";
            activeTimer.Reset();    // Task_A_Base에 의해 스턴 도중에 패턴 종료되는 것 방지
            return;
        }
        else
        {
            reboundTimer = null;
            blackboard.Set(BBK.isStunned, false);
            Debug.Log("스턴 끝");
            Fail();
            return;
        }
    }

    protected void DoTackle()
    {
        rigidbody.velocity = tackleDir * tackleSpeed;
    }

    protected override void OnRecoveryBegin()
    {
        // 기존 공격력으로 복구
        damageComponent.damage = defaultCollideDamage;
    }

    protected void CalculateAttackDirection(bool showErrorMsg = true)
    {
        GameObject enemy;

        // 디버그
        Debug.Log($"계산 전 스케일: {transform.localScale}\n" +
            $"terrainSensor: {terrainSensor.transform.localPosition}");

        if (!blackboard.TryGet(BBK.Enemy, out enemy) || enemy == null)
        {
            if(showErrorMsg)
                Debug.LogError($"{gameObject.name}: 블랙보드에서 적을 찾을 수 없음!");
        }
        tackleDir = (enemy.transform.position - transform.position).normalized;

        // 공격 방향에 따라 좌우 반전하기
        LookAt2D(enemy.transform.position);
        // 지형 감지 센서 위치 설정
        terrainSensor.transform.localPosition = Vector2.right * terrainSensorOffset;

        // 기울기 계산
        Vector2 vectorToEnemy = enemy.transform.position - transform.position;
        // 좌우 반전 고려 1
        if (transform.localScale.x < 0)
            vectorToEnemy.x = -vectorToEnemy.x;
        float radianToEnemy = Mathf.Atan2(vectorToEnemy.y, vectorToEnemy.x);
        // 좌우 반전 고려 2
        if (transform.localScale.x < 0)
            radianToEnemy = -radianToEnemy;
        transform.DORotate(new Vector3(0, 0, radianToEnemy * Mathf.Rad2Deg), 0.2f);
        Debug.Log($"Shoebill Angle: {radianToEnemy}");

        // 디버그
        Debug.Log($"계산 후 스케일: {transform.localScale}\n" +
            $"terrainSensor: {terrainSensor.transform.localPosition}");
    }

    protected override void Succeed()
    {
        base.Succeed();

        transform.rotation = Quaternion.identity;
        reboundTimer = null;
        isRebounded = false;
        Debug.Log("Reset Shoebill Angle");
    }

    protected override void Fail()
    {
        base.Fail();

        transform.rotation = Quaternion.identity;
        reboundTimer = null;
        isRebounded = false;

        Debug.Log("Reset Shoebill Angle");
    }
}
