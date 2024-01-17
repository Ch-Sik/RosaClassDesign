using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using Sirenix.OdinInspector;
using Rito;
using System;
using UnityEditor.Tilemaps;
using static UnityEngine.GraphicsBuffer;

public class AITask_AirPatrol : AITask_Base
{
    [Title("컴포넌트 레퍼런스")]// 컴포넌트 레퍼런스
    [SerializeField]
    protected Blackboard blackboard;
    [SerializeField]
    private new Rigidbody2D rigidbody;

    [Title("순찰 목적지 선정 옵션")]// 순찰 목적지 선정 관련
    [SerializeField, Tooltip("다음 방향 선정 시 최고 선호도대신 선호도 가중치를 사용하는 랜덤 사용")]
    private bool useRandomToPreference;
    [ShowIf("useRandomToPreference")]
    [SerializeField, Range(0.3f, 3f), Tooltip("랜덤 요소 사용 시 가중치 지수")]
    private float randomWeightExp;
    [SerializeField, Tooltip("다음 목적지까지의 최소 거리")]
    private float minPatrolDist = 1f;
    [SerializeField, Tooltip("다음 목적지까지의 최대 거리")]
    private float maxPatrolDist = 4f;

    [FoldoutGroup("AI 계수")]
    [SerializeField, Tooltip("순찰 범위의 중심으로 가려는 성질")]
    private float aiCoef_toCenter = 1;
    [FoldoutGroup("AI 계수")]
    [SerializeField, Tooltip("벽에서 먼 곳으로 향하려는 성질")]
    private float aiCoef_awayFromWall = 0.3f;
    [FoldoutGroup("AI 계수")]
    [SerializeField, Tooltip("순찰 범위의 중심을 중점으로 하는 원형 궤도로 움직이려는 성질")]
    private float aiCoef_sidestep = 0.6f;

    [Title("타이밍과 속도")]
    [SerializeField, Tooltip("순찰 사이사이 멈추는 시간 길이")]
    private float patrolStopTime;
    [SerializeField, Tooltip("순찰 시의 이동 속도")]
    private float patrolSpeed;

    [Title("지형 감지 관련")]
    [SerializeField, Tooltip("지형 감지 콜라이더가 달려있는 오브젝트")]
    private GameObject terrainSensor;
    [SerializeField, Tooltip("지형 감지 콜라이더를 몬스터 중심으로부터 얼만큼 떨어뜨릴건지")]
    private float terrainSensorOffset;

    [FoldoutGroup("시각화 옵션")]
    [SerializeField, Tooltip("순찰 중심점 표시")]
    private bool drawPatrolCenter;
    [FoldoutGroup("시각화 옵션")]
    [SerializeField, Tooltip("다음 방향 선호도 표시")]
    private bool drawDestDecision;
    [FoldoutGroup("시각화 옵션")]
    [SerializeField, Tooltip("현재 순찰 포인트 표시")]
    private bool drawDestPoint;

    // 순찰 범위(원형)의 중심. Start 시점에 이 몬스터가 위치한 곳을 중심으로 삼음.
    private Vector2 patrolAreaCenter;
    // 16방향으로 움직일 때, 어느 방향으로 갈지 계산하면서 사용하는 방향 기준 벡터. (0,1)부터 시계 방향.
    private Vector2[] dirVector = { Vector2.up,                                     // 정북
                                    (2 * Vector2.up + Vector2.right).normalized,    // 북북동
                                    (Vector2.up + 2 * Vector2.right).normalized,    // 동북동
                                    Vector2.right,                                  // 정동
                                    (2 * Vector2.right + Vector2.down).normalized,  // 동남동
                                    (Vector2.right + 2 * Vector2.down).normalized,  // 남남동
                                    Vector2.down,                                   // 정남
                                    (2 * Vector2.down + Vector2.left).normalized,   // 남남서
                                    (Vector2.down + 2 * Vector2.left).normalized,   // 서남서
                                    Vector2.left,                                   // 정서
                                    (2 * Vector2.left + Vector2.up).normalized,     // 서북서
                                    (Vector2.left + 2 * Vector2.up).normalized };   // 북북서
    // 16방향으로 움직일 때, 어느 방향으로 갈지 계산한 선호도.
    private float[] dirPreference = new float[16];
    private Vector2 nextDest;
    private Timer waitTimer = null;
    private WeightedRandomPicker<int> randomPicker;

    private void Start()
    {
        patrolAreaCenter = transform.position;
        randomPicker = new Rito.WeightedRandomPicker<int>();
    }

    [Task]
    private void Patrol()
    {
        bool isStuckAtWall;
        blackboard.TryGet(BBK.StuckAtWall, out isStuckAtWall);

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

        // 벽에 가로막혔을 때 행동 중지
        if (isStuckAtWall)
        {
            ThisTask.Succeed();
        }
        else if (Vector2.Distance((Vector2)transform.position, nextDest) < 0.1f)
        {
            ThisTask.Succeed();
        }
        else
        {
            MoveTo(nextDest);
        }
    }

