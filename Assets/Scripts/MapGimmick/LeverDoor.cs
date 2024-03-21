using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverDoor : LeverGear
{
    [SerializeField] private new BoxCollider2D collider;
    [SerializeField] private Transform doorSprite;
    [SerializeField] private float openTime;
    [SerializeField] private float openDelay = 2f;

    // 세이브/로드 될 때 초기화용
    public void Init(bool activated)
    {
        if(activated)
        {
            collider.enabled = false;
            doorSprite.localPosition = new Vector3(0, 2.5f, 0);
        }
    }

    public override void Activate()
    {
        Debug.Log("레버 작동됨");
        Sequence sq = DOTween.Sequence()
            .AppendInterval(openDelay)
            .Append(doorSprite.DOMoveY(1.5f, openTime * 0.6f).SetRelative(true))
            .AppendCallback(() =>
            {
                collider.enabled = false;
            })
            .Append(doorSprite.DOMoveY(1f, openTime * 0.4f).SetRelative(true));
    }

    public override void Activate(int value)
    {
        throw new System.NotImplementedException();
    }
}
