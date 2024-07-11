using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseBlock : MonoBehaviour
{
    [SerializeField]
    bool defaultPhase = false;
    [SerializeField]
    new Collider2D collider;
    [SerializeField, ReadOnly]
    int phaseFlowerCount = 0;

    private MovePlatform movePlatform = null;

    public int PhaseFlowerCount {
        get { return phaseFlowerCount; } 
        set { 
            phaseFlowerCount = value;
            ChangePhase(phaseFlowerCount > 0);
        } 
    }

    // 참이면 충돌 가능, 거짓이면 충돌 불가능
    bool currentPhase = true;


    private void Start()
    {
        if(collider == null)
            collider = GetComponent<Collider2D>();
        movePlatform = GetComponentInParent<MovePlatform>();


        if(defaultPhase != currentPhase)
        {
            ChangePhase(defaultPhase);
        }
        else
        {
            // 참이면 충돌 가능
            collider.isTrigger = !currentPhase;
        }
    }

    public void ChangePhase(bool value)
    {
        if (currentPhase == value)
            return;

        currentPhase = value;
        collider.isTrigger = !currentPhase;
        // TODO: 페이즈 변환 연출 추가하기
        Debug.Log("PhaseBlock" + (currentPhase ? "켜짐" : "꺼짐"));

        // 만약 플레이어가 해당 플랫폼에 '발을 디딘 상태'였다면, '발을 디딘 상태' 해제함.
        if(movePlatform != null && PlayerRef.Instance.transform.parent == movePlatform.transform)
        {
            movePlatform.HandleChildTriggerExit(PlayerRef.Instance.transform);
        }
    }
}
