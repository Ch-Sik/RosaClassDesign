using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    public float lifeTime = 8f;
    public float moveSpeed = 1f;

    // 기즈모 그리기용
    Vector3 endPoint;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifeTime);
        gameObject.GetComponent<Rigidbody2D>().DOMoveY(transform.position.y - moveSpeed * lifeTime, lifeTime);

        endPoint = transform.position + Vector3.down * lifeTime * moveSpeed;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if(Application.isPlaying)
        {
            Gizmos.DrawLine(transform.position, endPoint);
        }
        else
        {
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * moveSpeed * lifeTime);
        }
    }
}
