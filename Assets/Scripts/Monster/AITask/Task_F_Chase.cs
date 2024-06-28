using Panda;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class Task_F_Chase : Task_Base
{
    [Title("컴포넌트 레퍼런스")] 
    [SerializeField]
    private Blackboard blackboard;
    [SerializeField]
    private new Rigidbody2D rigidbody;

    [Title("추격 관련 파라미터")]
    [SerializeField, Tooltip("어느정도까지 가까워져야 접근 완료로 판단할지")]
    private float acceptableRadius = 1.5f;
    [SerializeField, Tooltip("가로/세로 방향 추격 속도")]
    protected Vector2 chaseSpeed = new Vector2(1, 1);

    [Title("종료시의 각도 제한")]
    [SerializeField, Tooltip("각도 제한 사용")]
    protected bool useFinishAngleRestriction;
    [SerializeField, ShowIf("useFinishAngleRestriction"), Range(0, 180), Tooltip("추격 종료시에 각도 제한 (최소). 0이 아래, 180이 위")]
    protected float minAngle;
    [SerializeField, ShowIf("useFinishAngleRestriction"), Range(0, 180), Tooltip("추격 종료시에 각도 제한 (최대). 0이 아래, 180이 위")]
    protected float maxAngle;
    [SerializeField, ShowIf("useFinishAngleRestriction"), Tooltip("각도 제한을 해소하기 위한 움직임의 비중")]
    protected float restrictionMoveWeight;

    [Title("지형 감지")]
    [SerializeField]
    protected GameObject terrainSensor;
    [SerializeField]
    protected float terrainSensorOffset;

    [Title("디버그 정보")]
    [SerializeField, ReadOnly]
    protected float toEnemyDistance;
    [SerializeField, ReadOnly]
    protected Vector2 toEnemyVector;
    [SerializeField, ReadOnly]
    protected Vector2 moveVector;

    // 시각화용
    protected bool drawGizmos;


    private void Start()
    {
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
    }

    [Task]
    protected void ChaseEnemy()
    {
        // 피격당했을 때 행동 중지
        bool isHitt;
        if (blackboard.TryGet(BBK.isHitt, out isHitt) && isHitt)
        {
            Fail();
            return;
        }

        // 적이 탐지 범위에서 사라졌을 경우 추격 종료
        GameObject enemy;
        if (!blackboard.TryGet(BBK.Enemy, out enemy) || enemy == null)
        {
            Fail();
            return;
        }
        // 디버그 출력
        toEnemyVector = enemy.transform.position - transform.position;
        toEnemyDistance = Vector2.Distance(enemy.transform.position, transform.position);
        drawGizmos = true;

        // 적과 충분히 가까워졌을 경우
        if (Vector2.Distance(enemy.transform.position, transform.position) < acceptableRadius)
        {
            // 각도 제한 사용한다면 각도 제한 검사
            if(useFinishAngleRestriction)
            {
                Vector2 toEnemyVector = enemy.transform.position - transform.position;
                float toEnemyAngle = Vector2Float180(toEnemyVector);
                Debug.Log($"toEnemyAngle: {toEnemyAngle}");
                // 각도 제한 통과하였다면 추격 종료
                if(toEnemyAngle > minAngle && toEnemyAngle < maxAngle)
                {
                    Succeed();
                    return;
                }
            }
            // 각도 제한 사용 안한다면 즉시 추격 종료
            else
            {
                Succeed();
                return;
            }
        }

        // 벽에 막혔을 경우 행동 중지
        bool isStuckAtWall;
        blackboard.TryGet(BBK.StuckAtWall, out isStuckAtWall);
        if (isStuckAtWall)
        {
            Fail();
            return;
        }

        // 스프라이트 방향 설정
        LookAt2D(enemy.transform.position);

        // 지형 센서 이동
        Vector2 toTarget = (Vector2)(enemy.transform.position - transform.position);
        SetChildObjectPos(terrainSensor, toTarget.normalized * terrainSensorOffset);

        // 적 방향으로 이동
        MoveTo(enemy);
    }

    private void MoveTo(GameObject enemy)
    {
        Vector2 toEnemy = (enemy.transform.position - transform.position).normalized;
        Vector2 moveDir;
        // 각도 제한 사용할경우, toEnemy와 verticalDir 혼합
        if(useFinishAngleRestriction)
        {
            // 각도 제한 해소하기 위한 방향 계산
            float toEnemyDegree = Vector2Float180(toEnemy);
            if (toEnemyDegree < minAngle)
            {
                moveDir = Vector2.Lerp(Vector2.down, toEnemy, restrictionMoveWeight);
            }
            else if (toEnemyDegree > maxAngle)
            {
                moveDir = Vector2.Lerp(Vector2.up, toEnemy, restrictionMoveWeight);
            }
            // 이미 각도 범위에 들어와있다면 toEnemy 그대로 사용
            else
            {
                moveDir = toEnemy;
            }
        }
        // 각도 제한 사용 안할 경우, 그냥 toEnemy 사용
        else
        {
            moveDir = toEnemy;
        }

        moveVector = Vector2.Scale(moveDir, chaseSpeed);   // 디버그용 출력
        rigidbody.velocity = Vector2.Scale(moveDir, chaseSpeed);
    }

    private void StopMoving()
    {
        // TODO: 플레이어의 넉백과 충돌하지 않는지 확인 필요
        rigidbody.velocity = Vector2.zero;
    }

    private float Vector2Float180(Vector2 input)
    {
        return Mathf.Atan2(input.y, Mathf.Abs(input.x)) * Mathf.Rad2Deg + 90.0f;
    }

    protected void Succeed()
    {
        ThisTask.Succeed();
        StopMoving();
        drawGizmos = false;
    }

    protected void Fail()
    {
        ThisTask.Fail();
        StopMoving();
        drawGizmos = false;
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, acceptableRadius);
        if(useFinishAngleRestriction)
        {
            float minRad = (minAngle - 90) * Mathf.Deg2Rad;
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(Mathf.Cos(minRad), Mathf.Sin(minRad)) * 2);
            float maxRad = (maxAngle - 90) * Mathf.Deg2Rad;
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(Mathf.Cos(maxRad), Mathf.Sin(maxRad)) * 2);
            Vector2 toEnemy = new Vector2(Mathf.Abs(toEnemyVector.x), toEnemyVector.y).normalized;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)toEnemy * 2);
        }
    }
}
