using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_BlinkPlatform : MonoBehaviour
{
    // 시작은 무조건 onTime
    [SerializeField] float onTime = 1.0f;
    [SerializeField] float offTime = 1.0f;
    [SerializeField] bool reverseOnOff = false; // OnTime 때 사라지고 OffTime 때 나타나기

    // On/Off 때 비활성화할 콜라이더와 렌더러.
    // 게임오브젝트 전체를 비활성화하면 플레이어가 플랫폼 자식으로 들어갔을 때
    // 플레이어까지 통째로 사라질 수 있음.
    [SerializeField] new Collider2D collider;
    [SerializeField] Renderer rendrer;

    private void Start()
    {
        Toggle(true);
        DOTween.Sequence()
            .AppendInterval(onTime)
            .AppendCallback(() => { Toggle(false); })
            .AppendInterval(offTime)
            .AppendCallback(() => { Toggle(true); })
            .SetLoops(-1);
    }

    private void Toggle(bool onoff)
    {
        if(reverseOnOff) onoff = !onoff;

        collider.enabled = onoff;
        rendrer.enabled = onoff;
    }
}