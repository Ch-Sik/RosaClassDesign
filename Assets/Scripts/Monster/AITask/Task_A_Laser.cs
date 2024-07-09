using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

public class Task_A_Laser : Task_A_Base
{
    [SerializeField, Tooltip("레이저 발사될 위치")]
    protected Transform muzzle;
    [SerializeField]
    protected GameObject laserPrefab;

    [SerializeField]
    protected int damage;
    [SerializeField, ReadOnly]
    protected MonsterLaser instance = null;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(muzzle != null);
        Debug.Assert(laserPrefab != null);
    }

    [Task]
    void LaserAttack()
    {
        ExecuteAttack();
    }

    protected override void OnStartupBegin()
    {
        GameObject spawned = Instantiate(laserPrefab, muzzle.position, Quaternion.identity);
        instance = spawned.GetComponent<MonsterLaser>();

        instance.Initalize(GetCurrentDir().toVector2());
    }

    protected override void OnActiveBegin()
    {
        instance.Activate(damage);
    }

    protected override void OnRecoveryBegin()
    {
        instance.Terminate();
    }
}
