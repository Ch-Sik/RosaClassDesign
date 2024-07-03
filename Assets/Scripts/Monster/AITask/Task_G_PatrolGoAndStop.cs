using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using System;

/// <summary>
/// 지상형 몬스터의 Patrol Task를 수행하는 스크립트
/// 멈췄다 갔다 하는 버전
/// </summary>
public class Task_G_PatrolGoAndStop : Task_Base
{
    [SerializeField]
    private Blackboard blackboard;
    [SerializeField]
    private new Rigidbody2D rigidbody;

    [SerializeField]
    private float patrolSpeed = 1.0f;
    [SerializeField, Tooltip("순찰 중 앞으로 가는 시간")]
    private float patrolGoTime = 1.0f;
    [SerializeField, Tooltip("순찰 중 잠시 멈추는 시간")]
    private float patrolStopTime = 1.0f;
    [SerializeField, Tooltip("낭떠러지에 도달하면 멈출지 말지")]
    private bool considerCliff = true;

    [SerializeField]
    private bool hasPatrolRangeLimit = true;   // 순찰 범위를 수동으로 제한할 것인지
    [SerializeField, DrawIf("hasPatrolRangeLimit", true)]
    private float PatrolRangeSize = 5.0f;
    [SerializeField, DrawIf("hasPatrolRangeLimit", true)]
    private float PatrolRangeOffset = 0.0f;
    [SerializeField, ReadOnly]
    private float startXpos, destXpos;      // 한 번 와리가리 할 때 시작점과 도착점의 x좌표

    [SerializeField]
    private LR startDir = LR.RIGHT;         // 처음 시작할 때 어느쪽으로 갈 건지

    private Timer patrolTimer = null;

    private void Start()
    {
        // 컴포넌트 누락된 거 있으면 설정
        if (blackboard == null)
        {
            blackboard = GetComponent<Blackboard>();
            if (blackboard == null)
                Debug.LogError($"{gameObject.name}: Blackboard를 찾을 수 없음!");
        }
        if (rigidbody == null)
        {
            rigidbody = GetComponent<Rigidbody2D>();
            if (rigidbody == null)
                Debug.LogError($"{gameObject.name}: Rigidbody2D를 찾을 수 없음!");
        }

        // 순찰 범위 지정
        LR currentDir = GetCurrentDir();
        destXpos = transform.position.x + PatrolRangeOffset;
        destXpos += 0.5f * PatrolRangeSize * (currentDir.isLEFT() ? -1 : +1);
        startXpos = destXpos - PatrolRangeSize * (currentDir.isLEFT() ? -1 : +1);

        // 시작하자마자 SetNextDest 실행되는 것 고려하여 방향 설정
        if (GetCurrentDir() == startDir)
        {
            Flip();
        }
    }

    [Task]
    private void Patrol()
    {
        LR nowDir = GetCurrentDir();

        // 피격당했을 때 행동 중지
        bool isHitt;
        if (blackboard.TryGet(BBK.isHitt, out isHitt) && isHitt)
        {
            ThisTask.Fail();
            StopMoving();
            return;
        }

        // 적이 탐지되었을 경우 순찰 종료
        GameObject enemy;
        if (blackboard.TryGet(BBK.Enemy, out enemy) && enemy != null)
        {
            ThisTask.Fail();
            StopMoving();
            return;
        }

        // 순찰 범위가 지정되었을 때, 순찰 범위에 도달했다면 순찰 종료
        if (hasPatrolRangeLimit)
        {
            if ((nowDir.isLEFT() && transform.position.x < destXpos) ||
                (nowDir.isRIGHT() && transform.position.x > destXpos))
            {
                StopMoving();
                ThisTask.Succeed();
                return;
            }
        }

        // 벽에 막혔을 경우 행동 중지
        bool isStuckAtWall;
        blackboard.TryGet(BBK.StuckAtWall, out isStuckAtWall);
        if (isStuckAtWall)
        {
            // Succeed로 끝나야 PatrolWait를 수행함
            ThisTask.Succeed();
            StopMoving();
            return;
        }

        // 절벽에 이르렀을 경우 똑똑하다면 행동 중지
        if (considerCliff)
        {
            bool isStuckAtCliff;
            blackboard.TryGet(BBK.StuckAtCliff, out isStuckAtCliff);
            if (isStuckAtCliff)
            {
                ThisTask.Succeed();
                StopMoving();
                return;
            }
        }

        if (patrolTimer == null)
        {
            // 타이머 시작
            patrolTimer = Timer.StartTimer();
        }
        if (patrolTimer.duration < patrolGoTime)
        {
            // patrolGoTime 동안 순찰 수행
            MoveTo(destXpos);
        }
        else if (patrolTimer.duration < patrolGoTime + patrolStopTime)
        {
            // patrolStopTime 동안 휴식
            PatrolHangOn();
        }
        else
        {
            // 휴식 끝나면 다시 출발
            patrolTimer.Reset();
            MoveTo(destXpos);
        }
    }

    // 반대쪽 순찰범위 끝까지 가는 동안 중간에 잠깐 멈춰서기
    [Task]
    private void PatrolHangOn()
    {
        blackboard.Set(BBK.isMoving, false);
    }

