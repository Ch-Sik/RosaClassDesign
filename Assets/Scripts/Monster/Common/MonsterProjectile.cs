using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: 투사체 회전 옵션 만들기
public class MonsterProjectile : MonoBehaviour
{
    [SerializeField, Tooltip("투사체 속도")]
    private float projectileSpeed = 3f;

    [SerializeField]
    private new Rigidbody2D rigidbody;
    [SerializeField]
    private new Collider2D collider;

    // projectileDir은 Normalize된 벡터임을 전제
    public void InitProjectile(Vector2 projectileDir)
    {
        // 필요 컴포넌트 설정
        if(rigidbody == null)
        {
            rigidbody = GetComponent<Rigidbody2D>();
            Debug.Assert(rigidbody != null, $"{gameObject.name}: Rigidbody2D 레퍼런스가 설정되어있지 않음");
        }

        // 속도 설정
        rigidbody.velocity = projectileDir * projectileSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 벽에 닿거나 플레이어에게 닿으면 사라지기
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            // TODO: 투사체 사라지는 연출 넣기
            Debug.Log("몬스터 투사체 지형과 접촉");
            rigidbody.velocity = Vector2.zero;
            collider.enabled = false;
            Destroy(gameObject, 1f);
        }

        if(collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("몬스터 투사체 플레이어와 접촉");
            rigidbody.velocity = Vector2.zero;
            collider.enabled = false;
            Destroy(gameObject, 1f);
        }
    }
}
