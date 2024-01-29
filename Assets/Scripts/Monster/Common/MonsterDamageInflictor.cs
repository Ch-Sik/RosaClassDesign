using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MonsterDamageInflictor : MonoBehaviour
{
    public int damage;
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
        }
    }
}
