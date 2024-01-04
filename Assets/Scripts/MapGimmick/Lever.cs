using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    [SerializeField] bool leverActivated = false;
    [SerializeField] LeverGear leverGear;
    [SerializeField] Transform leverHandle;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (leverActivated) return;        // 루틴 중복 실행 방지
        leverActivated = true;

        if(leverGear == null)
        {
            Debug.LogWarning($"레버로 인해 작동할 문이 지정되어있지 않음!");
            return;
        }

        StartCoroutine(ActivateLever());
    }

    private IEnumerator ActivateLever()
    {
        DOTween.Sequence()
            .Append(leverHandle.DORotate(new Vector3(0, 0, -45), 0.4f, RotateMode.FastBeyond360))
            .AppendCallback(() => { leverGear.Activate(); });
        yield return 0;
    }
}
