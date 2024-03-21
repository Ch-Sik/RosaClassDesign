using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class MovePlatform : LeverGear
{
    [Header("플랫폼 설정")]
    public List<Transform> points;
    public float speed = 2f;
    public int currentIndex = 0;

    public bool isRepeat = true;
    public bool isActivateWhenStart = true;
    public int moveAmount = 1;

    private bool isMoving = false;
    private Tween moveTween;

    void Start()
    {
        if (isActivateWhenStart)
        {
            MoveToNextPoint();
        }
    }

    void MoveToNextPoint()
    {
        if (points.Count == 0) return;
        if (isMoving) return;
        isMoving = true;
        currentIndex = (currentIndex + moveAmount) % points.Count;
        if (currentIndex < 0) currentIndex = points.Count + currentIndex;
        Transform targetPoint = points[currentIndex];

        float duration = Vector3.Distance(transform.position, targetPoint.position) / speed;
        moveTween = transform.DOMove(targetPoint.position, duration).SetEase(Ease.Linear).OnComplete(OnMoveComplete);
    }

    void OnMoveComplete()
    {
        
        moveAmount = 1;
        isMoving = false;
        if (isRepeat)
        {
            MoveToNextPoint();
        }
    }

    public override void Activate()
    {
        Debug.Log("레버 작동됨");
        MoveToNextPoint();
    }

    public override void Activate(int value)
    {
        Debug.Log("레버 작동됨");
        moveAmount = value;
        MoveToNextPoint();
    }

    public void HandleChildTriggerEnter(Collision2D other)
    {
        other.transform.SetParent(transform);
    }

    public void HandleChildTriggerExit(Collision2D other)
    {
        if(Application.isPlaying)
        {
            if (other.transform.parent == transform)
            {
                other.transform.SetParent(null);
            }
        }
    }
}