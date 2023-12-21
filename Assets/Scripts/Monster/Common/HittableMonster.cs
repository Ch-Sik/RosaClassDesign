using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HittableMonster : Hittable
{
    [SerializeField] Blackboard blackboard;
    [SerializeField] MonsterState monsterState;
    [SerializeField] new Rigidbody2D rigidbody;

    [SerializeField] private bool isSuperArmour;    // 피격 리액션을 하지 않으며 밀려나지 않음
    [SerializeField] private bool isInvincible;     // 피격 리액션은 수행하지만 데미지를 입지 않음
    [SerializeField] private float knockbackCoeff;  // 넉백 계수

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

        // 넉백계수가 0인 적은 넉백 적용을 위한 rigidbody 필요 없음
        if (knockbackCoeff > float.Epsilon && rigidbody == null)
        {
            rigidbody = GetComponent<Rigidbody2D>();
            if (rigidbody == null)
                Debug.LogError($"{gameObject.name}: Rigidbody2D를 찾을 수 없음!");
        }
    }

    public override void GetHitt(int damage, float attackAngle)
    {
        if(!isSuperArmour)
        {
            // 여기서는 블랙보드에 isHitt을 true로 설정해두기만 하고
            // 피격 액트 수행은 상태머신에서 실행
            blackboard.Set(BBK.isHitt, true);
            
            // 넉백 계수가 0보다 크다면 넉백 수행
            if(knockbackCoeff > float.Epsilon)
            {
                KnockBack(attackAngle);
            }

            // 다음 프레임에서 IsHitt 플래그 False로 만들기
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

    private void KnockBack(float attackAngle)
    {

        StartCoroutine(DoKnockBack());

        // Hit Stop 연출 및 몬스터 패턴 캔슬 시 강제로 속도 0으로 고정되는 것 고려, 넉백은 잠시 기다린 후 수행함.
        IEnumerator DoKnockBack()
        {
            // TODO: Hit Stop 연출 추가할 경우 주석 해제
            // yield return new WaitForSeconds(0.1f);
            yield return 0;

            Debug.LogWarning(attackAngle);
            Debug.LogWarning(-11 / 2);
            // 넉백 방향을 4방향으로 고정?
            attackAngle =  (((int)(attackAngle + 180 + 45)) / 90 - 2)  * 90f;
            Debug.LogWarning(attackAngle);

            attackAngle *= Mathf.Deg2Rad;
            // attackDir는 PlayerCombat에서 Normalize되어서 Unit Vector임이 보장됨
            Vector2 knockbackVector = new Vector2(Mathf.Cos(attackAngle), Mathf.Sin(attackAngle)) * knockbackCoeff;
            Debug.LogWarning(knockbackVector);
            rigidbody.velocity = knockbackVector;
        }
    }
}
