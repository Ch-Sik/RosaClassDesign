using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageReceiver : MonoBehaviour
{
    [SerializeField] float defaultNoDmgTime = 2f;

    PlayerRef playerRef;

    public void Start()
    {
        playerRef = PlayerRef.Instance;
    }


    public void GetDamage(GameObject target, int damage)
    {
        playerRef.animation.BlinkEffect();

        playerRef.movement.OnKnockback(new Vector3(target.transform.position.x,
                    target.transform.position.y - (target.transform.localScale.y / 2),
                    target.transform.position.z));
        playerRef.state.TakeDamage(damage);

        int originalLayer = target.layer;
        int collisionLayer = gameObject.layer;

        // 현재 게임 오브젝트와 충돌한 오브젝트의 충돌을 무시
        Physics2D.IgnoreLayerCollision(originalLayer, collisionLayer, true);

        // 일정 시간 후 충돌 무시 해제
        StartCoroutine(RestoreCollision(originalLayer, collisionLayer, defaultNoDmgTime));
    }

    public void GetDamage(GameObject target, int damage, float ignoreDur)
    {
        playerRef.animation.BlinkEffect();

        playerRef.movement.OnKnockback(new Vector3(target.transform.position.x,
                    target.transform.position.y - (target.transform.localScale.y / 2),
                    target.transform.position.z));
        playerRef.state.TakeDamage(damage);

        int originalLayer = target.layer;
        int collisionLayer = gameObject.layer;

        // 현재 게임 오브젝트와 충돌한 오브젝트의 충돌을 무시
        Physics2D.IgnoreLayerCollision(originalLayer, collisionLayer, true);

        // 일정 시간 후 충돌 무시 해제
        StartCoroutine(RestoreCollision(originalLayer, collisionLayer, ignoreDur));
    }

    IEnumerator RestoreCollision(int originalLayer, int collisionLayer, float delay)
    {
        yield return new WaitForSeconds(delay);

        // 충돌 무시 해제
        Physics2D.IgnoreLayerCollision(originalLayer, collisionLayer, false);
    }
}