    private void MoveTo(Vector2 dest)
    {
        Vector2 moveDir = (dest - (Vector2)transform.position);
        if (moveDir.sqrMagnitude > 1)
            moveDir.Normalize();

        // 이동 속도에 Lerp 사용
        rigidbody.velocity = rigidbody.velocity * 0.9f + moveDir * patrolSpeed * 0.1f;
    }

    private void StopMoving()
    {
        rigidbody.velocity = Vector2.zero;
    }

    [Task]
    private void PatrolWait()
    {
        if (waitTimer == null)
        {
            // 타이머 시작
            waitTimer = Timer.StartTimer();
        }
        else if (waitTimer.duration >= patrolStopTime)
        {
            // 타이머 시간 다 되었으면 PatrolWait 종료
            waitTimer = null;
            ThisTask.Succeed();
        }
        // 감속
        rigidbody.velocity = rigidbody.velocity * 0.3f;
    }

    [Task]
    private void SetNextDest()
    {
        UpdatePreference();
        UpdateNextDest();
        MoveTerrainCheckCollider();
        ThisTask.Succeed();
    }

    private void UpdatePreference()
    {
        // 선호도 초기화
        {
            for (int i = 0; i < dirVector.Length; i++)
            {
                dirPreference[i] = 0.5f;        // 기본 선호도 0.5f
            }
        }
        // 센터에서 멀어졌을 때 중심 쪽으로 가도록 선호도 증가
        Vector2 toPatrolCenter = patrolAreaCenter - (Vector2)transform.position;         // 멀어졌을 때 선호도 크도록 normalize 안함.
        for (int i = 0; i < dirVector.Length; i++)
        {
            dirPreference[i] += (Vector2.Dot(dirVector[i], toPatrolCenter) - 0.2f) * aiCoef_toCenter;
            // 목표 방향과 수직인 요소 추가
            dirPreference[i] += (1 - Mathf.Abs(Vector2.Dot(dirVector[i], toPatrolCenter))) * aiCoef_sidestep;
        }
        // 벽과 가까워졌을 때 벽과 가까운 방향일수록 선호도 감소
        {
            for (int i = 0; i < dirVector.Length; i++)
            {
                RaycastHit2D rayhit = Physics2D.Raycast(
                            transform.position, dirVector[i], 5f,       // 벽과의 거리 최대 5까지 탐지
                            LayerMask.GetMask("Ground"));
                if (rayhit.collider != null)
                {
                    dirPreference[i] -= 1 / rayhit.distance * aiCoef_awayFromWall;
                }
            }
        }
    }

    private void UpdateNextDest()
    {
        int index = 0;

        if (useRandomToPreference) // 랜덤 요소 사용시 선호도를 가중치로 사용하여 랜덤 픽.
        {
            randomPicker.Clear();
            for (int i = 0; i < dirVector.Length; i++)
            {
                if (dirPreference[i] > 0)
                    randomPicker.Add(i, Mathf.Pow(dirPreference[i], randomWeightExp));    // 가중치 높은 게 더 잘 선택되도록 pow
            }
            index = randomPicker.GetRandomPick();
        }
        else                        // 랜덤 요소 미사용시 최대 선호도 방향을 픽
        {
            // List.IndexOf가 고장나서 수동으로 최대값 구함
            for (int i = 1; i < dirVector.Length; i++)
            {
                if (dirPreference[i] > dirPreference[index])
                    index = i;
            }
        }

        nextDest = (Vector2)transform.position + dirVector[index] * UnityEngine.Random.Range(minPatrolDist, maxPatrolDist);

        // 필요하다면 스프라이트 좌우반전
        lookAt2D(nextDest);
    }

    private void MoveTerrainCheckCollider()
    {
        Vector2 toNextDest = nextDest - (Vector2)transform.position;
        SetChildObjectPos(terrainSensor, toNextDest.normalized * terrainSensorOffset);
        blackboard.Set(BBK.StuckAtWall, false);     // 벽에 막혔음 플래그 강제 초기화
    }

    private void OnDrawGizmosSelected()
    {
        if (drawPatrolCenter)               // 순찰 중심점 표시
        {
            if (Application.isPlaying)
                Gizmos.DrawWireSphere(patrolAreaCenter, 0.5f);
            else
                Gizmos.DrawWireSphere(transform.position, 0.5f);
        }

        if(drawDestDecision)          // 다음 방향 선호도 표시
        {
            for(int i=0;i<dirVector.Length;i++)
            {
                if (dirPreference[i] > 0)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine((Vector2)transform.position + dirVector[i] * 0.5f,
                        (Vector2)transform.position + dirVector[i] 
                            * (0.5f + Mathf.Pow(dirPreference[i] * 0.2f, randomWeightExp)));
                }
                else
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine((Vector2)transform.position + dirVector[i] * 0.5f,
                        (Vector2)transform.position + dirVector[i]
                            * (0.5f - Mathf.Pow(dirPreference[i] * 0.2f, randomWeightExp)));
                }
            }
        }

        if(drawDestPoint)                    // 현재 순찰 목적지 표시
        { 
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(nextDest, 0.4f);
        }
    }
}