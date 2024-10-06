using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

public class Task_A_Boss3ThrowRock : Task_A_Base
{
    [SerializeField] private GameObject rockPrefab;
    [SerializeField] private Transform thrower; // 바위를 잡고 휘두를, 발사될 위치.
    [SerializeField] private Transform receiver;    // 바위가 버섯에 튕겨났을 때 회수할 위치
    [SerializeField] private float throwSpeed = 3f;

    private GameObject rockInstance;

    [Task]
    void ThrowRock()
    {
        ExecuteAttack();
    }

    protected override void OnStartupBegin()
    {
        // 바위 소환 & thrower에 부착
        rockInstance = Instantiate(rockPrefab, thrower.position, Quaternion.identity, thrower);
        // 이후 thrower쪽 애니메이션에 의해 '휘둘러지는' 모션 출력
    }

    protected override void OnActiveBegin()
    {
        Boss3Rock projectileComponent = rockInstance.GetComponent<Boss3Rock>();
        projectileComponent.InitProjectile(GetCurrentDir().toVector2() * throwSpeed);
        projectileComponent.returnPosition = receiver.position;
        projectileComponent.damageReceiver = GetComponent<MonsterDamageReceiver>();
    }
}
