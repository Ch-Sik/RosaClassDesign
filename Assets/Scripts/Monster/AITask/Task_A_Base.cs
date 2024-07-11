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
public class Task_A_Base : Task_Base
{
    [SerializeField]
    protected Blackboard blackboard = null;
    protected MonsterDamageReceiver dmgReceiver = null;

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

    // 이거 없어도 구현에는 문제없긴 한데 애니메이션 컨트롤 목적도 겸해서 추가함.
    [SerializeField]
    protected MonsterAtttackState attackState {
        get { return currentState; }
        set {
            currentState = value;
            blackboard.Set(BBK.AttackState, (int)currentState); 
        } 
    }
    [SerializeField]
    private MonsterAtttackState currentState = MonsterAtttackState.Null;

    /// <summary>
    /// <br> 설정한 startup/active/recovery 타이밍에 맞게 공격 이벤트 함수들을 호출함. </br>
    /// <br> 이 이름 그대로면 다양한 패턴(AITask)을 BT에서 구별이 불가능하므로, 상속된 클래스에서
    /// 고유한 이름의 Task 함수를 만들고 이 함수를 호출해줘야 됨. </br>
    /// </summary>
    protected void ExecuteAttack()
    {
        // 사망 시 패턴 종료
        bool isDead;
        blackboard.TryGet(BBK.isDead, out isDead);
        if (isDead)
        {
            Fail();
            return;
        }

        // 공격 active 중에 슈퍼아머 옵션이 꺼져있거나 애초에 공격 active가 아니라면 피격 여부 검사
        if (attackState != MonsterAtttackState.Active || !superArmourOnActiveTime)
        {
            bool isHitt;
            blackboard.TryGet(BBK.isHitt, out isHitt);
            if(isHitt)
            {
                Fail();
                return;
            }
        }

        switch(attackState)
        {
            case MonsterAtttackState.Null:
                // 선딜레이 첫 프레임
                attackState = MonsterAtttackState.Startup;
                startupTimer = Timer.StartTimer();
                OnStartupBegin();
                break;
            case MonsterAtttackState.Startup:
                if(startupTimer.duration < startupDuration)
                {
                    // 선딜레이 중
                    ThisTask.debugInfo = $"선딜레이: {startupTimer.duration}";
                    OnStartupLast();
                }
                else
                {
                    // 공격 시전 첫프레임
                    attackState = MonsterAtttackState.Active;
                    activeTimer = Timer.StartTimer();
                    // 슈퍼아머 효과 필요하다면 적용
                    if(superArmourOnActiveTime)
                    {
                        if (dmgReceiver == null) dmgReceiver = GetComponent<MonsterDamageReceiver>();
                        dmgReceiver.SetTempSuperArmour(true);
                    }
                    OnActiveBegin();
                }
                break;
            case MonsterAtttackState.Active:
                if(activeTimer.duration < activeDuration)
                {
                    // 공격 지속 중
                    ThisTask.debugInfo = $"공격 지속: {activeTimer.duration}";
                    OnActiveLast();
                }
                else
                {
                    // 후딜레이 첫 프레임
                    attackState = MonsterAtttackState.Recovery;
                    recoveryTimer = Timer.StartTimer();
                    // 슈퍼아머 효과 적용했다면 해제
                    if (superArmourOnActiveTime)
                    {
                        dmgReceiver.SetTempSuperArmour(false);
                    }
                    OnRecoveryBegin();
                }
                break;
            case MonsterAtttackState.Recovery:
                if (recoveryTimer.duration < recoveryDuration)
                {
                    // 후딜레이 중
                    OnRecoveryLast();
                    ThisTask.debugInfo = $"후딜레이: {recoveryTimer.duration}";
                }
                else
                {
                    // 후딜 종료, 패턴 완료
                    attackState = MonsterAtttackState.Null;
                    OnEnd();
                    Succeed();
                }
                break;
        }
    }

    /// <summary> 선딜레이 첫 프레임에서 호출되는 함수. 선딜레이가 0이여도 반드시 호출됨. </summary>
    protected virtual void OnStartupBegin() { }
    /// <summary> 선딜레이 프레임에서 반복적 호출되는 함수. 선딜레이가 0이면 호출되지 않음. </summary>
    protected virtual void OnStartupLast() { }
    /// <summary> 공격유지 첫 프레임에서 호출되는 함수. 공격유지시간이 0이여도 반드시 호출됨. </summary>
    protected virtual void OnActiveBegin() { }
    /// <summary> 공격유지 프레임에서 반복적 호출되는 함수. 공격유지시간이 0이면 호출되지 않음. </summary>
    protected virtual void OnActiveLast() { }
    /// <summary> 후딜레이 첫 프레임에서 호출되는 함수. 후딜레이가 0이여도 반드시 호출됨. </summary>
    protected virtual void OnRecoveryBegin() { }
    /// <summary> 후딜레이 프레임에서 반복적 호출되는 함수. 후딜레이가 0이면 호출되지 않음. </summary>
    protected virtual void OnRecoveryLast() { }
    /// <summary> 공격 패턴의 마지막 프레임에서 호출되는 함수. </summary>
    protected virtual void OnEnd() { }

    /// <summary>
    /// <br> 패턴이 성공한 것으로 BT에게 알리고 불필요한 요소들을 정리함. </br>
    /// <br> 필요하다면 override해서 추가 정리 수행 </br>
    /// </summary>
    protected virtual void Succeed()
    {
        ThisTask.Succeed();
        attackState = MonsterAtttackState.Null;
        startupTimer = null;
        activeTimer = null;
        recoveryTimer = null;
        // 슈퍼아머 효과 적용했다면 해제
        if (superArmourOnActiveTime)
        {
            dmgReceiver.SetTempSuperArmour(false);
        }
    }

    /// <summary>
    /// <br> 패턴이 실패한 것으로 BT에게 알리고 불필요한 요소들을 정리함. </br>
    /// <br> 필요하다면 override해서 추가 정리 수행 </br>
    /// </summary>
    protected virtual void Fail()
    {
        ThisTask.Fail();
        attackState = MonsterAtttackState.Null;
        startupTimer = null;
        activeTimer = null;
        recoveryTimer = null;
        // 슈퍼아머 효과 적용했다면 해제
        if (superArmourOnActiveTime)
        {
            dmgReceiver.SetTempSuperArmour(false);
        }
    }

    /// <summary>
    /// <br> Startup 또는 Active 상태인 패턴을 즉시 Recovery 상태로 이행함.</br>
    /// <br> Succeed와 비슷하지만 이 함수는 후딜레이(Recovery)를 수행한다는 차이점이 있음.</br>
    /// </summary>
    protected virtual void SkipToRecovery()
    {
        attackState = MonsterAtttackState.Recovery;
        recoveryTimer = Timer.StartTimer();
        // 슈퍼아머 효과 적용했다면 해제
        if (superArmourOnActiveTime)
        {
            dmgReceiver.SetTempSuperArmour(false);
        }
        OnRecoveryBegin();
    }
}
