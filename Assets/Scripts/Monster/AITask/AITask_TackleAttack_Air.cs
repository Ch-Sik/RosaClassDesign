using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using Sirenix.OdinInspector;

// TODO: 이거 AITask_TackleAttack 복붙한거라 공중 몬스터에 맞게 전면 수정해야됨
public class AITask_TackleAttack_Air : AITask_TackleAttack
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
        lookAt2D(enemy.transform.position);
        // 지형 감지 센서 위치 설정
        SetChildObjectPos(terrainSensor, tackleDir * terrainSensorOffset);
    }

    protected override void DoTackle()
    {
        rigidbody.velocity = tackleDir * tackleSpeed;
    }
}
