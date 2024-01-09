using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverDoor : LeverGear
{
    // TODO: 스프라이트 컬러로 때워놓은 연출 애니메이션 적용하고 개선.
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private new BoxCollider2D collider;

    public override void Activate()
    {
        Debug.Log("레버 작동됨");
        sprite.color = Color.gray;
        collider.enabled = false;
    }
}
