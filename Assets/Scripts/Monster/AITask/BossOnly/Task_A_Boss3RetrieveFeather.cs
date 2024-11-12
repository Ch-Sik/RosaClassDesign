using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

public class Task_A_Boss3RetrieveFeather : Task_A_Base
{
    public Transform retreivePos;

    private Task_A_Boss3FeatherAttack task_launchFeather;

    void Start()
    {
        task_launchFeather = GetComponent<Task_A_Boss3FeatherAttack>();
    }

    [Task]
    void RetrieveFeather()
    {
        ExecuteAttack();
    }

    protected override void OnActiveBegin()
    {
        foreach(var instance in task_launchFeather.featherInstances)
        {
            instance.GetComponent<Boss3Projectile>().RetrieveProjectile(retreivePos.position);
        }
        task_launchFeather.featherInstances.Clear();
    }
}
