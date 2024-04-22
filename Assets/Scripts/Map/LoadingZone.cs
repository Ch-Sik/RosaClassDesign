using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어와의 충돌을 감지하여, 충돌시 맵로더에게 이벤트 제출
/// </summary>

public class LoadingZone : MonoBehaviour
{
    GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player)
            gameObject.GetComponentInParent<MapLoader>()?.Loading();
    }
}
