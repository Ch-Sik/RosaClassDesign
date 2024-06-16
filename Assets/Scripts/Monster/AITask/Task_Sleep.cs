using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using Sirenix.OdinInspector;

public class Task_Sleep : Task_Base
{
    [InfoBox("Sleep은 적을 발견할 때까지 아무것도 하지 않으며, 적을 발견하면 Fail한다.")]
    Blackboard blackboard;
    [SerializeField]
    private bool lookatEnemyOnWakeup = false;
    [SerializeField]
    protected float wakeupDuration = 0.5f;
    protected Timer wakeupTimer = null;

    protected void Start()
    {
        if(blackboard == null)
        {
            blackboard = GetComponent<Blackboard>();
            Debug.Assert(blackboard != null);
        }
        blackboard.Set(BBK.isWokeUp, false);
    }

    [Task]
    protected void Sleep()
    {
        GameObject enemy;
        blackboard.TryGet(BBK.Enemy, out enemy);
        if (enemy != null && wakeupTimer == null)
        {
            wakeupTimer = Timer.StartTimer();
            // 애니메이션 때문에 플래그는 적 인식 즉시 설정.
            blackboard.Set(BBK.isWokeUp, true);
            if(lookatEnemyOnWakeup)
                LookAt2D(enemy.transform.position);
        }
        if(wakeupTimer != null)
        {
            ThisTask.debugInfo = $"sleep: {wakeupTimer.duration}";
            if(wakeupTimer.duration > wakeupDuration)
            {
                // 실제 AI 행동을 막기 위해 Fail로 task 끝나는 거는 타이머 끝나고 수행
                ThisTask.Fail();
            }
        }
    }
}
