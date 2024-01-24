using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

public class AITask_RangeAttack_SimpleProjectile : AITask_Base
{
    [SerializeField]
    protected Blackboard blackboard;

    [Header("공격 관련")]
    [SerializeField, Tooltip("투사체 프리팹")]
    private GameObject projectilePrefab;
    [SerializeField, Tooltip("투사체가 발사되어야 할 위치")]
    private Transform muzzle;
    [SerializeField, Tooltip("공격 선딜레이")]
    private float startupDuration;
    [SerializeField, Tooltip("공격 후딜레이")]
    private float recoveryDuration;
    [SerializeField, Tooltip("공격 방향 옵션")]
    private bool forceDirection90Deg = false;

    private Timer startupTimer = null;
    private Timer recoveryTimer = null;
    private Vector2 attackDir;

    private void Start()
    {
        if(blackboard == null)
        {
            blackboard = GetComponent<Blackboard>();
            Debug.Assert(blackboard != null, $"{gameObject.name}: Blackboard를 찾을 수 없음");
        }
        Debug.Assert(projectilePrefab != null, $"{gameObject.name}: 투사체 프리팹이 설정되어있지 않음");
    }

    [Task]
    private void Attack()
    {
        SimpleProjectileAttack();
    }

    [Task]
    private void SimpleProjectileAttack()
    {
        // 블랙보드에서 피격 정보 가져오기
        bool isHitt;
        blackboard.TryGet(BBK.isHitt, out isHitt);
        // 피격 시 행동 중지
        if (isHitt)
        {
            Fail();
            return;
        }

        if ( !startupTimer && !recoveryTimer ) // 공격의 첫 프레임
        {
            startupTimer = Timer.StartTimer();
        }
        else if (startupTimer && startupTimer.duration < startupDuration)    // 선딜레이 중
        {
            ThisTask.debugInfo = $"선딜레이: {startupTimer.duration}";
        }
        else if (!recoveryTimer)     // 선딜은 끝났지만 공격을 아직 수행하지 않은 경우
        {
            // 공격 시전
            DoAttack();
            startupTimer = null;
            recoveryTimer = Timer.StartTimer();
        }
        else if (recoveryTimer.duration < recoveryDuration)  // 후딜 진행중인 경우
        {
            ThisTask.debugInfo = $"후딜레이: {recoveryTimer.duration}";
        }
        else        // 후딜 종료
        {
            Succeed();
        }
    }

    

    private void DoAttack()
    {
        // 적(플레이어) 위치 파악
        Vector2 dir;
        GameObject enemy;
        if (!blackboard.TryGet(BBK.Enemy, out enemy) || enemy == null)
        {
            Debug.LogWarning($"{gameObject.name}: Attack에서 적을 찾을 수 없음!");
            ThisTask.Fail();
            return;
        }
        dir = enemy.transform.position - gameObject.transform.position;

        // 옵션에 따라 방향 벡터 조정
        if (forceDirection90Deg)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Debug.Log($"보정 전 각도: {angle}");
            angle = Mathf.FloorToInt(angle + 45) / 4 * 90 - 45;
            Debug.Log($"보정 후 각도: {angle}");

            dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }
        attackDir = dir;

        // 공격 시전
        GameObject projectile = Instantiate(projectilePrefab, muzzle.position, Quaternion.identity);
        projectile.GetComponent<MonsterProjectile>().InitProjectile(attackDir.normalized, enemy);
    }

    private void Succeed()
    {
        ThisTask.Succeed();
        startupTimer = null;
        recoveryTimer = null;
    }

    private void Fail()
    {
        ThisTask.Fail();
        startupTimer = null;
        recoveryTimer = null;
    }
}
