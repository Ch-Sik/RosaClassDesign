using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

/// <summary>
/// BT에서 특정 조건 하에서만 패턴을 쓰게 하고 싶을 때 쓰는 판단용 Task들 모음
/// </summary>
[RequireComponent(typeof(Blackboard))]
public class Task_PatternSelector : Task_Base
{
    [SerializeField]
    private Blackboard blackboard;

    // Start is called before the first frame update
    private void Start()
    {
        if(blackboard == null)
        {
            blackboard = GetComponent<Blackboard>();
            Debug.Assert(blackboard != null);
        }
    }

    [Task]
    public bool isEnemyCloserThan(float dist)
    {
        GameObject enemy;
        blackboard.TryGet(BBK.Enemy, out enemy);

        if (enemy == null)
            return false;

        float toEnemySqr = (enemy.transform.position - transform.position).sqrMagnitude;
        Debug.Log($"적과의 거리: {Mathf.Sqrt(toEnemySqr)}");
        if( toEnemySqr <= dist * dist )
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
