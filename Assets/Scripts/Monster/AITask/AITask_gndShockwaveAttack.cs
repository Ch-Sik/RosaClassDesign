using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using System.Runtime.Serialization;

/// <summary>
/// 보스가 사용하는 충격파 공격
/// </summary>
public class AITask_gndShockwaveAttack : AITask_AttackBase
{
    [SerializeField]
    private GameObject attackPrefab;
    [SerializeField]
    private Transform muzzle;

    private void Start()
    {
        if(blackboard == null)
        {
            blackboard = GetComponent<Blackboard>();
            Debug.Assert(blackboard != null, $"{gameObject.name}: Blackboard를 찾을 수 없음");
        }
        Debug.Assert(attackPrefab != null, $"{gameObject.name}: 충격파공격 프리팹이 설정되어있지 않음");
    }

    [Task]
    private void ShockwaveAttack()
    {
        _Attack();
    }

    // 공격 패턴을 구체적으로 지정하지 않고 대충 Attack()으로 뭉뚱그려 작성된 BT 스크립트 호환용
    [Task]
    private void Attack()
    {
        _Attack();
    }

    protected override void OnAttackStartupBeginFrame()
    {
        GameObject enemy;
        blackboard.TryGet(BBK.Enemy, out enemy);
        Debug.Assert(enemy != null, "충격파 패턴: 적을 찾을 수 없음!");

        lookAt2D(enemy.transform.position);
    }

    protected override void OnAttackActiveBeginFrame()
    {
        EmitShockwave();
    }

    protected virtual void EmitShockwave()
    {
        // 적(플레이어)의 위치 파악
        GameObject enemy;
        blackboard.TryGet(BBK.Enemy, out enemy);
        Vector2 dir = enemy.transform.position - transform.position;

        GameObject shockwave = Instantiate(attackPrefab, muzzle.position, Quaternion.identity);
        shockwave.GetComponent<MonsterShockwave>().Init(dir.toLR());
    }
}
