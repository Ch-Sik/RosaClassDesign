using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HittableMonster : Hittable
{
    [SerializeField] Blackboard blackboard;
    [SerializeField] MonsterState monsterState;
    [SerializeField] bool isSuperArmour;        // 피격 리액션을 하지 않으며 밀려나지 않음
    [SerializeField] bool isInvincible;         // 피격 리액션은 수행하지만 데미지를 입지 않음

    private void Start()
    {
        if (blackboard == null)
        {
            blackboard = GetComponent<Blackboard>();
            if (blackboard == null)
                Debug.LogError($"{gameObject.name}: Blackboard를 찾을 수 없음!");
        }

        // 무적 상태인 기믹형 적은 체력 관리 필요 없음
        if (!isInvincible && monsterState == null)
        {
            monsterState = GetComponent<MonsterState>();
            if (monsterState == null)
                Debug.LogError($"{gameObject.name}: MonsterState를 찾을 수 없음!");
        }
    }

    public override void GetHitt(int damage)
    {
        if(!isSuperArmour)
        {
            // 여기서는 블랙보드에 isHitt을 true로 설정해두기만 하고
            // 피격 액트 수행은 상태머신에서 실행
            blackboard.Set(BBK.isHitt, true);
            StartCoroutine(SetIsHittFalse());
            IEnumerator SetIsHittFalse()
            {
                yield return 0;     // 다음프레임까지 대기
                blackboard.Set(BBK.isHitt, false);
            }
        }
        if(!isInvincible)
        {
            monsterState.TakeDamage(damage);
        }
    }
}
