using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 몹 개발이 완료되면, 죽은 몹의 이벤트를 받아서 통합하고, 이를 send하는 방식으로 쓰는 것이 좋을듯.
/// </summary>

public class G_MobCounter : GimmickSignalSender
{
    #region State

    public override void Init(int state)
    {
        SetState(state);
        switch (state)
        {
            case 0:
                isActive = false;
                break;
            case 1: // Active
                isActive = true;
                break;
            case 2: // InActive
                isActive = false;
                break;
        }
        ImmediateSendSignal();
    }

    #endregion

    public int fullCount;
    public int deadMobCount;
    public List<MonsterState> mobs = new List<MonsterState>();

    public void Start()
    {
        foreach (var mob in mobs)
            mob.OnDead += DieSignal;

        fullCount = mobs.Count;
    }

    public void DieSignal(GameObject monster)
    {
        //이미 클리어로 기록된다면,
        if (GetState() == 1)
            return;

        deadMobCount++;

        if (deadMobCount == fullCount)
        {
            SetState(1);
            isActive = true;
            SendSignal();
        }
    }
}
