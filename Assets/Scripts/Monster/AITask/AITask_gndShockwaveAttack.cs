using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using System.Runtime.Serialization;

/// <summary>
/// 보스가 사용하는 충격파 공격
/// </summary>
public class AITask_gndShockwaveAttack : MonoBehaviour
{
    [SerializeField]
    private Blackboard blackboard;
    [SerializeField]
    private GameObject attackPrefab;
    [SerializeField]
    private Transform muzzle;

    [SerializeField]
    private float startupDuration;
    [SerializeField]
    private float recoveryDuration;

    private Timer startupTimer;
    private Timer recoveryTimer;

    private void Start()
    {
        if(blackboard == null)
        {
            blackboard = GetComponent<Blackboard>();
            Debug.Assert(blackboard != null, $"{gameObject.name}: Blackboard를 찾을 수 없음");
        }
        Debug.Assert(attackPrefab != null, $"{gameObject.name}: 충격파공격 프리팹이 설정되어있지 않음");
    }

    // 공격 패턴을 구체적으로 지정하지 않고 대충 Attack()으로 뭉뚱그려 작성된 BT 스크립트 호환용
    [Task]
    private void Attack()
    {
        ShockwaveAttack();
    }

    [Task]
    private void ShockwaveAttack()
    {
        if (!startupTimer && !recoveryTimer) // 공격의 첫 프레임
        {
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

    private void DoAttack()
    {
        // 적(플레이어)의 위치 파악
        GameObject enemy;
        blackboard.TryGet(BBK.Enemy, out enemy);
        Vector2 dir = enemy.transform.position - transform.position;

        GameObject shockwave = Instantiate(attackPrefab, muzzle.position, Quaternion.identity);
        shockwave.GetComponent<MonsterShockwave>().Init(dir.toLR());
    }

    private void Succeed()
    {
        ThisTask.Succeed();
        startupTimer = null;
        recoveryTimer = null;
    }
}
