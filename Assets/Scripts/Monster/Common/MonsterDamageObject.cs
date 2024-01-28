using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MonsterDamageObject : MonoBehaviour
{
    public int damage;
    public float hitCoolTime = 1.5f;
    [ReadOnly, SerializeField] private Timer hitTimer;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if(hitTimer != null)
            {
                if (hitTimer.duration > hitCoolTime)
                {
                    collision.gameObject.GetComponent<PlayerMovement>().OnKnockback(new Vector3(gameObject.transform.position.x,
                        gameObject.transform.position.y - (gameObject.transform.localScale.y / 2),
                        gameObject.transform.position.z));
                    collision.gameObject.GetComponent<PlayerState>().TakeDamage(damage);
                }
            }
            else
            {
                collision.gameObject.GetComponent<PlayerMovement>().OnKnockback(new Vector3(gameObject.transform.position.x,
                    gameObject.transform.position.y - (gameObject.transform.localScale.y / 2),
                    gameObject.transform.position.z));
                collision.gameObject.GetComponent<PlayerState>().TakeDamage(damage);
            }

            hitTimer = Timer.StartTimer();
            // Do Damage
            
            
        }
    }
}
