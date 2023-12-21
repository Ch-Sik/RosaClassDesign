using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

/// <summary>
/// 몬스터의 피격 담당
/// </summary>
public class AITask_HittReaction : MonoBehaviour
{
    [SerializeField]
    private Blackboard blackboard;

    [SerializeField]
    private float hittReactionTime;    // 피격 리액션 유지 시간

    private Timer hittReactionTimer = null;

    private void Awake()
    {
        if (blackboard == null)
        {
            blackboard = GetComponent<Blackboard>();
            if (blackboard == null)
                Debug.LogError($"{gameObject.name}: Blackboard를 찾을 수 없음!");
        }
    }

    [Task]
    public void IsHitt()
    {
        bool hitResult;
        if(!blackboard.TryGet(BBK.isHitt, out hitResult))
        {
            Debug.Log("IsHitt? NO");
            ThisTask.Fail();
        }
        if (hitResult == true)
        {
            Debug.Log("IsHitt? YES");
            ThisTask.Succeed();
        }
        else
        {
            Debug.Log("IsHitt? NO");
            ThisTask.Fail();
        }
    }

    /// <summary>
    /// 피격 애니메이션 적용은 MonsterAnimation에서 수행하고 여기서는 단순히 AI가 멈추는 것만 처리함.
    /// </summary>
    [Task]
    public void HittReaction()
    {
        if (hittReactionTimer == null)
        {
            hittReactionTimer = Timer.StartTimer();
            ThisTask.debugInfo = $"t: {hittReactionTime - hittReactionTimer.duration}";
            return;
        }
        if(hittReactionTimer.duration >= hittReactionTime)
        {
            hittReactionTimer = null;
            ThisTask.Succeed();
        }
    }
}
