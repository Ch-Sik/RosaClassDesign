using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using UnityEditor.Experimental.GraphView;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;

public class Attack_TackleAttack : MonoBehaviour
{
    [SerializeField]
    protected Blackboard blackboard;
    [SerializeField]
    private new Rigidbody2D rigidbody;

    [SerializeField, Tooltip("돌진 속도")]
    private float tackleSpeed;
    [SerializeField, Tooltip("돌진 준비 시간(선딜레이)")]
    private float prepareDuration;
    [SerializeField, Tooltip("돌진 유지 시간")]
    private float tackleDuration;
    [SerializeField, Tooltip("돌진 후 대기 시간(후딜레이)")]
    private float recoveryDuration;
    [SerializeField, Tooltip("돌진 도중에 슈퍼아머")]
    private bool superArmourOnTackle;

    private Timer prepareTimer = null;
    private Timer tackleTimer = null;
    private Timer recoveryTimer = null;
    private LR tackleDir;


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
    private void Attack()
    {
        // 피격 정보 가져오기
        bool isHitt;
        blackboard.TryGet(BBK.isHitt, out isHitt);
        if (prepareTimer == null || prepareTimer.duration < prepareDuration)
        {
            // 피격 시 행동 중지
            if (isHitt)
            {
                Fail();
                return;
            }
            PrepareAttack();
        }
        else if (tackleTimer == null || tackleTimer.duration < tackleDuration)
        {
            // 슈퍼아머가 아니면서 피격 시 행동 중지
            if (!superArmourOnTackle && isHitt)
            {
                Fail();
                return;
            }
            DoAttack();
        }
        else if (recoveryTimer == null || recoveryTimer.duration < recoveryDuration)
            AttackRecovery();
        else
            Succeed();
    }

    private void PrepareAttack()
    {
        // 발구르면서 돌진 준비하는 시간
        if (prepareTimer == null)
        {
            // 방향 계산
            GameObject enemy;
            if (!blackboard.TryGet(BBK.Enemy, out enemy) || enemy == null)
            {
                Debug.LogError($"{gameObject.name}: PrepareAttack에서 적을 찾을 수 없음!");
            }
            tackleDir = (enemy.transform.position.x - transform.position.x) < 0 ? LR.LEFT : LR.RIGHT;

            prepareTimer = Timer.StartTimer();
            return;
        }

        ThisTask.debugInfo = $"선딜: {prepareTimer.duration}";
    }

    private void DoAttack()
    {
        if (tackleTimer == null)
        {
            tackleTimer = Timer.StartTimer();
        }
        // 디버그 출력
        ThisTask.debugInfo = tackleTimer.duration.ToString();

        // 실제 돌진 수행
        Vector2 velocity = tackleDir.toVector2() * tackleSpeed;
        velocity.y = rigidbody.velocity.y;
        rigidbody.velocity = velocity;

        ThisTask.debugInfo = $"돌진: {tackleTimer.duration}";
    }

    private void AttackRecovery()
    {
        if(recoveryTimer == null)
        {
            recoveryTimer = Timer.StartTimer();
        }
        /*
            // 움직임 강제 멈춤
            tmpVector = rigidbody.velocity;
            tmpVector.x = 0f;
            rigidbody.velocity = tmpVector;
        */
        ThisTask.debugInfo = $"후딜: {recoveryTimer.duration}";
    }

    private bool isTargetBehind(GameObject enemy)
    {
        float deltaX = enemy.transform.position.x - transform.position.x;
        if(tackleDir.isLEFT()) 
            return deltaX > 0;
        else 
            return deltaX < 0;
    }

    // Task 성공/실패로 인해 종료 시 Timer 값 정리를 위해 간단히 추상화
    private void Succeed()
    {
        prepareTimer = null;
        tackleTimer = null;
        recoveryTimer = null;
        ThisTask.Succeed();
    }

    private void Fail()
    {
        prepareTimer = null;
        tackleTimer = null;
        recoveryTimer = null;
        ThisTask.Fail();
    }
}
