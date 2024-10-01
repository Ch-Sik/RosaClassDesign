using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

public class Task_CountCheck : MonoBehaviour
{
    [SerializeField] Blackboard blackboard;

    private void Start()
    {
        Debug.Assert(blackboard != null);
    }

    // 쿨타임이 다 지났다면 Succeed, 남았다면 Fail
    [Task]
    private bool CheckCountGEQ(string key, int value)
    {
        int count;
        if (blackboard.TryGet(key, out count) && count >= value)
            return true;
        else
            return false;
    }

    // '마지막 시전 이후 지난 시간'을 강제로 지정된 값으로 설정
    [Task]
    private void ResetCount(string key)
    {
        blackboard.Set(key, 0);
        ThisTask.Succeed();
        return;
    }

    [Task]
    private void AddCount(string key)
    {
        int count;
        if(blackboard.TryGet(key, out count))
        {
            count++;
            blackboard.Set(key, count);
        }
        else
        {
            blackboard.Set(key, 1);
        }
        ThisTask.Succeed();
        return;
    }
}
