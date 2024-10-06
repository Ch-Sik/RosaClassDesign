using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using DG.Tweening;

public class Task_A_RisingPheonix : Task_A_Base
{
    new Rigidbody2D rigidbody;
    MonsterDamageInflictor attackComponent;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        attackComponent = GetComponent<MonsterDamageInflictor>();
    }

    [SerializeField] private GameObject previewObject;

    [Task]
    void RisingPheonix()
    {
        ExecuteAttack();
    }

    protected override void OnStartupBegin()
    {
        Vector3 targetPosition = transform.position;

        // 적(플레이어) 정보 가져오기
        if(blackboard.TryGet(BBK.Enemy, out GameObject enemy))
        {
            targetPosition.x = enemy.transform.position.x;
        }
        else
        {
            Debug.LogError("적(플레이어)를 찾을 수 없음!");
        }

        DOTween.Sequence()
            // 1. 위로 올라가기
            .Append(rigidbody.DOMoveY(transform.position.y + 20f, 1.5f))
            // 2. 공격 판정 끄고, 저 멀리서 나타나기
            .AppendCallback(()=>{
                attackComponent.attackEnabled = false;
                transform.position = targetPosition + Vector3.forward * 20;
            })
            // 3. 지면 아래로 날아오기
            .Append(transform.DOMove(targetPosition + Vector3.down * 15, 1f));
    }

    protected override void OnActiveBegin()
    {
        //
    }
}
