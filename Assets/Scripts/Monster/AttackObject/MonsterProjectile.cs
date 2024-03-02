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

    [SerializeField]
    private new Rigidbody2D rigidbody;
    [SerializeField]
    private new Collider2D collider;

    public void InitProjectile(Vector2 velocity)
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
        rigidbody.velocity = velocity * speedScale;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // 벽에 닿거나 플레이어에게 닿으면 사라지기
        if (collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            // TODO: 투사체 사라지는 연출 넣기
            Debug.Log("몬스터 투사체 지형과 접촉");
            rigidbody.velocity = Vector2.zero;
            this.collider.enabled = false;
            Destroy(gameObject, 1f);
        }

        if(collider.gameObject.CompareTag("Player"))
        {
            Debug.Log("몬스터 투사체 플레이어와 접촉");
            rigidbody.velocity = Vector2.zero;
            this.collider.enabled = false;
            Destroy(gameObject, 1f);
        }
    }
}
