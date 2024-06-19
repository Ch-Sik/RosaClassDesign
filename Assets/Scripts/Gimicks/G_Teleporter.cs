using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_Teleporter : MonoBehaviour
{
    public G_Teleporter opposite;
    public Transform dest;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player")
            return;

        collision.transform.position = opposite.dest.position;
    }
}
