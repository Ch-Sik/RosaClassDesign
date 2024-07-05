using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnSector : MonoBehaviour
{
    private RespawnManager respawnManager;

    [SerializeField] Transform respawnPoint;

    private void Start()
    {
        respawnManager = RespawnManager.Instance;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            respawnManager.SwitchRespawnPoint(respawnPoint);
        }
    }
}
