using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_FallingObjectPlatform : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsOnTopOfPlatform(collision))
        {
            HandleChildTriggerEnter(collision);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        HandleChildTriggerExit(collision);
    }

    bool IsOnTopOfPlatform(Collision2D collision)
    {
        float platformTopY = transform.position.y + GetComponent<Collider2D>().bounds.size.y / 2;

        float objectBottomY = collision.collider.bounds.min.y;

        return objectBottomY > platformTopY;
    }
    public void HandleChildTriggerEnter(Collision2D other)
    {
        other.transform.SetParent(transform);
    }

    public void HandleChildTriggerExit(Collision2D other)
    {
        if (Application.isPlaying)
        {
            if (other.transform.parent == transform)
            {
                other.transform.SetParent(null);
            }
        }
    }
}
