using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;


/// <summary>
/// MEMO: 이 스크립트는 공격 패턴 스크립트를 체계화 시키기 전에 작성된 것으로, 처음부터 다시 만들어야 됨.
/// </summary>
public class Task_A_Melee : Task_A_Base
{
    // MonsterAOE랑 melee attack이랑 역할이 겹치는 것 같은데... 애초에 두 공격 방식을 하나로 통합해봐도 좋을 듯?
    [SerializeField, Tooltip("공격 범위를 나타내는 자식오브젝트 컴포넌트")]
    [Header("공격 관련")]
    private MonsterAOE attackInstance = null;

    protected virtual void Start()
    {
        if (blackboard == null)
        {
            blackboard = GetComponent<Blackboard>();
            Debug.Assert(blackboard != null, $"{gameObject.name}: Blackboard를 찾을 수 없음");
        }
    }

    [Task]
    private void MeleeAttack()
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
        Debug.Log("공격 대기");
        attackInstance.Init();
    }

    protected override void OnActiveBegin()
    {
        // 공격 범위 미리보기를 실제 공격으로 변환
        Debug.Log("공격 수행");
        attackInstance.ExecuteAttack();
    }

    protected override void OnRecoveryBegin()
    {
        Debug.Log("공격 종료");
        attackInstance.gameObject.SetActive(false);
    }
}
