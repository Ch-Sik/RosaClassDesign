using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RespawnManager : MonoBehaviour
{
    private static RespawnManager _instance = null;

    public static RespawnManager Instance { get { return _instance; } }

    [SerializeField] Vector2Int respawnPoint;
    [SerializeField] GameObject player;

    public int healAmount = 2;
    private void Awake()
    {
        _instance = this; 
    }

    private void Update()
    {
        if (MapManager.Instance.room == null)
            return;

        Vector2Int curPosition = new Vector2Int((int)player.transform.position.x,
                                                (int)player.transform.position.y - 1);

        if (!MapManager.Instance.room.safePositions.Contains(curPosition))
            return;

        SwitchRespawnPoint(curPosition);
    }

    public void SwitchRespawnPoint(Vector2Int respawnPoint)
    {
        this.respawnPoint = respawnPoint;
    }

    [Button]
    public void Respawn()
    {
        player.transform.position = new Vector3(respawnPoint.x, respawnPoint.y, player.transform.position.z);
        //PlayerRef.Instance.State.Heal(healAmount);
    }
}
