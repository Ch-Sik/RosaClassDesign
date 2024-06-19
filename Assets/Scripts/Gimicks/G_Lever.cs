using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class G_Lever : MonoBehaviour
{
    public bool isActive;
    [SerializeField] Transform leverHandle;


    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Hit");
        if (isActive) return;        // 루틴 중복 실행 방지
        isActive = true;
        StartCoroutine(ActivateLever());
    }

    private IEnumerator ActivateLever()
    {
        DOTween.Sequence()
            .Append(leverHandle.DORotate(new Vector3(0, 0, -45), 0.4f, RotateMode.FastBeyond360));
        yield return 0;
    }
}
