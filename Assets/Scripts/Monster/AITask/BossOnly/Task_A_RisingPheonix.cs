using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using DG.Tweening;
using Sirenix.OdinInspector;

public class Task_A_RisingPheonix : Task_A_Base
{
    [SerializeField] GameObject previewObject;      // 공격 범위 미리보기 오브젝트
    [InfoBox("공격 데미지나 판정 범위 등은 공격 오브젝트 쪽으로 가서 수정할 것.")]
    [SerializeField] GameObject attackObject;       // 공격 오브젝트
    [InfoBox("보스방의 아래, 정가운데에 위치시킬 것")]
    [SerializeField] Transform bossroomCenter;

    Rigidbody2D _rigidbody;
    MonsterDamageInflictor _bodyDamageComponent;
    GameObject _target;
    Vector3 _startPosition;      // 패턴 후 복귀 등에 사용할 '기준 위치'

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _bodyDamageComponent = GetComponent<MonsterDamageInflictor>();
        previewObject.SetActive(false);
        _startPosition = transform.position;
    }


    [Task]
    void RisingPheonix()
    {
        ExecuteAttack();
    }

    protected override void OnStartupBegin()
    {
        Vector3 targetPosition = transform.position;

        // 적(플레이어) 정보 가져오기
        if(blackboard.TryGet(BBK.Enemy, out _target))
        {
            targetPosition.x = _target.transform.position.x;
        }
        else
        {
            Debug.LogError("적(플레이어)를 찾을 수 없음!");
        }

        DOTween.Sequence()
            // 1. 위로 올라가기
            .Append(_rigidbody.DOMoveY(transform.position.y + 20f, 1.5f))
            // 2. 공격 판정 끄고, 저 멀리서 나타나기
            .AppendCallback(()=>{
                _bodyDamageComponent.attackEnabled = false;
                transform.position = targetPosition + Vector3.forward * 20;
            })
            // 3. 지면 아래로 날아오기
            // 여기 Transform으로 하는 게 맞는가? Lifecycle 주기가 좀 다른데;;
            .Append(transform.DOMove(targetPosition + Vector3.down * 15, 1f))
            // 4. 미리보기 오브젝트 활성화
            .AppendCallback(() => { previewObject.SetActive(true); });
    }

    protected override void OnStartupLast()
    {
        // 5. 미리보기 오브젝트 좌우 위치 업데이트
        if(previewObject.activeSelf)
        {
            Vector3 newPosition = previewObject.transform.position;
            newPosition.x = _target.transform.position.x;
            previewObject.transform.position = newPosition;

            // 자기 자신의 위치도 업데이트
            newPosition = transform.position;
            newPosition.x = _target.transform.position.x;
            transform.position = newPosition;
        }
    }

    protected override void OnActiveBegin()
    {
        DOTween.Sequence()
            // 6. 플레이어가 피할 수 있도록 약간의 여유 시간 주기
            .AppendInterval(1f)
            // 7. 미리보기 오브젝트 비활성화
            .AppendCallback(() => {
                previewObject.SetActive(false);
            })
            // 8. 이펙트&데미지 판정 활성화하고 위로 치솟아오르기
            .AppendCallback(() => {
                attackObject.SetActive(true);
                Debug.Log($"startPosition: {_startPosition}");
            })
            .Append(_rigidbody.DOMoveY(_startPosition.y + 20, 1f));
    }

    protected override void OnRecoveryBegin()
    {
        attackObject.SetActive(false);
        _bodyDamageComponent.attackEnabled = true;

        // 8. 플레이어 위치 고려해서 반대쪽에 내려오기
        float offsetFromCenter = Mathf.Abs(_startPosition.x - bossroomCenter.position.x);
        if(_target.transform.position.x > bossroomCenter.transform.position.x)
        {
            // 플레이어가 보스방 중앙보다 오른쪽에 있을 경우, 왼쪽에 착지
            transform.position = new Vector3(bossroomCenter.position.x - offsetFromCenter, transform.position.y, 0);
            LookAt2D(_target.transform.position);
            DOTween.Sequence()
                .Append(_rigidbody.DOMoveY(_startPosition.y, 3f));
        }
        else
        {
            // 플레이어가 보스방 중앙보다 왼쪽에 있을 경우, 오른쪽에 착지
            transform.position = new Vector3(bossroomCenter.position.x + offsetFromCenter, transform.position.y, 0);
            LookAt2D(_target.transform.position);
            DOTween.Sequence()
                .Append(_rigidbody.DOMoveY(_startPosition.y, 3f));
        }
    }
}
