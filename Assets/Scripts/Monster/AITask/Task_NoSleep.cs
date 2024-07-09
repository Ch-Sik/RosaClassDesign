using UnityEngine;
using Panda;
using Sirenix.OdinInspector;

public class Task_NoSleep : MonoBehaviour
{
    [InfoBox("Sleep을 사용한 BT를 재활용하기 위한 가짜 Sleep task. 실행되면 무조건 즉시 Fail한다.")]
    Blackboard blackboard;
    // Start is called before the first frame update

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
        blackboard.Set(BBK.isWokeUp, true);
        ThisTask.Fail();
    }
}
