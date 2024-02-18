using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D)), DisallowMultipleComponent]
public class MonsterDamageInflictor : MonoBehaviour
{
    public int damage;

    public bool isUseIgnoreDuration = false;
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
            Debug.Log("damaged");
            if(!isUseIgnoreDuration)
            {
                go.GetComponent<PlayerMovement>().PlayerDamageReceiver(gameObject, damage);
            }
            else
            {
                go.GetComponent<PlayerMovement>().PlayerDamageReceiver(gameObject, damage,ignoreDuration);
            }
           
        }
    }
}
