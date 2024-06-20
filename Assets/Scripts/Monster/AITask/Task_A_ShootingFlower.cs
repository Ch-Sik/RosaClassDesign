using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task_A_ShootingFlower : Task_A_SimpleProjectile
{
    protected override void OnRecoveryLast()
    {
        // '적' 바라보기
        GameObject enemy;
        if (blackboard.TryGet(BBK.Enemy, out enemy) && enemy != null)
        {
            enemyPosition = enemy.transform.position;
            LookAt2D(enemyPosition);
        }
    }
}
