using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

[RequireComponent(typeof(MonsterState))]
public class Task_BossPhaseChange : Task_Base
{
    [SerializeField]
    private MonsterState bossState;
    [SerializeField]
    private int[] phaseMinHP = null;

    public int currentPhase = 0;    // 디버깅용

    private void Start()
    {
        if (bossState == null)
        {
            bossState = GetComponent<MonsterState>();
            Debug.Assert(bossState != null, $"{gameObject.name}: BossState를 찾을 수 없음!");
        }
        Debug.Assert(phaseMinHP != null, "보스의 페이즈 별 HP를 설정해야 함!");
    }

    [Task]
    private bool isHpOverPhaseLimit(int phaseNum)
    {
        if (bossState.HP >= phaseMinHP[phaseNum])
        {
            return true;
        }
        else
        {
            currentPhase = phaseNum + 1;
            return false;
        }
    }
}
