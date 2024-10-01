using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3Projectile : MonsterProjectile
{
    // 벽에 박혀있는지 여부
    [SerializeField] private bool isStuck = false;

    // 벽에 닿아도 사라지지 않고 남아있도록 함수 오버라이드
    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        // Destroy하는 코드는 삭제
        // 제자리에 박혀있기, 충돌(공격)판정은 비활성화
        if (collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            rigidbody.velocity = Vector2.zero;
            rigidbody.isKinematic = true;
            collider.enabled = false;
        }
    }

    // 투사체 회수 기믹
    public void RetrieveProjectile(Vector3 returnPosition)
    {
        // 돌아가야할 위치 계산

        // 돌아가기 시퀀스
    }
}
