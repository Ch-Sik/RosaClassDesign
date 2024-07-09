using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 몬스터용 지면 체크
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class MonsterGroundCheck : MonoBehaviour
{
    [SerializeField]
    Blackboard blackboard;

    [SerializeField, ReadOnly]
    private int overlappingCollider = 0;
    [SerializeField, ReadOnly]
    private bool isGrounded = false;

    private void Start()
    {
        if (blackboard == null)
        {
            blackboard = transform.parent.GetComponent<Blackboard>();
            if (blackboard == null)
                Debug.LogError($"{gameObject.name}: Blackboard를 찾을 수 없음!");
        }
        overlappingCollider = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 공통의 부모를 가지고 있는 경우(=Parent Constraint로 묶여 있는경우) 무시함.
        if (collision.gameObject.transform.parent == transform.parent)
            return;

        if (overlappingCollider <= 0)
        {
            isGrounded = true;
            blackboard.Set(BBK.isGrounded, isGrounded);
            // Debug.Log($"isGrounded: {isGrounded}");
        }
        overlappingCollider++;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 공통의 부모를 가지고 있는 경우(=Parent Constraint로 묶여 있는경우) 무시함.
        if (collision.gameObject.transform.parent == transform.parent)
            return;

        overlappingCollider--;
        if (overlappingCollider <= 0)
        {
            isGrounded = false;
            blackboard.Set(BBK.isGrounded, isGrounded);
            // Debug.Log($"isGrounded: {isGrounded}");
        }
        if (overlappingCollider < 0)     // 오류 감지
        {
            Debug.LogError($"{gameObject.name}: Overlapping Collider가 0개 미만임!");
        }
    }
}
