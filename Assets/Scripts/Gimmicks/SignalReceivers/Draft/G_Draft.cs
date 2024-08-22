using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_Draft : GimmickSignalReceiver
{
    public bool isActivated = false;
    public float risingPower = 3.0f;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isActivated)
            return;
        if (!collision.CompareTag("Player"))
            return;

        PlayerRef.Instance.Move.Rising(risingPower);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isActivated)
            return;
        if (!collision.CompareTag("Player"))
            return;

        PlayerRef.Instance.Move.CancleRising();
    }

    public override void OnAct()
    {
        isActivated = true;
    }

    public override void OffAct()
    {
        isActivated = false;
    }
}
