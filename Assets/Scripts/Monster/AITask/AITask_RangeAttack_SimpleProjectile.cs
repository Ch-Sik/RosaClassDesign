using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

public class AITask_RangeAttack_SimpleProjectile : MonoBehaviour
{
    [SerializeField]
    protected Blackboard blackboard;

    [Header("공격 관련")]
    [SerializeField, Tooltip("투사체 프리팹")]
    private GameObject projectilePrefab;
    [SerializeField, Tooltip("투사체가 발사되어야 할 위치")]
    private Transform muzzle;
    [SerializeField, Tooltip("공격 선딜레이")]
    private float prepareDuration;
    [SerializeField, Tooltip("공격 후딜레이")]
    private float recoveryDuration;
    [SerializeField, Tooltip("공격 방향 옵션")]
    private bool forceDirection90Deg = false;

    private Timer prepareTimer = null;
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
        // 블랙보드에서 피격 정보 가져오기
        bool isHitt;
        blackboard.TryGet(BBK.isHitt, out isHitt);
        if (prepareTimer == null || prepareTimer.duration < prepareDuration)
        {
            // 피격 시 행동 중지
            if (isHitt)
            {
                Fail();
                return;
            }
            if(prepareTimer == null)
                prepareTimer = Timer.StartTimer();
            else
                ThisTask.debugInfo = $"선딜레이: {prepareTimer.duration}";
        }
        else if (!recoveryTimer)     // 선딜은 끝났지만 공격을 아직 수행하지 않은 경우
        {
            // 피격 시 행동 중지
            if (isHitt)
            {
                Fail();
                return;
            }
            // 공격 시전
            {
                DoAttack();
            }
            recoveryTimer = Timer.StartTimer();
            prepareTimer = null;
        }
        else if (recoveryTimer.duration < recoveryDuration)  // 후딜 진행중인 경우
        {
            // 피격 시 행동 중지
            if (isHitt)
            {
                Fail();
                return;
            }
            ThisTask.debugInfo = $"후딜레이: {recoveryTimer.duration}";
        }
        else        // 후딜 종료
        {
            ThisTask.Succeed();
            recoveryTimer = null;
        }
    }

    private void DoAttack()
    {
        // 적(플레이어) 위치 파악
        Vector2 dir;
        GameObject enemy;
        if (!blackboard.TryGet(BBK.Enemy, out enemy) || enemy == null)
        {
            Debug.LogError($"{gameObject.name}: Attack에서 적을 찾을 수 없음!");
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
        projectile.GetComponent<MonsterProjectile>().InitProjectile(attackDir);
    }

    private void Fail()
    {
        ThisTask.Fail();
        recoveryTimer = null;
        prepareTimer = null;
    }
}
