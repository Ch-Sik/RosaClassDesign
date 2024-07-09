using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    private static RespawnManager _instance = null;

    public static RespawnManager Instance { get { return _instance; } }

    [SerializeField] Transform respawnPoint;
    [SerializeField] GameObject player;

    public int healAmount = 2;
    private void Awake()
    {
        _instance = this; 
    }

    public void SwitchRespawnPoint(Transform respawnPoint)
    {
        this.respawnPoint = respawnPoint;
    }

    public void Respawn()
    {
        //player.SetActive(false);
        player.transform.position = respawnPoint.transform.position;
        PlayerRef.Instance.State.Heal(healAmount);
    }
}
