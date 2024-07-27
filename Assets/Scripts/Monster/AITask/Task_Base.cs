using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AITask에서 공통적으로 사용하는 함수들 모음
/// </summary>
public class Task_Base : MonoBehaviour
{
    protected void LookAt2D(Vector2 targetPosition)
    {
        Vector2 toTarget = targetPosition - (Vector2)transform.position;
        Vector2 localRight = transform.rotation * Vector3.right;  // 몬스터 입장에서의 '오른쪽'
        LR curDir;
        if(Vector2.Dot(toTarget, localRight) > 0)
        {
            curDir = LR.RIGHT;
        }
        else
        {
            curDir = LR.LEFT;
        }
        if(curDir != transform.localScale.toLR())
        {
            Flip();
        }
    }

    protected LR GetCurrentDir()
    {
        if (transform.localScale.x < 0)
        {
            return LR.LEFT;
        }
        else
        {
            return LR.RIGHT;
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
