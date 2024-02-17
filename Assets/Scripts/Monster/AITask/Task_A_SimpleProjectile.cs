using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

public class Task_A_SimpleProjectile : Task_A_Base
{
    [Header("공격 관련")]
    [SerializeField, Tooltip("투사체 프리팹")]
    private GameObject projectilePrefab;
    [SerializeField, Tooltip("투사체가 발사되어야 할 위치")]
    private Transform muzzle;
    [SerializeField, Tooltip("공격 방향 옵션")]
    private bool forceDirection90Deg = false;

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
    private void SimpleProjectileAttack()
    {
        ExecuteAttack();
    }

    // 공격 패턴을 구체적으로 지정하지 않고 대충 Attack()으로 뭉뚱그려 작성된 BT 스크립트 호환용
    [Task]
    private void Attack()
    {
        ExecuteAttack();
    }

    protected override void OnActiveBegin()
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
        projectile.GetComponent<MonsterProjectile>().InitProjectile(attackDir.normalized);
    }
}
