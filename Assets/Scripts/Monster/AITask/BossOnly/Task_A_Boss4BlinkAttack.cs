using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using DG.Tweening;

public class Task_A_Boss4BlinkAttack : Task_A_Base
{
    [Tooltip("사라질 때까지 딜레이")]
    [SerializeField] float blinkDelay;

    [Tooltip("사라지는 이펙트")]
    [SerializeField] GameObject blinkVFX;

    [Tooltip("몬스터 본체의 비주얼")]
    [SerializeField] GameObject bodyVisual;

    [Tooltip("몸통 충돌판정")]
    [SerializeField] Collider2D bodyCollider;

    [Tooltip("다시 나타날 때의 높이")]
    [SerializeField] float jumpHeight;

    [Tooltip("공격 이펙트 & 판정 히트박스")]
    [SerializeField] GameObject attackColliderAndVFX;

    [Task]
    private void BlinkAttack()
    {
        ExecuteAttack();
    }

    protected override void OnStartupBegin()
    {
        DOTween.Sequence()
            .AppendInterval(blinkDelay)
            .AppendCallback(()=>
            {
                // 이펙트 남겨두고 비주얼은 사라지기, 충돌판정 끄기
                blinkVFX.SetActive(true);
                bodyVisual.SetActive(true);
                bodyCollider.enabled = false;
            })
            .AppendInterval(1f)
            .AppendCallback(()=>{
                blinkVFX.SetActive(false);      // 약 1초 후에 점멸 이펙트 끄기.
            });
    }

    protected override void OnActiveBegin()
    {
        // 대상 머리 위로 순간이동
        GameObject enemy;
        blackboard.TryGet(BBK.Enemy, out enemy);
        Vector3 newPosition = enemy.transform.position + Vector3.up * jumpHeight;
        transform.position = newPosition;

        // 공격 판정 켜기
        attackColliderAndVFX.SetActive(true);
        DOTween.Sequence()
            .AppendInterval(0.1f)       // 공격 판정 켜고 잠시 기다려서 '벽에 충돌하여 튕겨나오는 모습' 보이지 않도록 함.
            .AppendCallback(()=>
            {
                blinkVFX.SetActive(true);           // 점멸 이펙트 켜기
            });
        
        // 떨어지는 건 중력 영향으로 자연스럽게 될 것 같으니 일단 따로 내려오는 코드는 빼고 테스트해보도록 하자.
    }

    protected override void OnRecoveryBegin()
    {
        attackColliderAndVFX.SetActive(false);
        blinkVFX.SetActive(false);
    }
}
