using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMushCollider : MonoBehaviour
{
    private MagicMushroom mushroom;

    // Start is called before the first frame update
    void Start()
    {
        mushroom = GetComponentInParent<MagicMushroom>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            mushroom.MushJump(collision);
        }
    }
}
