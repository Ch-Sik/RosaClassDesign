using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using Sirenix.OdinInspector;

public enum MonsterAtttackState { Null, Startup, Active, Recovery }

/// <summary>
/// AttackBase는 몬스터 공격 패턴 구현할 때 자주 사용되는 패턴을 미리 구현해둔 클래스.
/// 상속해서 OnAttack...() 함수들 override하면 간단히 새 패턴을 구현할 수 있음.
/// </summary>
[RequireComponent(typeof(Blackboard))]
public class AITask_AttackBase : AITask_Base
{
    [SerializeField]
    protected Blackboard blackboard = null;

    [FoldoutGroup("기본 설정")]
    [SerializeField, Tooltip("패턴이 ActiveTime일 때 슈퍼아머 부여")]
    protected bool superArmourOnActiveTime = false;

    [FoldoutGroup("기본 설정")]
    [SerializeField]
    protected float startupDuration;
    [FoldoutGroup("기본 설정")]
    [SerializeField]
    protected float activeDuration;
    [FoldoutGroup("기본 설정")]
    [SerializeField]
    protected float recoveryDuration;

    protected Timer startupTimer;
    protected Timer activeTimer;
    protected Timer recoveryTimer;

    [SerializeField, ReadOnly]
    protected MonsterAtttackState currentState;

    /// <summary>
    /// <br> 설정한 startup/active/recovery 타이밍에 맞게 공격 이벤트 함수들을 호출함. </br>
    /// <br> 이 이름 그대로면 다양한 패턴(AITask)을 BT에서 구별이 불가능하므로, 상속된 클래스에서
    /// 고유한 이름의 Task 함수를 만들고 이 함수를 호출해줘야 됨. </br>
    /// </summary>
    [Task]
    protected void _Attack()
    {
        // 공격 active 중에 슈퍼아머 옵션이 꺼져있거나 애초에 공격 active가 아니라면 피격 여부 검사
        if(currentState != MonsterAtttackState.Active || !superArmourOnActiveTime)
        {
            if(blackboard == null)
            {
                blackboard = GetComponent<Blackboard>();
            }

            bool isHitt;
            blackboard.TryGet(BBK.isHitt, out isHitt);
            if(isHitt)
            {
                Fail();
                return;
            }
        }

        switch(currentState)
        {
            case MonsterAtttackState.Null:
                // 선딜레이 첫 프레임
                currentState = MonsterAtttackState.Startup;
                startupTimer = Timer.StartTimer();
                OnAttackStartupBeginFrame();
                break;
            case MonsterAtttackState.Startup:
                if(startupTimer.duration < startupDuration)
                {
                    // 선딜레이 중
                    ThisTask.debugInfo = $"선딜레이: {startupTimer.duration}";
                    OnAttackStartupFrames();
                }
                else
                {
                    // 공격 시전 첫프레임
                    currentState = MonsterAtttackState.Active;
                    activeTimer = Timer.StartTimer();
                    OnAttackActiveBeginFrame();
                }
                break;
            case MonsterAtttackState.Active:
                if(activeTimer.duration < activeDuration)
                {
                    // 공격 지속 중
                    ThisTask.debugInfo = $"공격 지속: {activeTimer.duration}";
                    OnAttackActiveFrames();
                }
                else
                {
                    // 후딜레이 첫 프레임
                    currentState = MonsterAtttackState.Recovery;
                    recoveryTimer = Timer.StartTimer();
                    OnAttackRecoveryBeginFrame();
                }
                break;
            case MonsterAtttackState.Recovery:
                if (recoveryTimer.duration < recoveryDuration)
                {
                    // 후딜레이 중
                    OnAttackRecoveryFrames();
                    ThisTask.debugInfo = $"후딜레이: {recoveryTimer.duration}";
                }
                else
                {
                    // 후딜 종료, 패턴 완료
                    OnAttackEndFrame();
                    Succeed();
                }
                break;
        }
    }

    /// <summary> 선딜레이 첫 프레임에서 호출되는 함수. 선딜레이가 0이여도 반드시 호출됨. </summary>
    protected virtual void OnAttackStartupBeginFrame() { }
    /// <summary> 선딜레이 프레임에서 반복적 호출되는 함수. 선딜레이가 0이면 호출되지 않음. </summary>
    protected virtual void OnAttackStartupFrames() { }
    /// <summary> 공격유지 첫 프레임에서 호출되는 함수. 공격유지시간이 0이여도 반드시 호출됨. </summary>
    protected virtual void OnAttackActiveBeginFrame() { }
    /// <summary> 공격유지 프레임에서 반복적 호출되는 함수. 공격유지시간이 0이면 호출되지 않음. </summary>
    protected virtual void OnAttackActiveFrames() { }
    /// <summary> 후딜레이 첫 프레임에서 호출되는 함수. 후딜레이가 0이여도 반드시 호출됨. </summary>
    protected virtual void OnAttackRecoveryBeginFrame() { }
    /// <summary> 후딜레이 프레임에서 반복적 호출되는 함수. 후딜레이가 0이면 호출되지 않음. </summary>
    protected virtual void OnAttackRecoveryFrames() { }
    /// <summary> 공격 패턴의 마지막 프레임에서 호출되는 함수. </summary>
    protected virtual void OnAttackEndFrame() { }

    /// <summary>
    /// <br> 패턴이 성공한 것으로 BT에게 알리고 불필요한 요소들을 정리함. </br>
    /// <br> 필요하다면 override해서 추가 정리 수행 </br>
    /// </summary>
    protected virtual void Succeed()
    {
        ThisTask.Succeed();
        currentState = MonsterAtttackState.Null;
        startupTimer = null;
        activeTimer = null;
        recoveryTimer = null;
    }

    /// <summary>
    /// <br> 패턴이 실패한 것으로 BT에게 알리고 불필요한 요소들을 정리함. </br>
    /// <br> 필요하다면 override해서 추가 정리 수행 </br>
    /// </summary>
    protected virtual void Fail()
    {
        ThisTask.Fail();
        currentState = MonsterAtttackState.Null;
        startupTimer = null;
        activeTimer = null;
        recoveryTimer = null;
    }
}
