using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCucumber : MagicObject
{
    public void CucumberDash()
    {
        // 플레이어를 오이 위치로 끌어당기기
        Sequence doRide = DOTween.Sequence()
            .Append(PlayerRef.Instance.transform.DOMove(transform.position + new Vector3(0, 0.8f, 0), 0.1f));

        PlayerRef.Instance.Move.PrepareSuperDash();
    }
}
