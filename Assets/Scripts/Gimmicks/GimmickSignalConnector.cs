using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimmickSignalConnector : MonoBehaviour
{
    public List<GimmickSignalReceiver> gimmicks = new List<GimmickSignalReceiver>();
    public List<GimmickSignalSender> signals = new List<GimmickSignalSender>();

    public bool isActive = false;
    private bool curIsActive = false;

    public void Start()
    {
        Init();
    }

    private void Init()
    {
        for (int i = 0; i < gimmicks.Count; i++)
            gimmicks[i]?.OffAct();
        for (int i = 0; i < signals.Count; i++)
            signals[i]?.SetHandler(this);
    }

    public bool Signal()
    {
        for (int i = 0; i < signals.Count; i++)
            if (!signals[i].isActive)
            {
                isActive = false;
                ChangeState();
                return false;
            }

        isActive = true;
        ChangeState();
        return true;
    }

    public void ChangeState()
    {
        if (curIsActive == isActive)
            return;

        curIsActive = isActive;

        if (isActive) OnAct();
        else OffAct();
    }

    public void OnAct()
    {
        for (int i = 0; i < gimmicks.Count; i++)
            gimmicks[i].OnAct();
    }

    public void OffAct()
    {
        for(int i = 0; i < gimmicks.Count; i++)
            gimmicks[i].OffAct();
    }
}
