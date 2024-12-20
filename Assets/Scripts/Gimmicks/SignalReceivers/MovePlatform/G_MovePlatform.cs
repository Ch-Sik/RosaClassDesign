using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_MovePlatform : GimmickSignalReceiver
{
    [SerializeField] private MovePlatform mp;

    public override void ImmediateOffAct()
    {
        mp.ImmediateOffAct();
    }

    public override void ImmediateOnAct()
    {
        mp.ImmediateOnAct();
    }

    public override void OffAct()
    {
        mp.OffAct();
    }

    public override void OnAct()
    {
        mp.OnAct();
    }
}