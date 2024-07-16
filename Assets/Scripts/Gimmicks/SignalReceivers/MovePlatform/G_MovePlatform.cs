using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_MovePlatform : GimmickSignalReceiver
{
    [SerializeField] private MovePlatform mp;

    public override void OffAct() { mp.enabled = false; }

    public override void OnAct() { mp.enabled = true; }
}
