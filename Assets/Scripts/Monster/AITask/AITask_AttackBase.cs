using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using System;

public class AITask_AttackBase : AITask_Base
{
    [SerializeField]
    private float startupDuration;
    [SerializeField]
    private float activeDuration;
    [SerializeField]
    private float recoveryDuration;

    private Timer startupTimer;
    private Timer activeTimer;
    private Timer recoveryTimer;

    [Task]
    protected void _Attack()
    {
        if (!startupTimer && !activeTimer && !recoveryTimer)
        {
            // 공격의 첫 프레임
            OnAttackStartupBeginFrame();
            startupTimer = Timer.StartTimer();
        }
        else if (startupTimer && startupTimer.duration < startupDuration)   
        {
            // 선딜레이 중
            OnAttackStartupFrames();
            ThisTask.debugInfo = $"선딜레이: {startupTimer.duration}";
        }
        else if (!activeTimer)
        {
            // 공격 시전 첫 프레임
            OnAttackActiveBeginFrame();
            activeTimer = Timer.StartTimer();
        }
        else if (activeTimer && activeTimer.duration < activeDuration)
        {
            // (즉발 공격이 아니라면) 공격 시전 중
            OnAttackActiveFrames();
            ThisTask.debugInfo = $"공격 지속: {activeTimer.duration}";
        }
        else if (!recoveryTimer)
        {
            // 후딜레이 첫 프레임
            OnAttackRecoveryBeginFrame();
            recoveryTimer = Timer.StartTimer();
        }
        else if (recoveryTimer && recoveryTimer.duration < recoveryDuration)
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
    }

    protected virtual void OnAttackStartupBeginFrame() { }
    protected virtual void OnAttackStartupFrames() { }
    protected virtual void OnAttackActiveBeginFrame() { }
    protected virtual void OnAttackActiveFrames() { }
    protected virtual void OnAttackRecoveryBeginFrame() { }
    protected virtual void OnAttackRecoveryFrames() { }
    protected virtual void OnAttackEndFrame() { }

    protected virtual void Succeed()
    {
        ThisTask.Succeed();
        startupTimer = null;
        activeTimer = null;
        recoveryTimer = null;
    }

    protected virtual void Fail()
    {
        ThisTask.Fail();
        startupTimer = null;
        activeTimer = null;
        recoveryTimer = null;
    }
}
