using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using Sirenix.OdinInspector;

// Task_GA_Tackle(지상 몬스터 태클 공격 스크립트)에서 공격 방향 계산하는 거랑 공격 시의 velocity 설정하는 거만 override함.
public class Task_FA_Tackle : Task_GA_Tackle
{
    [Title("지형 감지 관련")]
    [SerializeField]
    protected GameObject terrainSensor;
    [SerializeField]
    protected float terrainSensorOffset;

    protected override void CalculateAttackDirection()
    {
        GameObject enemy;
        if (!blackboard.TryGet(BBK.Enemy, out enemy) || enemy == null)
        {
            Debug.LogError($"{gameObject.name}: PrepareAttack에서 적을 찾을 수 없음!");
        }
        tackleDir = (enemy.transform.position - transform.position).normalized;

        // 공격 방향에 따라 좌우 반전하기
        LookAt2D(enemy.transform.position);
        // 지형 감지 센서 위치 설정
        SetChildObjectPos(terrainSensor, tackleDir * terrainSensorOffset);
    }

    protected override void DoTackle()
    {
        rigidbody.velocity = tackleDir * tackleSpeed;
    }
}
