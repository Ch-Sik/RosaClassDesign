using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

/// <summary>
/// 몬스터의 피격 담당
/// </summary>
public class Task_HittReaction : Task_Base
{
    [SerializeField]
    private Blackboard blackboard;

    [SerializeField]
    private float hittReactionTime;    // 피격 리액션 유지 시간
    [SerializeField]
    private bool lookPlayerIfHitt;      // 피격 되었을 때 플레이어 바라보기

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
            ThisTask.Fail();
        }
        if (hitResult == true)
        {
            // Debug.Log("피격 당함");
            ThisTask.Succeed();
        }
        else
        {
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
            // lookPlayerIfHitt 옵션이 참이라면, 피격 되었을 때 플레이어 바라보기
            if(lookPlayerIfHitt)
            {
                LookAt2D(PlayerRef.Instance.transform.position);
            }
            return;
        }
        if(hittReactionTimer.duration >= hittReactionTime)
        {
            hittReactionTimer = null;
            ThisTask.Succeed();
        }
    }
}
