using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_FallingObjectDamage : MonoBehaviour
{
    public Vector2 respawnPoint;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.transform.position = respawnPoint;
        }
    }
}
