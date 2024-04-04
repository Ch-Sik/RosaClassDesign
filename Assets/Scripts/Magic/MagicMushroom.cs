using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMushroom : MagicObject
{
    [SerializeField] float jumpPower; // 점프력

    public void MushJump(Collider2D collision)
    {
        if (PlayerRef.Instance.Controller.isMIDAIR)
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = (Vector2.up * jumpPower);
    }
}
