using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D))]
public class Task_A_Piranha : MonoBehaviour
{
    [SerializeField] float breachHeight = 3f;     // 튀어오르는 높이
    [SerializeField] float breachUpTime = 1f;
    [SerializeField] float breachDownTime = 0.7f;
    [SerializeField] float timeBetweenBreach = 1.0f;
    [SerializeField] float startDelay = 0;       // 딜레이를 줘서 다른 피라냐들과 같이 파도타기 구현 가능

    private new Rigidbody2D rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        float originPosition = transform.position.y;
        DOTween.Sequence().AppendInterval(startDelay).AppendCallback(
            () =>
            {
                DOTween.Sequence()
                .Append(
                    rigidbody.DOMoveY(transform.position.y + breachHeight, breachUpTime)
                        .SetEase(Ease.OutCubic))
                .Insert(0, transform.DORotate(new Vector3(0, 0, 0), 0.2f))
                .Append(
                    rigidbody.DOMoveY(originPosition, breachDownTime)
                        .SetEase(Ease.InQuad))
                .Insert(breachUpTime, transform.DORotate(new Vector3(0, 0, 180), 0.2f))
                .AppendInterval(timeBetweenBreach)
                .SetLoops(-1);
            }
        );
    }
}
