using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_Lever : GimmickSignalSender
{
    public bool isOnce = true;
    [SerializeField] Transform leverHandle;

    public bool onAct = false;
    [SerializeField] private InteractiveObject interactiveObject;

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

    [Button]
    public void LeverAction()
    {

        if (onAct)
            return;


        if (isActive)
        {
            if (isOnce)
                return;
            InActiveLever();
        }
        else
        {
            ActiveLever();
        }
    }
    
    public void ActiveLever()
    {
        isActive = true;
        StartCoroutine(ActivateLever());

        if (isOnce)
        {
            interactiveObject.canUse = false;
            interactiveObject.OnInactive();
        }
    }

    public void InActiveLever()
    {
        isActive = false;
        StartCoroutine(InActivateLever());
    }
}
