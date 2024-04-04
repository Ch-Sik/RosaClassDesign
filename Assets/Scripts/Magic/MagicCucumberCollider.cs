using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCucumberCollider : MonoBehaviour
{
    private MagicCucumber cucumber;

    public Collider2D trigger;

    private const string _playerTag = "Player";
    
    private void Start()
    {
        cucumber = GetComponentInParent<MagicCucumber>();
        trigger = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(_playerTag)) return;

        cucumber.CucumberDash();
        trigger.enabled = false;
    }
}
