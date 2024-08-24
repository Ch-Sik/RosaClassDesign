using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatformCollider : MonoBehaviour
{
    private MovePlatform platform;
    private void Start()
    {
        platform = GetComponentInParent<MovePlatform>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player") && IsOnTopOfPlatform(collision))
        {
            if (platform != null)
            {
                platform.HandleChildTriggerEnter(collision.transform);
            }
        }
        else if (collision.transform.CompareTag("Cube") && IsCubeOnTopOfPlatform(collision))
        {
            if (platform != null)
            {
                platform.HandleChildTriggerEnter(collision.transform);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Cube") && IsCubeOnTopOfPlatform(collision))
        {
            if (platform != null)
            {
                platform.HandleChildTriggerEnter(collision.transform);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (platform != null)
        {
            platform.HandleChildTriggerExit(collision.transform);
        }
    }

    bool IsOnTopOfPlatform(Collision2D collision)
    {
        float platformTopY = transform.position.y +  GetComponent<Collider2D>().bounds.size.y / 2;

        float objectBottomY = collision.collider.bounds.min.y;

        return objectBottomY > platformTopY;
    }

    bool IsCubeOnTopOfPlatform(Collision2D collision)
    {
        float platformTopY = transform.position.y + GetComponent<Collider2D>().bounds.size.y / 2;

        float cubeBottomY = collision.transform.position.y - collision.transform.GetComponent<G_Cube>().boxCol.bounds.size.y / 2;

        return cubeBottomY > platformTopY;
    }
}
