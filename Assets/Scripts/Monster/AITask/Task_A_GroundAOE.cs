using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

/// <summary>
/// 성큰 콜로니같이 지면에서 튀어나오는 범위 공격
/// </summary>
public class Task_A_GroundAOE : Task_A_Base
{
    [Header("공격 관련")]
    [SerializeField, Tooltip("범위 공격 프리팹")]
    private GameObject AoePrefab;

    private MonsterAOE attackInstance = null;

    private void Start()
    {
        if (blackboard == null)
        {
            blackboard = GetComponent<Blackboard>();
            Debug.Assert(blackboard != null, $"{gameObject.name}: Blackboard를 찾을 수 없음");
        }
        Debug.Assert(AoePrefab != null, $"{gameObject.name}: 범위공격 프리팹이 설정되어있지 않음");
    }

    [Task]
    private void GroundAOEAttack()
    {
        ExecuteAttack();
    }

    // 공격 패턴을 구체적으로 지정하지 않고 대충 Attack()으로 뭉뚱그려 작성된 BT 스크립트 호환용
    [Task]
    private void Attack()
    {
        ExecuteAttack();
    }

    protected override void OnStartupBegin()
    {
        // Debug.Log("공격 소환");
        // 적(플레이어) 위치 파악
        GameObject enemy;
        if (!blackboard.TryGet(BBK.Enemy, out enemy) || enemy == null)
        {
            Debug.LogWarning($"{gameObject.name}: Attack에서 적을 찾을 수 없음!");
            Fail();
            return;
        }

        // 플레이어 발 밑 지면의 좌표 얻기
        RaycastHit2D rayhit;
        // TODO: 레이어마스크에 플랫폼도 포함시키기
        rayhit = Physics2D.Raycast(enemy.transform.position, Vector2.down, float.MaxValue, LayerMask.GetMask("Ground"));
        Debug.Assert(rayhit.collider != null, $"{gameObject.name}: 플레이어 발 밑의 지면을 찾을 수 없음!");
        // TODO: 지면 가장자리에 걸쳐있는 경우, 위치 보정해야 됨

        // 소환
        attackInstance = Instantiate(AoePrefab, rayhit.point, Quaternion.identity).GetComponent<MonsterAOE>();
        attackInstance.Init();
    }

    protected override void OnActiveBegin()
    {
        // 공격 범위 미리보기를 실제 공격으로 변환
        attackInstance.ExecuteAttack();
    }

    protected override void Fail()
    {
        base.Fail();
        // Debug.Log($"Fail in Task_A_GroundAOE, curAttackState: {attackState}");
        // 선딜레이 상황인 경우, 소환된 미리보기 오브젝트를 삭제
        if(attackInstance != null && (attackState == MonsterAtttackState.Startup || attackState == MonsterAtttackState.Null))
            attackInstance.CancelAttack();
    }

    protected void OnDie()
    {
        // 범위 미리보기 상태에서는 다른 명령이 오기 전까지 대기하므로
        // 수동으로 캔슬해줘야 몬스터가 사라졌는데도
        // 공격 미리보기가 남아있는 상황을 피할 수 있음.
        if (attackInstance != null && attackState == MonsterAtttackState.Startup)
        {
            // Debug.Log("AOE 오브젝트 수동 캔슬");
            attackInstance.CancelAttack();
        }
    }
}
