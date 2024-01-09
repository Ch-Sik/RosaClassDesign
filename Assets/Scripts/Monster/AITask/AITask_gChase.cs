using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using System.Runtime.CompilerServices;

/// <summary>
/// 지상형 몬스터의 Chase Task를 수행하는 스크립트
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class AITask_gChase : MonoBehaviour
{
    [SerializeField]
    private Blackboard blackboard;
    [SerializeField]
    private new Rigidbody2D rigidbody;

    [SerializeField, Tooltip("어느정도까지 가까워져야 접근 완료로 판단할지")]
    private float acceptableRadius = 1.5f;
    [SerializeField, Tooltip("접근 속도")]
    private float chaseSpeed = 1f;
    [SerializeField, Tooltip("낭떠러지에 도달하면 멈출지 말지")]
    private bool considerCliff = true;

    private void Start()
    {
        if (blackboard == null)
        {
            blackboard = GetComponent<Blackboard>();
            if (blackboard == null)
                Debug.LogError($"{gameObject.name}: Blackboard를 찾을 수 없음!");
        }
        if (rigidbody == null)
        {
            rigidbody = GetComponent<Rigidbody2D>();
            if(rigidbody == null)
                Debug.LogError($"{gameObject.name}: Rigidbody2D를 찾을 수 없음!");
        }
    }

    [Task]
    private void ChaseEnemy()
    {
        // 피격당했을 때 행동 중지
        bool isHitt;
        if (blackboard.TryGet(BBK.isHitt, out isHitt) && isHitt)
        {
            ThisTask.Fail();
            StopMoving();
            return;
        }

        // 적이 탐지 범위에서 사라졌을 경우 추격 종료
        GameObject enemy;
        if (!blackboard.TryGet(BBK.Enemy, out enemy) || enemy == null)
        {
            ThisTask.Fail();
            StopMoving();
            return;
        }

        // 적과 충분히 가까워졌을 경우 역시 추격 종료
        if (Vector2.Distance(enemy.transform.position, transform.position) < acceptableRadius)
        {
            ThisTask.Succeed();
            StopMoving();
            // 마지막에 방향 확실히 설정
            if((enemy.transform.position - transform.position).toLR() != transform.localScale.toLR())
                flip();
            return;
        }

        // 벽에 막혔을 경우 행동 중지
        bool isStuckAtWall;
        blackboard.TryGet(BBK.StuckAtWall, out isStuckAtWall);
        if(isStuckAtWall)
        {
            ThisTask.Fail();
            StopMoving();
            return;
        }

        // 절벽에 이르렀을 경우 똑똑하다면 행동 중지
        if(considerCliff)
        {
            bool isStuckAtCliff;
            blackboard.TryGet(BBK.StuckAtCliff, out isStuckAtCliff);
            if(isStuckAtCliff)
            {
                ThisTask.Fail();
                StopMoving(); 
                return;
            }
        }

        // 방향 설정
        if ((enemy.transform.position.x - transform.position.x) 
            * transform.localScale.x < 0f)
        {
            flip();
        }    

        // 적 방향으로 이동
        MoveTo(enemy.transform.position.x);
    }

    private void flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y);
    }

    private void MoveTo(float xCoord)
    {
        float moveDir;
        if (xCoord < transform.position.x)
            moveDir = -1;
        else
            moveDir = 1;

        rigidbody.velocity = new Vector2(moveDir * chaseSpeed, rigidbody.velocity.y);
    }

    private void StopMoving()
    {
        // TODO: 플레이어의 넉백과 충돌하지 않는지 확인 필요
        rigidbody.velocity = Vector2.zero;
    }
}
