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
            // TODO: 플레이어에게 데미지 주기
        }
    }
}
