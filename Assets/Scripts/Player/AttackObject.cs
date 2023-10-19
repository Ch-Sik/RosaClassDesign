using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackObject : MonoBehaviour
{
    Collider2D col;
    PlayerCombat playerCombat;
    LayerMask attackableObjects;
    LayerMask butterfly;

    public void Init(PlayerCombat playerCombat, LayerMask attackableObjects, LayerMask butterfly)
    {
        this.playerCombat = playerCombat;
        this.attackableObjects = attackableObjects;
        this. butterfly = butterfly;
    }

    private void Start()
    {
        col = GetComponent<Collider2D>();
        col.enabled = false;
        col.isTrigger = true;
    }

    public void StartAttack()
    {
        col.enabled = true;
    }

    public void EndAttack()
    {
        col.enabled = false;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        playerCombat.CombatCalcultor(collision.gameObject);
    }
}
