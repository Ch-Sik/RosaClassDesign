using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_Lever : GimmickSignalSender
{
    public bool isOnce = true;
    [SerializeField] Transform leverHandle;

    public bool onAct = false;

    private IEnumerator ActivateLever()
    {
        DOTween.Sequence()
            .AppendCallback(() => onAct = true)
            .Append(leverHandle.DORotate(new Vector3(0, 0, -90), 0.4f, RotateMode.LocalAxisAdd).SetRelative(true))
            .AppendCallback(() =>
            {
                onAct = false;
                SendSignal();
            });
        yield return 0;
    }

    private IEnumerator InActivateLever()
    {
        DOTween.Sequence()
            .AppendCallback(() => onAct = true)
            .Append(leverHandle.DORotate(new Vector3(0, 0, +90), 0.4f, RotateMode.LocalAxisAdd).SetRelative(true))
            .AppendCallback(() =>
            {
                onAct = false;
                SendSignal();
            });
        yield return 0;
    }

    public void LeverAction()
    {
        // Debug.Log("11");

        if (onAct)
            return;

        // Debug.Log("22");

        if (isActive)
        {
            // Debug.Log("33");
            if (isOnce)
                return;
            // Debug.Log("44");
            InActiveLever();
        }
        else
        {
            // Debug.Log("55");
            ActiveLever();
        }
    }

    public void ActiveLever()
    {
        isActive = true;
        StartCoroutine(ActivateLever());
        Debug.Log("66");
    }

    public void InActiveLever()
    {
        isActive = false;
        StartCoroutine(InActivateLever());
        Debug.Log("77");
    }
}
