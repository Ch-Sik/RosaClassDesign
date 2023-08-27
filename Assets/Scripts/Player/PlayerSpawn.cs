using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player 스포너
/// ※ 플레이어 스폰은 Awake보다 뒤에 수행되어야 함 (Start 등에서) 
/// § 사유: Player가 일찍 스폰되어버리면 PlayerState가 아직 초기화되지 않은 상태의 PlayerStateUI를 참조할 수 있음
/// </summary>
public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;

    void SpawnPlayer(Vector2 position)
    {
        var playerObject = GameObject.Instantiate(playerPrefab, position, Quaternion.identity);
        InputManager.GetInstance().player = playerObject.GetComponent<PlayerRef>();
    }
}
