using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

public class Task_A_LastBossEnergyBall : Task_A_Base
{
    [SerializeField] GameObject prefab;
    [SerializeField] float projectileSpeed;

    GameObject instance;
    Task_CountCheck counter;

    private void Start()
    {
        counter = GetComponent<Task_CountCheck>();
    }

    [Task]
    void EnergyBallAttack()
    {
        ExecuteAttack();
    }

    // 구체 활성화 & 위치 초기화
    protected override void OnStartupBegin()
    {
        instance = Instantiate(prefab);
        instance.transform.position = transform.position;
    }

    // 구체 발사. 구체 수명은 구체쪽에서 관리. Fire & Forget 방식이니 구체 소멸 관리해줄 필요 x
    protected override void OnActiveBegin()
    {
        // 목표 위치 가져오기
        GameObject target;
        if(blackboard.TryGet(BBK.Enemy, out target))
        {
            // TODO: 예외처리 추가
        }

        // 발사 벡터 계산
        Vector2 launchVector = (target.transform.position - transform.position).normalized * projectileSpeed;

        // 값 전달 & 발사 수행
        var energyBallScript = instance.GetComponent<LastBossEnergyBall>();
        energyBallScript.SetTarget(target);
        energyBallScript.OnDestroyAction += ResetEnergyBallCount;
        energyBallScript.InitProjectile(launchVector);
    }

    void ResetEnergyBallCount()
    {
        if (counter != null)
            counter.ResetCount("EnergyBall");
    }
}
