using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D)), DisallowMultipleComponent]
public class MonsterDamageInflictor : MonoBehaviour
{
    public int damage;
    public float ignoreDuration = 2f; 
    private void OnCollisionEnter2D(Collision2D collision)
    {
        DoDamageIfItsPlayer(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DoDamageIfItsPlayer(collision.gameObject);
    }

    private void DoDamageIfItsPlayer(GameObject go)
    {
        if (go.CompareTag("Player"))
        {
            Debug.Log($"플레이어에게 {damage}데미지");
            // TODO: 플레이어에게 데미지 주기
            go.GetComponent<PlayerMovement>().OnKnockback(new Vector3(gameObject.transform.position.x,
                    gameObject.transform.position.y - (gameObject.transform.localScale.y / 2),
                    gameObject.transform.position.z));
            go.GetComponent<PlayerState>().TakeDamage(damage);

            int originalLayer = gameObject.layer;
            int collisionLayer = go.layer;

            // 현재 게임 오브젝트와 충돌한 오브젝트의 충돌을 무시
            Physics2D.IgnoreLayerCollision(originalLayer, collisionLayer, true);

            // 일정 시간 후 충돌 무시 해제
            StartCoroutine(RestoreCollision(originalLayer, collisionLayer, ignoreDuration));
        }
    }
    IEnumerator RestoreCollision(int originalLayer, int collisionLayer, float delay)
    {
        yield return new WaitForSeconds(delay);

        // 충돌 무시 해제
        Physics2D.IgnoreLayerCollision(originalLayer, collisionLayer, false);
    }
}
