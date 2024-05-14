using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

/// <summary>
/// 지면/벽/천장 등 지형의 경계선을 따라 움직이는 Patrol을 수행하는 스크립트
/// </summary>
public class Task_G_PatrolCrawl : Task_Base
{
    [SerializeField]
    private Blackboard blackboard;
    [SerializeField]
    private new Rigidbody2D rigidbody;

    [SerializeField]
    private Transform bottomCenter;
    [SerializeField]
    private Transform topCenter;

    [SerializeField]
    private float patrolSpeed = 1.0f;
    [SerializeField]
    private float rotateTime = 0.5f;
    [SerializeField, Tooltip("움직이는 방향.\n시계방향이면 RIGHT, 반시계방향이면 LEFT")]
    private LR moveDir = LR.RIGHT;


    // 몬스터의 진행 방향.
    // 지면에 있을 경우 moveDir가 가리키는 방향과 동일.
    // 천장에 붙어있을 경우 moveDir가 가리키는 방향과 정반대.
    [SerializeField, ReadOnly]
    private Vector2 forwardVector;

    private bool isDoingRotate = false;

    // Start is called before the first frame update
    void Start()
    {
        // 컴포넌트 누락된 거 있으면 설정
        if (blackboard == null)
        {
            blackboard = GetComponent<Blackboard>();
            Debug.Assert(blackboard != null, $"{gameObject.name}: Blackboard를 찾을 수 없음!");
        }
        if (rigidbody == null)
        {
            rigidbody = GetComponent<Rigidbody2D>();
            Debug.Assert(rigidbody != null, $"{gameObject.name}: Rigidbody2D를 찾을 수 없음!");
        }

        // rigidbody 설정
        rigidbody.isKinematic = true;

        // 시작하자마자 방향 설정
        forwardVector = moveDir.toVector2();
        if (GetCurrentDir() != moveDir)
        {
            Flip();
        }
    }

    [Task]
    private void Patrol()
    {
        if (isDoingRotate) return;

        // 벽에 닿았을 경우 회전
        bool isStuckAtWall;
        blackboard.TryGet(BBK.StuckAtWall, out isStuckAtWall);
        if(isStuckAtWall)
        {
            StopMove();
            Rotate(topCenter.position, moveDir.isRIGHT() ? 90 : -90);
            return;
        }

        // 낭떠러지에 닿았을 경우 회전
        bool isStuckAtCliff;
        blackboard.TryGet(BBK.StuckAtCliff, out isStuckAtCliff);
        if (isStuckAtCliff)
        {
            StopMove();
            Rotate(bottomCenter.position, moveDir.isRIGHT() ? -90 : 90);
            return;
        }
        
        // 앞에 걸리는 거 없다면 전진
        Move();
    }

    public float currentAngle;
    public float targetAngle;

    private void Rotate(Vector3 pivot, float angle)
    {
        isDoingRotate = true;
        currentAngle = transform.localRotation.eulerAngles.z;
        targetAngle = currentAngle + angle;

        float rotatePerSecond = angle / rotateTime;
        

        StartCoroutine(DoRotate90());

        IEnumerator DoRotate90()
        {
            Debug.LogWarning("회전 시작");
            float sumAngle = 0;
            //while((angle < 0 && CompareAngle(currentAngle, targetAngle) > 0)     // 시계방향 회전
            //    || (angle > 0 && CompareAngle(currentAngle, targetAngle) < 0))                   // 반시계방향 회전
            while(true)
            {
                float rotatePerDeltaTime = rotatePerSecond * Time.deltaTime;
                transform.RotateAround(pivot, Vector3.forward, rotatePerDeltaTime);
                sumAngle += rotatePerDeltaTime;
                if (Mathf.Abs(sumAngle) > 90)
                    break;
                else
                    yield return 0;
            }
            Debug.LogWarning("while loop end, set angle to targetAngle");
            transform.rotation = Quaternion.Euler(0, 0, targetAngle);
            forwardVector = moveDir.isRIGHT()? transform.right : -transform.right;
            isDoingRotate = false;
            yield return 0;
        }
    }

    private float CompareAngle(float a, float b)
    {
        if (a < 0) a += 360;
        if (b < 0) b += 360;
        return a - b;
    }

    private void Move()
    {
        rigidbody.velocity = forwardVector * patrolSpeed;
    }

    private void StopMove()
    {
        rigidbody.velocity = Vector2.zero;
    }
}
