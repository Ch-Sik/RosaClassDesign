using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using Sirenix.OdinInspector;

/// <summary>
/// 몬스터의 체력 등 상태를 관리. 몬스터가 사망했을 때의 이벤트도 여기서 처리함.
/// </summary>
public class MonsterState : MonoBehaviour
{
    [SerializeField] private int maxHP = 5;
    [SerializeField] private int currentHP;
    [SerializeField] private Blackboard blackboard;
    [HideInInspector] public G_MobCounter mobCounter;

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

    [Button]
    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        if(currentHP <= 0)
        {
            BroadcastMessage("OnDie");
            // Blackboard의 isDead 항목은 DamageReceiver에서 관리함
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

        // 레이어 바꿔서 플레이어나 다른 몬스터와 충돌 방지
        gameObject.layer = LayerMask.NameToLayer("DeadObjects");

        StartCoroutine(DieRoutine());
        IEnumerator DieRoutine()        // 사망 연출
        {
            float frameTime;

            pandaBT.enabled = false;
            frameTime = Time.deltaTime;
            yield return new WaitForSeconds(3.0f - frameTime);

            // AI Sensor들과 Empty Parent로 묶여있는 것 고려, 부모 삭제
            mobCounter?.DieSignal();
            Destroy(gameObject.transform.parent.gameObject);
        }
    }
}
