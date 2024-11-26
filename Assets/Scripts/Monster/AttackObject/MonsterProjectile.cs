using System.Collections;
using System.Collections.Generic;
using System.Text;
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

    [SerializeField, Tooltip("투사체와 충돌하여 가로막힐 레이어")]
    private LayerMask blockingLayers = 656384;          // 기본값: "Ground", "Cube", "PlayerGrab"
    [SerializeField, Tooltip("버섯 파괴 가능?")]
    protected bool canDestroyMushroom = false;
    [SerializeField, Tooltip("투사체가 벽에 닿았을 때 행동 설정")]
    protected ProjectileWallHitOption onWallHit = ProjectileWallHitOption.Destroy;

    [SerializeField]
    protected new Rigidbody2D rigidbody;
    [SerializeField]
    protected new Collider2D collider;
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
        if(collider == null)
        {
            collider = GetComponent<Collider2D>();
            Debug.Assert(collider != null);
        }

        // 기본 속도 설정
        rigidbody.velocity = direction * speedScale;
        // Debug.Log($"투사체 속도:{direction * speedScale}");

        // 회전값 설정
        if(useRandomRotation)
        {
            rigidbody.angularVelocity = Random.Range(-randomRotationRange, randomRotationRange);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        // 벽에 닿았다면
        if((1 << collider.gameObject.layer & blockingLayers) != 0)
        {
            switch(onWallHit)
            {
                case ProjectileWallHitOption.Disable:
                    rigidbody.velocity = Vector2.zero;
                    this.collider.enabled = false;
                    StartCoroutine(DisableWithDelay());
                    IEnumerator DisableWithDelay()
                    {
                        yield return new WaitForSeconds(1f);
                        gameObject.SetActive(false);
                    }
                    break;
                case ProjectileWallHitOption.Destroy:
                    rigidbody.velocity = Vector2.zero;
                    this.collider.enabled = false;
                    DoDestroy(1f);
                    break;
                case ProjectileWallHitOption.Stop:
                    rigidbody.velocity = Vector2.zero;
                    break;
                case ProjectileWallHitOption.Reflect:
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, rigidbody.velocity, 10f, blockingLayers);
                    if (hit.collider != null)
                    {
                        // 벽에 부딪혔을 때 반사 수행
                        Vector2 normal = hit.normal;
                        Vector2 reflected = Vector2.Reflect(rigidbody.velocity, normal);
                        rigidbody.velocity = reflected;
                    }
                    break;
                default:
                    Debug.LogError("잘못된 ProjectileWallHitOption!");
                    break;
            }
        }

        if(collider.gameObject.CompareTag("Player"))
        {
            Debug.Log("몬스터 투사체 플레이어와 접촉");
            rigidbody.velocity = Vector2.zero;
            this.collider.enabled = false;
            DoDestroy(1f);
        }

        if(canDestroyMushroom && (collider.tag == "Mushroom"))
        {
            Debug.Log("버섯 파괴 시전");
            collider.GetComponent<MagicMushroom>().DoDestroy();
        }
    }

    protected void DoDestroy(float delay)
    {
        if(animator != null)
        {
            animator.SetTrigger("disappear");
        }
        Destroy(gameObject, delay);
    }
}
