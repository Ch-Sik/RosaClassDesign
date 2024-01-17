using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AITask에서 공통적으로 사용하는 함수들 모음
/// </summary>
public class AITask_Base : MonoBehaviour
{
    protected void lookAt2D(Vector2 targetPosition)
    {
        Vector2 toTarget = targetPosition - (Vector2)transform.position;
        if(toTarget.toLR() != transform.localScale.toLR())
        {
            Flip();
        }
    }

    protected void Flip()
    {
        transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
    }

    protected void SetChildObjectPos(GameObject child, Vector2 position)
    {
        position.x *= transform.localScale.x < 0 ? -1 : 1;
        child.transform.localPosition = position;
    }
}
