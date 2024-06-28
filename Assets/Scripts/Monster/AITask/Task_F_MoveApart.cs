using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Panda;

public class Task_F_MoveApart : Task_Base
{
    [Title("컴포넌트 레퍼런스")]
    [SerializeField]
    private Blackboard blackboard;
    [SerializeField]
    private new Rigidbody2D rigidbody;

    [Title("일반 파라미터")]
    [SerializeField, Tooltip("task 유지 시간")]
    private float duration;

    [Title("움직임 관련 파라미터")]
    [SerializeField, Tooltip("수평/수직 방향 각각의 이동 속도")]
    private Vector2 moveSpeed;
    [SerializeField, Tooltip("적과 어느정도의 거리 두는 걸 목표로 움직일지")]
    private float targetDistance;

    [Title("디버그")]
    [SerializeField, ReadOnly]
    private Vector2 moveDir = Vector2.zero;
    [SerializeField, ReadOnly]
    private GameObject enemy;

    private Timer timer;

    /// 지형 감지를 사용해야 할까?
    

    // Start is called before the first frame update
    void Start()
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
    protected void MoveApart()
    {
        // Task의 첫 프레임
        if(timer == null)
        {
            TryInit();
            // 초기화 실패했다면 리턴
            if (timer == null)
                return;
        }
        // 지정된 시간만큼 task 수행했으면 Succeed하고 종료
        if(timer.duration >= duration)
        {
            Suceed();
            return;
        }
        else
        {
            // 디버그 정보 출력
            ThisTask.debugInfo = $"t: {duration - timer.duration}";
        }

        // 피격당하면 Fail로 종료
        bool isHitt;
        blackboard.TryGet(BBK.isHitt, out isHitt);
        if(isHitt)
        {
            Fail();
            return;
        }

        // 해당되는 사항 없으면 움직일 방향 계산하고 실제로 움직이기
        CalculateMoveDir();
        DoMove();
    }

    private void TryInit()
    {
        // enemy는 패턴을 시작할 때에만 가져온다.
        blackboard.TryGet(BBK.Enemy, out enemy);

        // 적이 근처에 없다면 물러날 기준이 없으므로 즉시 종료
        if(enemy == null)
        {
            Fail();
            return;
        }

        // 적이 근처에 있다면 물러나기 패턴 수행
        // 이하 초기화 파트
        timer = Timer.StartTimer();
    }

    protected void CalculateMoveDir()
    {
        // 플레이어 반대방향으로 이동방향 설정
        moveDir = (transform.position - enemy.transform.position).normalized;
    }

    protected void DoMove()
    {
        // 적이 목표 거리보다 가까이 있을 경우 뒤로 물러나기
        if(Vector2.Distance(enemy.transform.position, transform.position) < targetDistance)
        {
            rigidbody.velocity = Vector2.Scale(moveDir, moveSpeed);
            LookAt2D(enemy.transform.position);
        }
        // 그 외에는 아무것도 하지 않음 (rigidbody.drag로 인한 자연 감속)
        else
        {
            // do nothing;
        }
    }

    protected void Suceed()
    {
        moveDir = Vector2.zero;
        timer = null;
        ThisTask.Succeed();
    }

    protected void Fail()
    {
        moveDir = Vector2.zero;
        timer = null;
        ThisTask.Fail();
    }

    private void OnDrawGizmosSelected()
    {
        if (timer != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, targetDistance);
        }
    }
}
