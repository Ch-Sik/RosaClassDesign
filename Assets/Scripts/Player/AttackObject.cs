using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

/// <summary>
/// 공격을 판정하기 위한 투사체를 위한 스크립트입니다.
/// </summary>

public class AttackObject : MonoBehaviour
{
    Collider2D col;                                 //오브젝트의 충돌 컴포넌트
    PlayerCombat playerCombat;                      //PlayerCombat과 이벤트 전달을 위해 직접 연결
    LayerMask attackableObjects;                    //attackable한 objects의 layermask
    LayerMask butterfly;                            //butterfly인식을 위한 layermask

    //PlayerCombat에서 Init해준다. 기본적으로 
    public void Init(PlayerCombat playerCombat, LayerMask attackableObjects, LayerMask butterfly)
    {
        this.playerCombat = playerCombat;
        this.attackableObjects = attackableObjects;
        this.butterfly = butterfly;
    }

    //시작과 동시에 콜라이더를 세팅한다.
    private void Start()
    {
        col = GetComponent<Collider2D>();
        col.enabled = false;
        col.isTrigger = true;
    }

    //공격 시작 시 콜라이더 활성화
    public void StartAttack() { col.enabled = true; }

    //공격 종료 후 콜라이더 비활성화
    public void EndAttack() { col.enabled = false; }

    //충돌 감지
    public void OnTriggerEnter2D(Collider2D collision)
    {
        //나비와 충돌한다면,
        if ((butterfly.value & 1 << collision.gameObject.layer) > 0)
        {
            //PlayerCombat에 RideButterFly함수를 실행시키며, 나비의 부모 데이터를 전달한다.
            //나비의 충돌포인트는 나비 자체에 있으나, 나비 프리팹은 웨이포인트 통솔을 위한 부모오브젝트에서 스크립트를 조절하기 때문
            playerCombat.RideButterFly(collision.transform.parent.GetComponent<ButterFly>());
        }
    }
}
