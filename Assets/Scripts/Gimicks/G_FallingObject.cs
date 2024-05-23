using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class G_FallingObject : MonoBehaviour
{
    public float fallingLength = 3.0f;
    public float fallingTime = 3.0f;
    public float beforeFallingDelay = 1.0f;
    public float afterFallingDelay = 1.0f;
    public Vector2 sizeOfFallingObject = Vector2.one;
    public GameObject platform;

    private float fixedFallingLength;

    private void Start()
    {
        fixedFallingLength = fallingLength - sizeOfFallingObject.y / 2;
        Fall();
    }

    public void Fall()
    {
        Sequence falling = DOTween.Sequence()
        .AppendInterval(beforeFallingDelay)
        .Append(platform.transform.DOLocalMoveY(-1 * fixedFallingLength, fallingTime).SetRelative(true))
        .AppendInterval(afterFallingDelay)
        .Append(platform.transform.DOLocalMoveY(fixedFallingLength, fallingTime).SetRelative(true))
        .SetLoops(-1);
    }

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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x,
                                                        transform.position.y - fallingLength));
        Gizmos.DrawWireCube(new Vector3(transform.position.x,
                                        transform.position.y - fallingLength),
                            sizeOfFallingObject);
    }
}
