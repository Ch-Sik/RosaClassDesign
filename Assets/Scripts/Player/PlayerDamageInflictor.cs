using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

/// <summary>
/// 공격을 판정하기 위한 투사체를 위한 스크립트입니다.
/// </summary>

public class PlayerDamageInflictor : MonoBehaviour
{
    Collider2D col;                                 //오브젝트의 충돌 컴포넌트
    PlayerCombat playerCombat;                      //PlayerCombat과 이벤트 전달을 위해 직접 연결


    float damagePercent;                            //콤보 당 데미지 비중을 가지기 위한 비중 값
    float attackAngle;

    LayerMask layer_wall;
    LayerMask layer_attackable;                    //attackable한 objects의 layermask
    LayerMask layer_butterfly;                      //butterfly인식을 위한 layermask



    //PlayerCombat에서 Init해준다. 기본적으로 
    public void Init(PlayerCombat playerCombat, float damagePercent, LayerMask wall, LayerMask attackableObjects, LayerMask butterfly)
    {
        this.playerCombat = playerCombat;
        this.damagePercent = damagePercent;
        layer_wall = wall;
        layer_attackable = attackableObjects;
        layer_butterfly = butterfly;
    }

    //시작과 동시에 콜라이더를 세팅한다.
    private void Start()
    {
        col = GetComponent<Collider2D>();
        col.enabled = false;
        col.isTrigger = true;
    }

    // 공격 판정 활성화 or 비활성화
    // attackAngle은 공격 성공시 넉백 계산에 사용
    public void SetAttackActive(float attackAngle)
    {
        this.attackAngle = attackAngle;
        col.enabled = true;
    }

    public void SetAttackInactive()
    {
        col.enabled = false;
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if ((layer_wall & 1 << collision.gameObject.layer) > 0)
        {
            Debug.Log("벽 충돌로 인한 공격 취소");
            playerCombat.StopAttack();
        }
    }

    //충돌 감지
    public void OnTriggerEnter2D(Collider2D collision)
    {
        // if (playerCombat.canInteraction) 상호작용이 필요한 오브젝트에 적용할 것.
        if ((layer_wall & 1 << collision.gameObject.layer) > 0)
        {
            Debug.Log("벽 충돌로 인한 공격 취소");
            playerCombat.StopAttack();
        }


        // 몬스터 등등과 충돌한다면,
        if ((layer_attackable & 1 << collision.gameObject.layer) != 0)
        {
            Debug.Log($"플레이어 공격: {collision.gameObject.name}");
            collision.GetComponent<DamageReceiver>().GetHitt(Mathf.RoundToInt(PlayerRef.Instance.State.AttackDmg * damagePercent), attackAngle);
            playerCombat.StopAttack();
        }

        //나비와 충돌한다면,
        if ((layer_butterfly.value & 1 << collision.gameObject.layer) > 0 &&
            playerCombat.canInteraction)
        {
            if (collision.GetComponent<Butterfly>().isCaged)
                return;
            Debug.Log("나비와 충돌됨");
            //PlayerCombat에 RideButterFly함수를 실행시키며, 나비의 부모 데이터를 전달한다.
            playerCombat.RideButterFly(collision.transform.parent.GetComponentInChildren<Butterfly>());
        }
    }
}
