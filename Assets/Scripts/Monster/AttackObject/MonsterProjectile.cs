using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: 투사체 회전 옵션 만들기
public class MonsterProjectile : MonoBehaviour
{
    [SerializeField, Tooltip("중력 영향 여부")]
    private bool useGravity = false;
    [SerializeField, Tooltip("속도 계수. 중력 사용하지 않을 때에만 사용할 것")]
    private float speedScale = 1f;
    [SerializeField, Tooltip("투사체 랜덤 회전 여부")]
    private bool useRandomRotation = false;
    [SerializeField, Tooltip("랜덤 회전 최대치")]
    private float randomRotationRange = 30f;

    [SerializeField]
    private new Rigidbody2D rigidbody;
    [SerializeField]
    private new Collider2D collider;
    [SerializeField]
    private Animator animator;

    public void InitProjectile(Vector2 direction)
    {
        // 필요 컴포넌트 설정
        if(rigidbody == null)
        {
            rigidbody = GetComponent<Rigidbody2D>();
            Debug.Assert(rigidbody != null, $"{gameObject.name}: Rigidbody2D 레퍼런스가 설정되어있지 않음");
        }
        if (useGravity)
            rigidbody.isKinematic = false;
        else
            rigidbody.isKinematic = true;

        // 기본 속도 설정
        rigidbody.velocity = direction * speedScale;
        // Debug.Log($"투사체 속도:{direction * speedScale}");

        // 회전값 설정
        if(useRandomRotation)
        {
            rigidbody.angularVelocity = Random.Range(-randomRotationRange, randomRotationRange);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // 벽에 닿거나 플레이어에게 닿으면 사라지기
        if (collider.gameObject.layer == LayerMask.NameToLayer("Ground") ||
            collider.gameObject.layer == LayerMask.NameToLayer("Cube") ||
            collider.gameObject.layer == LayerMask.NameToLayer("PlayerGrab"))
        {
            // TODO: 투사체 사라지는 연출 넣기
            // Debug.Log("몬스터 투사체 지형과 접촉");
            rigidbody.velocity = Vector2.zero;
            this.collider.enabled = false;
            DoDestroy(1f);
        }

        if(collider.gameObject.CompareTag("Player"))
        {
            Debug.Log("몬스터 투사체 플레이어와 접촉");
            rigidbody.velocity = Vector2.zero;
            this.collider.enabled = false;
            DoDestroy(1f);
        }
    }

    private void DoDestroy(float delay)
    {
        if(animator != null)
        {
            animator.SetTrigger("disappear");
        }
        Destroy(gameObject, delay);
    }
}
