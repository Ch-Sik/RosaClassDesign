using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

/// <summary>
/// 몬스터의 피격 담당
/// </summary>
public class MonsterHitt : MonoBehaviour
{
    [SerializeField]
    private Blackboard blackboard;
    [SerializeField]
    private MonsterState state;

    [SerializeField, Tooltip("true면 공격에 맞아도 체력 닳지 않음")]
    private bool isInvincible;
    [SerializeField, Tooltip("true면 공격에 맞아도 AI 행동이 중단되지 않음")]
    private bool isSuperArmor;
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
        if (state == null)
        {
            state = GetComponent<MonsterState>();
            if (state == null)
                Debug.LogError($"{gameObject.name}: MonsterState를 찾을 수 없음!");
        }
    }

    /// <summary>
    /// player Attack에서 호출되어 데미지와 피격 리액션을 처리하는 함수
    /// </summary>
    /// <param name="Damage">데미지 수치</param>
    public void Hitt(int Damage)
    {
        blackboard.Set("Hitt", true);
        if (!isInvincible)
        {
            state.TakeDamage(Damage);
        }
        if (!isSuperArmor)
        {

        }
    }

    [Task]
    public void IsHitt()
    {
        bool hitResult;
        if(!blackboard.TryGet("Hitt", out hitResult))
        {
            ThisTask.Fail();
        }
        if (hitResult == true)
        {
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
            hittReactionTimer = Timer.StartTimer(hittReactionTime);
            ThisTask.debugInfo = $"t: {hittReactionTime - hittReactionTimer.duration}";
            return;
        }
        if(hittReactionTimer.duration >= hittReactionTime)
        {
            hittReactionTimer = null;
            ThisTask.Succeed();
        }
    }

    public void SetInvincible(bool value)
    {
        isInvincible = value;
    }

    public void SetSuperArmor(bool value)
    {
        isSuperArmor = value;
    }
}
