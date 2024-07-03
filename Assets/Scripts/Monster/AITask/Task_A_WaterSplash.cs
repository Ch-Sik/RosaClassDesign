using Panda;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task_A_WaterSplash : Task_A_MultipleProjectile
{
    [Task]
    protected void WaterSplash()
    {
        ExecuteAttack();
    }

    protected override void UpdateAimTarget()
    {
        aimTarget = (Vector2)muzzle.transform.position + Vector2.up;
    }
}
