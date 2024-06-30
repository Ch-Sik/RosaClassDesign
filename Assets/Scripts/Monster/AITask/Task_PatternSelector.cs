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
    [SerializeField]
    private bool isTock = false;

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

    [Task]
    public bool isEnemyAtFront()
    {
        GameObject enemy;
        blackboard.TryGet(BBK.Enemy, out enemy);
        if (enemy == null) return false;

        bool isFacingRight = transform.localScale.x > 0f;
        bool isEnemyAtRight = (enemy.transform.position - transform.position).x > 0f;
        return isFacingRight == isEnemyAtRight;
    }

    // true와 false를 한 번씩 번갈아가면서 반환 
    [Task]
    public bool TikTok()
    {
        isTock = !isTock;
        return !isTock;
    }
}
