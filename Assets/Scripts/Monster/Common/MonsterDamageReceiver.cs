using AnyPortrait;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class MonsterDamageReceiver : DamageReceiver
{
    [SerializeField] Blackboard blackboard;
    [SerializeField] MonsterState monsterState;
    [SerializeField] new Rigidbody2D rigidbody;
    [SerializeField] apPortrait portrait;

    [SerializeField] private bool isSuperArmour;    // 피격 리액션을 하지 않으며 밀려나지 않음
    [SerializeField] private bool isInvincible;     // 피격 리액션은 수행하지만 데미지를 입지 않음
    [SerializeField] private float knockbackCoeff;  // 넉백 계수

    bool isAlive = true;
    Coroutine blinkCoroutine;
    float reactionBrightness = 0.75f;               // 피격/사망 시의 밝기

    private void Start()
    {
        if (blackboard == null)
        {
            blackboard = GetComponent<Blackboard>();
            Debug.Assert(blackboard != null, $"{gameObject.name}: Blackboard를 찾을 수 없음!");
        }

        // 무적 상태인 기믹형 적은 체력 관리 필요 없음
        if (!isInvincible && monsterState == null)
        {
            monsterState = GetComponent<MonsterState>();
            Debug.Assert(monsterState != null, $"{gameObject.name}: MonsterState를 찾을 수 없음!");
        }

        // 넉백계수가 0인 적은 넉백 적용을 위한 rigidbody 필요 없음
        if (knockbackCoeff > float.Epsilon && rigidbody == null)
        {
            rigidbody = GetComponent<Rigidbody2D>();
            Debug.Assert(rigidbody != null, $"{gameObject.name}: Rigidbody2D를 찾을 수 없음!");
        }

        // 무적 상태인 기믹형 적은 피격 연출 필요 없음
        if (!isInvincible && portrait == null)
        {
            portrait = GetComponentInChildren<apPortrait>();
            if(portrait == null)
            {
                Debug.LogWarning("apPortrait를 찾을 수 없음!", gameObject);
            }
        }
    }

    public override void GetHitt(int damage, float attackAngle)
    {
        if (!isAlive) return;

        // BlinkEffect 수행이 mosterState.TakeDamage->BroadcastMessage("OnDie")->this.OnDie 보다 앞서야 함.
        // 그래야 Die로 인한 밝기 변경이 Blink에 의해 덮어씌워지지 않음.
        BlinkEffect();

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

    // 피격되었을 때 스프라이트 깜빡이기
    private void BlinkEffect()
    {
        if (portrait == null)
            return;         // 아직 애니메이션이 적용되지 않은 녀석들 예외 처리

        // 딱히 추가로 수정할 필요 없어보여서 수치들을 하드코딩했는데 필요하다면 필드값으로 빼낼 것
        const float timePerBlink = 0.2f;
        const int blinkCount = 2;
        Color blinkColor = Color.gray * reactionBrightness;
        blinkColor.a = 1;

        blinkCoroutine = StartCoroutine(DoBlink());

        IEnumerator DoBlink()
        {
            for (int i = 0; i < blinkCount; i++)
            {
                portrait.SetMeshColorAll(blinkColor);
                yield return new WaitForSeconds(timePerBlink / 2);
                portrait.ResetMeshMaterialToBatchAll();
                yield return new WaitForSeconds(timePerBlink / 2);
            }
            blinkCoroutine = null;
        }
    }

    // OnDie 메세지는 MonsterState.TakeDamage에서 발생됨.
    private void OnDie()
    {
        isAlive = false;
        FadeEffect();
    }

    // 사망했을 때 스프라이트 색 어두워지기
    private void FadeEffect()
    {
        if (portrait == null)
            return;         // 아직 애니메이션이 적용되지 않은 녀석들 예외 처리

        const int step = 5;
        const float timePerBlink = 0.2f;
        const float fadeTime = 0.3f;
        Color defaultColor = Color.gray;   // AP의 기본 색상
        Color goalColor = Color.gray * reactionBrightness;
        goalColor.a = 1;

        if (blinkCoroutine != null)
            StopCoroutine(blinkCoroutine);
        StartCoroutine(DoFadeOut());

        // 피격 확인을 위해 일단 한번 깜빡인 후에 페이드
        IEnumerator DoFadeOut()
        {
            portrait.SetMeshColorAll(goalColor);
            yield return new WaitForSeconds(timePerBlink / 2);
            portrait.ResetMeshMaterialToBatchAll();
            yield return new WaitForSeconds(timePerBlink / 2);
            float fadeTick = fadeTime / step;
            for (int i = 1; i <= step; i++)
            {
                Color newColor = Color.Lerp(defaultColor, goalColor, i / (float)step);
                Debug.Log("newColor: " + newColor);
                portrait.SetMeshColorAll(newColor);
                yield return new WaitForSeconds(fadeTick);
            }
        }
    }
}
