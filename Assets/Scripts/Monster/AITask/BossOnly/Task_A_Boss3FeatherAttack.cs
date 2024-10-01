using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

public class Task_A_Boss3FeatherAttack : Task_A_Base
{
    [Header("공격 관련 기본 요소")]
    [SerializeField, Tooltip("투사체 프리팹")]
    private GameObject projectilePrefab;
    [SerializeField, Tooltip("투사체 진행 속도")]
    private float projectileSpeed = 4.0f;
    [SerializeField, Tooltip("투사체 생성될 위치")]
    private Transform muzzle;

    [ReadOnly]
    public List<GameObject> instances;

    Vector2 enemyPosition = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        if(blackboard == null)
        {
            blackboard = GetComponent<Blackboard>();
            Debug.Assert(blackboard != null, $"{gameObject.name}: Blackboard를 찾을 수 없음");
        }
    }

    [Task]
    void FeatherAttack()
    {
        ExecuteAttack();
    }

    protected override void OnStartupBegin()
    {
        // 적 위치 파악
        // StartupBegin 타이밍에 적 조준이 이루어지므로 Startup 시간동안 플레이어가 피할 여유가 있음.
        GameObject enemy;
        if(!blackboard.TryGet(BBK.Enemy, out enemy) || enemy == null)
        {
            // 보스전 한정으로 패턴 도중에 플레이어를 찾을 수 없으면 그건 '오류'임.
            Debug.LogError($"{gameObject.name}: Blackboard에서 적을 찾을 수 없음. 공격 취소");
            Fail();
            return;
        }
        enemyPosition = enemy.transform.position;
    }

    protected override void OnActiveBegin()
    {
        Vector2 attackDir = (enemyPosition - (Vector2)muzzle.position).normalized;

        GameObject projectile = Instantiate(projectilePrefab, muzzle.position, Quaternion.identity);
        projectile.GetComponent<Boss3Projectile>().InitProjectile(attackDir * projectileSpeed);

        // 생성된 인스턴스 public list에 보관
        instances.Add(projectile);
    }
}
