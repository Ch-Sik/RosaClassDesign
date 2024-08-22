using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMushroom : MonoBehaviour
{
    [SerializeField] float jumpPower; // 점프력

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if (PlayerRef.Instance.movement.isGrounded)
                PlayerRef.Instance.movement.MushJump();
        }
        
        if(collision.gameObject.CompareTag("Cube"))
        {
            int dir;
            if (gameObject.transform.position.x - collision.transform.position.x > 0) dir = -1;
            else dir = 1;
            collision.gameObject.GetComponent<G_Cube>().MushJump(dir);
        }
        
    }
}