    // 순찰 범위 끝까지 도달한 후 뒤돌아서기 전에 잠깐 멈추기
    [Task]
    private void PatrolWait()
    {
        blackboard.Set(BBK.isMoving, false);

        // 피격당한 상황일 때 Fail -> SetNextDest로 인해 뒤돌아버리는 것 방지
        bool isHitt;
        if (blackboard.TryGet(BBK.isHitt, out isHitt) && isHitt)
        {
            ThisTask.Fail();
            StopMoving();
            return;
        }
        // BT 파일 재활용을 위해 PatrolWaitOnEnd 함수를 남겨두지만
        // 이 함수는 실질적으로 아무것도 하지 않음
        ThisTask.Succeed();
    }

    [Task]
    private void SetNextDest()
    {
        if(hasPatrolRangeLimit)
        {
            // ==== 순찰 범위 제한이 있는 경우 ==== 

            // 벽이나 낭떠러지로 인해 멈춘 케이스인지 검사
            bool stuckAtWall, stuckAtCliff;
            blackboard.TryGet(BBK.StuckAtWall, out stuckAtWall);
            blackboard.TryGet(BBK.StuckAtCliff, out stuckAtCliff);

            // 벽이나 낭떠러지로 인해 멈춘 케이스: 목적지 좌표를 새로 계산
            if (stuckAtWall || stuckAtCliff)
            {
                if (GetCurrentDir().isLEFT())
                {
                    // 왼쪽으로 가고 있었다면 다음 목적지는 오른쪽
                    destXpos = transform.position.x + PatrolRangeSize;
                }
                else
                {
                    // 오른쪽으로 가고 있었다면 다음 목적지는 왼쪽
                    destXpos = transform.position.x - PatrolRangeSize;
                }
                startXpos = transform.position.x;
                // 물리 업데이트 지연으로 뒤돌고 나서도 벽/절벽 마주본 것처럼 인식하는 현상 방지
                blackboard.Set(BBK.StuckAtWall, false);
                blackboard.Set(BBK.StuckAtCliff, false);
            }
            // 목적지에 다다라서 멈춘 케이스: 좌표를 새로 계산하지 않고 바꾸기만 함
            else
            {
                // 플레이어가 간당간당하게 움직이며 SetNextDest가 반복적으로 호출되는 경우를 대비,
                // 단순히 start와 dest를 swap하지 않고 먼쪽을 골라 dest로 삼음.
                float tmpXpos1 = startXpos, tmpXpos2 = destXpos;
                if (MathF.Abs(tmpXpos1 - transform.position.x) > MathF.Abs(tmpXpos2 - transform.position.x))
                {
                    startXpos = tmpXpos2;
                    destXpos = tmpXpos1;
                }
                else
                {
                    startXpos = tmpXpos1;
                    destXpos = tmpXpos2;
                }
            }

            // 방향 설정
            if ((destXpos - transform.position.x)
                * transform.localScale.x < 0f)
            {
                Flip();
            }
        }
        else
        {
            // ==== 순찰 범위 제한이 없는 경우 ==== 

            // 목표 좌표를 +/- 무한대로 설정
            if (GetCurrentDir().isLEFT())    // 기존 가던 방향 왼쪽이라면
            {
                startXpos = float.MinValue;
                destXpos = float.MaxValue;
            }
            else
            {
                startXpos = float.MaxValue;
                destXpos = float.MinValue;
            }

            // 물리 업데이트 지연으로 뒤돌고 나서도 벽/절벽 마주본 것처럼 인식하는 현상 방지
            blackboard.Set(BBK.StuckAtWall, false);
            blackboard.Set(BBK.StuckAtCliff, false);

            // 방향 설정
            Flip();
        }

        ThisTask.Succeed();
    }

    private void MoveTo(float xCoord)
    {
        blackboard.Set(BBK.isMoving, true);

        float moveDir;
        if (xCoord < transform.position.x)
            moveDir = -1;
        else
            moveDir = 1;

        rigidbody.velocity = new Vector2(moveDir * patrolSpeed, rigidbody.velocity.y);
        // 바라보는 방향과 이동 방향이 다를 경우
        /*if (transform.localScale.x * rigidbody.velocity.x < 0)   
        {
            Flip();
        }*/
    }

    private void StopMoving()
    {
        blackboard.Set(BBK.isMoving, false);

        // TODO: 플레이어의 넉백과 충돌하지 않는지 확인 필요
        rigidbody.velocity = Vector2.zero;
    }

    private void OnDrawGizmos()
    {
        if (hasPatrolRangeLimit)
        {
            // 패트롤 범위 시각화
            Gizmos.color = Color.white;
            if (Application.isPlaying)
                Gizmos.DrawWireCube(new Vector3((startXpos + destXpos) / 2, transform.position.y, 0),
                                    new Vector3(PatrolRangeSize, 1));
            else
                Gizmos.DrawWireCube(transform.position + new Vector3(PatrolRangeOffset, 0),
                                    new Vector3(PatrolRangeSize, 1));
        }
    }
}
