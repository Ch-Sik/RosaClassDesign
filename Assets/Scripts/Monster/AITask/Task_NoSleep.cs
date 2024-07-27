using UnityEngine;
using Panda;
using Sirenix.OdinInspector;

public class Task_NoSleep : MonoBehaviour
{
    [InfoBox("Sleep을 사용한 BT를 재활용하기 위한 가짜 Sleep task. 실행되면 무조건 즉시 Fail한다.")]
    Blackboard blackboard;

    [Tooltip("대기 모드에서 깨어나는 동작 시간 (초)")]
    [SerializeField] private float wakeupDuration = 0.5f;

    private Timer timer;

    private void Start()
    {
        if (blackboard == null)
        {
            blackboard = GetComponent<Blackboard>();
            Debug.Assert(blackboard != null);
        }
    }

    [Task]
    protected void Sleep()
    {
        if (timer == null)
        {
            timer = Timer.StartTimer();
            blackboard.Set(BBK.isWokeUp, true);
        }

        if (timer.duration > wakeupDuration)
        {
            timer = null;
            // 실제 AI 행동을 막기 위해 Fail로 task 끝나는 거는 타이머 끝나고 수행
            ThisTask.Fail();
        }
    }
}
