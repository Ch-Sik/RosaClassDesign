using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using UnityEditor.Experimental.GraphView;

public class AITask_MeleeAttack : MonoBehaviour
{
    [SerializeField]
    private Blackboard blackboard;

    [SerializeField, Tooltip("공격 '시전' 사거리. 실제 공격이 닿는 거리와는 별개임에 주의")]
    private float attackRange = 2.0f;

    private void Start()
    {
        if (blackboard == null)
        {
            blackboard = GetComponent<Blackboard>();
            if (blackboard == null)
                Debug.LogError($"{gameObject.name}: Blackboard를 찾을 수 없음!");
        }
    }

    [Task]
    private void IsEnemyCloseEnough()
    {
        GameObject enemy;
        if (!blackboard.TryGet(BBK.Enemy, out enemy) || enemy == null)
        {
            ThisTask.Fail();
            return;
        }

        float distanceX = Mathf.Abs(enemy.transform.position.x
                                    - gameObject.transform.position.x);
        if (distanceX <= attackRange)
            ThisTask.Succeed();
        else
            ThisTask.Fail();
    }

    [Task]
    private void Attack()
    {
        // TODO: Attack 구현
        ThisTask.Succeed();
        Debug.Log("공격!");
    }
}
