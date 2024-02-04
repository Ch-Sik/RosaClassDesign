using Panda;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task_F_Chase : Task_Base
{
    // 컴포넌트 레퍼런스
    [SerializeField]
    private Blackboard blackboard;
    [SerializeField]
    private new Rigidbody2D rigidbody;

    // 추격 관련 파라미터
    [SerializeField, Tooltip("어느정도까지 가까워져야 접근 완료로 판단할지")]
    private float acceptableRadius = 1.5f;
    [SerializeField, Tooltip("가로/세로 방향 추격 속도")]
    protected Vector2 chaseSpeed = new Vector2(1, 1);

    // 지형 감지
    [SerializeField]
    protected GameObject terrainSensor;
    [SerializeField]
    protected float terrainSensorOffset;

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
            if (rigidbody == null)
                Debug.LogError($"{gameObject.name}: Rigidbody2D를 찾을 수 없음!");
        }
    }

    [Task]
    protected void ChaseEnemy()
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
            return;
        }

        // 벽에 막혔을 경우 행동 중지
        bool isStuckAtWall;
        blackboard.TryGet(BBK.StuckAtWall, out isStuckAtWall);
        if (isStuckAtWall)
        {
            ThisTask.Fail();
            StopMoving();
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
        rigidbody.velocity = Vector2.Scale(toEnemy, chaseSpeed);
    }

    private void StopMoving()
    {
        // TODO: 플레이어의 넉백과 충돌하지 않는지 확인 필요
        rigidbody.velocity = Vector2.zero;
    }
}
