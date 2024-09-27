using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 몹 개발이 완료되면, 죽은 몹의 이벤트를 받아서 통합하고, 이를 send하는 방식으로 쓰는 것이 좋을듯.
/// </summary>

public class G_MobCounter : GimmickSignalSender
{
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
        deadMobCount++;

        if (deadMobCount == fullCount)
        {
            isActive = true;
            SendSignal();
        }
    }
}
