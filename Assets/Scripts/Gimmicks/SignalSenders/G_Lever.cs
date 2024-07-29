using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_Lever : GimmickSignalSender
{
    [SerializeField] Transform leverHandle;

    private IEnumerator ActivateLever()
    {
        DOTween.Sequence()
            .Append(leverHandle.DORotate(new Vector3(0, 0, -90), 0.4f, RotateMode.FastBeyond360).SetRelative(true))
            .AppendCallback(() => SendSignal());
        yield return 0;
    }

    public void ActiveLever()
    {
        if (isActive) return;        // 루틴 중복 실행 방지

        isActive = true;
        StartCoroutine(ActivateLever());
    }
}
