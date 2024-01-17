using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

public class AITask_RangeAttack_GroundTargetAOE : AITask_Base
{
    [SerializeField]
    protected Blackboard blackboard;

    [Header("공격 관련")]
    [SerializeField, Tooltip("범위 공격 프리팹")]
    private GameObject AoePrefab;
    [SerializeField, Tooltip("공격 선딜레이")]
    private float startupDuration;
    [SerializeField, Tooltip("공격 후딜레이")]
    private float recoveryDuration;

    private Timer startupTimer = null;
    private Timer recoveryTimer = null;
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
    private void Attack()
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

        if (!startupTimer && !recoveryTimer) // 공격의 첫 프레임
        {
            // 공격(범위 미리보기) 오브젝트 소환 & 선딜레이 타이머 시작
            SpawnAttackObject();
            startupTimer = Timer.StartTimer();
        }
        else if (startupTimer && startupTimer.duration < startupDuration)   // 선딜레이 중
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

    private void SpawnAttackObject()
    {
        Debug.Log("공격 소환");
        // 적(플레이어) 위치 파악
        GameObject enemy;
        if (!blackboard.TryGet(BBK.Enemy, out enemy) || enemy == null)
        {
            Debug.LogError($"{gameObject.name}: Attack에서 적을 찾을 수 없음!");
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

    private void DoAttack()
    {
        // 공격 범위 미리보기를 실제 공격으로 변환
        attackInstance.ExecuteAttack();
    }

    private void Succeed()
    {
        ThisTask.Succeed();
        startupTimer = null;
        recoveryTimer = null;
    }

    private void Fail()
    {
        // 선딜레이 상황인 경우, 소환된 미리보기 오브젝트를 삭제
        if(startupTimer != null)
            attackInstance.CancelAttack();
        ThisTask.Fail();
        startupTimer = null;
        recoveryTimer = null;
    }
}
