using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using Sirenix.OdinInspector;
using DG.Tweening;

// Task_GA_Tackle(지상 몬스터 태클 공격 스크립트)에서 공격 방향 계산하는 거랑 공격 시의 velocity 설정하는 거만 override함.
public class Task_FA_Tackle : Task_GA_Tackle
{
    [Title("지형 감지 관련")]
    [SerializeField]
    protected GameObject terrainSensor;
    [SerializeField]
    protected float terrainSensorOffset;

    [Title("디버그")]
    [SerializeField, ReadOnly]
    protected GameObject enemy;

    protected override void CalculateAttackDirection()
    {
        // 디버그
        Debug.Log($"계산 전 스케일: {transform.localScale}\n" +
            $"terrainSensor: {terrainSensor.transform.localPosition}");


        if (!blackboard.TryGet(BBK.Enemy, out enemy) || enemy == null)
        {
            Debug.LogError($"{gameObject.name}: PrepareAttack에서 적을 찾을 수 없음!");
        }
        tackleDir = (enemy.transform.position - transform.position).normalized;

        // 공격 방향에 따라 좌우 반전하기
        LookAt2D(enemy.transform.position);
        // 지형 감지 센서 위치 설정
        terrainSensor.transform.localPosition = Vector2.right * terrainSensorOffset;

        // 기울기 계산
        Vector2 vectorToEnemy = enemy.transform.position - transform.position;
        // 좌우 반전 고려 1
        if (transform.localScale.x < 0)
            vectorToEnemy.x = -vectorToEnemy.x;
        float radianToEnemy = Mathf.Atan2(vectorToEnemy.y, vectorToEnemy.x);
        // 좌우 반전 고려 2
        if (transform.localScale.x < 0)
            radianToEnemy = -radianToEnemy;
        transform.DORotate(new Vector3(0, 0, radianToEnemy * Mathf.Rad2Deg), 0.2f);
        Debug.Log($"Shoebill Angle: {radianToEnemy}");

        // 디버그
        Debug.Log($"계산 후 스케일: {transform.localScale}\n" +
            $"terrainSensor: {terrainSensor.transform.localPosition}");
    }

    protected override void DoTackle()
    {
        rigidbody.velocity = tackleDir * tackleSpeed;
    }

    protected override void Succeed()
    {
        base.Succeed();
        transform.rotation = Quaternion.identity;
        Debug.Log("Reset Shoebill Angle");
    }

    protected override void Fail()
    {
        base.Fail();
        transform.rotation = Quaternion.identity;
        Debug.Log("Reset Shoebill Angle");
    }
}
