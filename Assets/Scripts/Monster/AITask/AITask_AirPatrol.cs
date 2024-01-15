using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using Sirenix.OdinInspector;
using Rito;
using System;

public class AITask_AirPatrol : MonoBehaviour
{
    // 컴포넌트 레퍼런스
    [SerializeField]
    protected Blackboard blackboard;
    [SerializeField]
    private new Rigidbody2D rigidbody;

    // 순찰 범위 관련
    [SerializeField, Tooltip("순찰 범위를 시작점 근처로 제한할 것인지 여부")]
    private bool moveOnlyNearby;

    // 순찰 목적지 선정 관련
    [SerializeField, Tooltip("다음 목적지 선호도 시각화")]
    private bool drawDestDecisionVector;
    [SerializeField, Tooltip("다음 목적지 선정 시 선호도에 랜덤 요소 반영")]
    private bool useRandomToPreference;
    [ShowIf("useRandomToPreference")]
    [SerializeField, Tooltip("랜덤 요소 사용 시 가중치 지수")]
    private float randomPow;
    [SerializeField, Tooltip("다음 목적지까지의 최소 거리")]
    private float minPatrolDist = 1f;
    [SerializeField, Tooltip("다음 목적지까지의 최대 거리")]
    private float maxPatrolDist = 4f;

    [Space(10)]
    [SerializeField, Tooltip("순찰 사이사이 멈추는 시간 길이")]
    private float patrolWaitTime;
    [SerializeField, Tooltip("순찰 시의 이동 속도")]
    private float patrolSpeed;

    [BoxGroup("AiCoefficient")]
    [SerializeField]
    private float aiCoef_toCenter = 1;
    [BoxGroup("AiCoefficient")]
    [SerializeField]
    private float aiCoef_awayFromWall = 0.3f;
    [BoxGroup("AiCoefficient")]
    [SerializeField]
    private float aiCoef_sidestep = 0.6f;

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
    [SerializeField]
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
        if(Vector2.Distance((Vector2)transform.position, nextDest) < 0.1f)
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
        bool isStuckAtWall;
        blackboard.TryGet(BBK.StuckAtWall, out isStuckAtWall);
        if (isStuckAtWall)
        {
            // 수동으로 목적지 재설정
            int index = 0;
            for (int i = 1; i < dirVector.Length; i++)
            {
                if (dirPreference[i] > dirPreference[index])
                    index = i;
            }
            nextDest = (Vector2)transform.position + dirVector[index] 
                        * UnityEngine.Random.Range(minPatrolDist, maxPatrolDist);
        }

        Vector2 moveDir = (dest - (Vector2)transform.position);
        if (moveDir.sqrMagnitude > 1)
            moveDir.Normalize();

        // Lerp 사용
        rigidbody.velocity = rigidbody.velocity * 0.9f + moveDir * patrolSpeed * 0.1f;
    }

    [Task]
    private void PatrolWait()
    {
        if (waitTimer == null)
        {
            // 타이머 시작
            waitTimer = Timer.StartTimer();
        }
        else if (waitTimer.duration >= patrolWaitTime)
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
        UpdateNextDest();
        ThisTask.Succeed();
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
                    randomPicker.Add(i, Mathf.Pow(dirPreference[i], randomPow));
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
    }


    // 순찰할 다음 위치 지정은 원래 매 프레임 수행할 게 아니라 SetNextDest에서만 수행하지만
    // 연구중에는 시각화를 위해 Update에서 수행하도록 하고 SetNextDest는 결과를 가져가기만 함.
    private void Update()
    {
        // 선호도 초기화
        {
            for(int i = 0; i < dirVector.Length; i++)
            {
                dirPreference[i] = 0.5f;        // 기본 선호도 0.5f
            }
        }
        // 순찰 범위가 제한되어 있다면 센터에서 멀어졌을 때 중심 쪽으로 가도록 선호도 증가
        if (moveOnlyNearby)
        {
            // 멀어졌을 때 선호도 크도록 normalize 안함.
            Vector2 toPatrolCenter = patrolAreaCenter - (Vector2)transform.position;
            for(int i=0; i<dirVector.Length; i++)
            {
                dirPreference[i] += (Vector2.Dot(dirVector[i], toPatrolCenter) - 0.2f) * aiCoef_toCenter;
                // 목표 방향과 수직인 요소 추가
                dirPreference[i] += (1 - Mathf.Abs(Vector2.Dot(dirVector[i], toPatrolCenter))) * aiCoef_sidestep;
            }
        }
        else
        {
            for(int i=0; i<dirVector.Length; i++)
            {
                dirPreference[i] += 1;
            }
        }
        // 벽과 가까워졌을 때 벽과 가까운 방향일수록 선호도 감소
        {
            for(int i=0; i<dirVector.Length; i++)
            {
                RaycastHit2D rayhit = Physics2D.Raycast(
                            transform.position, dirVector[i], 5f,       // 벽과의 거리 최대 5까지 탐지
                            LayerMask.GetMask("Ground"));
                if(rayhit.collider != null)
                {
                    dirPreference[i] -= 1 / rayhit.distance * aiCoef_awayFromWall;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(moveOnlyNearby)
        {
            if (Application.isPlaying)
                Gizmos.DrawWireSphere(patrolAreaCenter, 1.0f);
            else
                Gizmos.DrawWireSphere(transform.position, 1.0f);
        }

        if(drawDestDecisionVector)
        {
            for(int i=0;i<dirVector.Length;i++)
            {
                if (dirPreference[i] > 0)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine((Vector2)transform.position + dirVector[i] * 0.5f,
                        (Vector2)transform.position + dirVector[i] * (0.5f + dirPreference[i] * 0.2f));
                }
                else
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine((Vector2)transform.position + dirVector[i] * 0.5f,
                        (Vector2)transform.position + dirVector[i] * (0.5f - dirPreference[i] * 0.2f));
                }
            }
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(nextDest, 0.4f);
    }
}
