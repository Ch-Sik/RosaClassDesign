using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButterFlyCage : MonoBehaviour
{
    [SerializeField] 
    private ButterFly butterfly;

    [SerializeField, ReadOnly]
    private Vector3 butterflyReleasePosition;       // 해방한 나비가 어디로 날아가야 하는지
    

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }
    
    private void Init()
    {
        if ( butterfly == null )
        {
            butterfly = GetComponentInChildren<ButterFly>();
            if(butterfly == null)
            {
                Debug.LogError("나비장이 나비를 찾을 수 없음!");
                return;
            }
        }
        butterflyReleasePosition = butterfly.waypointMoules.GetChild(0).position;

        // 나비가 나비장에 갇힌 상태에서 플레이어 공격과 상호작용하는 것 방지
        butterfly.interactable = false;
    }


    // 플레이어의 공격에 닿았을 때, 나비를 해방한다.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ReleaseButterfly();

        // TODO: 나비장 파괴되는 연출 추가
        Destroy(gameObject, 1f);
    }

    private void ReleaseButterfly()
    {
        // Destroy 전에 나비를 자식오브젝트가 아니도록 설정
        butterfly.transform.parent = transform.parent;

        Sequence releaseSequence = DOTween.Sequence()
            .AppendInterval(1f)                                                         // 잠시 쉬었다가
            .Append(butterfly.butterFly.transform.DOMove(butterflyReleasePosition, 1f)) // 지정된 위치로 이동
            .AppendCallback(() =>                                                       // 그리고 상호작용 활성화
            {
                butterfly.interactable = true;
            });
    }
}
