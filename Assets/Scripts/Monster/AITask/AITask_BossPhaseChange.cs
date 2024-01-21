using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

[RequireComponent(typeof(MonsterState))]
public class AITask_BossPhaseChange : AITask_Base
{
    [SerializeField]
    private MonsterState bossState;
    [SerializeField]
    private int[] phaseMinHP = null;

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
        return (bossState.HP >= phaseMinHP[phaseNum]);
    }
}
