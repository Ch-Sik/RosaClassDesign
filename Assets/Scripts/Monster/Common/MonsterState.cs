using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

/// <summary>
/// 몬스터의 체력 등 상태를 관리. 몬스터가 사망했을 때의 이벤트도 여기서 처리함.
/// </summary>
public class MonsterState : MonoBehaviour
{
    [SerializeField] private int maxHP = 5;
    [SerializeField, ReadOnly] private int currentHP;
    [SerializeField] private Blackboard blackboard;

    public int HP { get { return currentHP; } }

    private void Start()
    {
        if(blackboard == null)
        {
            blackboard = GetComponent<Blackboard>();
            Debug.Assert(blackboard != null, $"{gameObject.name}: Blackboard를 찾을 수 없음!");
        }

        currentHP = maxHP;
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        if(currentHP <= 0)
        {
            BroadcastMessage("OnDie");
        }
    }

    [ContextMenu("OnDie")]  // ContextMenu는 디버깅용
    private void OnDie()
    {
        var pandaBT = GetComponent<PandaBehaviour>();
        if(pandaBT == null) 
        {
            Debug.LogError($"{gameObject.name}: 사망 시 멈출 pandaBT를 찾을 수 없음!");
            return;
        }
        pandaBT.enabled = false;

        StartCoroutine(DieRoutine());
        IEnumerator DieRoutine()        // 사망 연출
        {
            blackboard.Set(BBK.isDead, true);
            yield return new WaitForSeconds(3.0f);
            // TODO: 여기에 사망 연출 추가하기
            Destroy(gameObject);
        }
    }
}
