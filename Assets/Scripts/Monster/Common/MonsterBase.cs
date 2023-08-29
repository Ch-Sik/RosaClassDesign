using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBase : MonoBehaviour
{
    [SerializeField] private int maxHP;
    private int currentHP;

    private void Start()
    {
        currentHP = maxHP;
    }

    void TakeDamage()
    {
        
    }

    void Die()
    {

    }

    void DropItems()
    {

    }
}
