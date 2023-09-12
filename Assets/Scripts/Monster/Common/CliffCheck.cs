using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 낭떠러지 감지. 
/// 몬스터 최상단 오브젝트의 자식클래스에 부착할 것. (자손 x)
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class CliffCheck : MonoBehaviour
{
    [SerializeField]
    Blackboard blackboard;

    [SerializeField, ReadOnly]
    private int overlappingCollider = 0;
    [SerializeField, ReadOnly]
    private bool isStuckAtCliff = false;

    private void Start()
    {
        if(blackboard == null)
        {
            blackboard = transform.parent.GetComponent<Blackboard>();
            if(blackboard == null )
                Debug.LogError($"{gameObject.name}: Blackboard를 찾을 수 없음!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(overlappingCollider <= 0)
        {
            isStuckAtCliff = false;
            blackboard.Set(BBK.StuckAtCliff, isStuckAtCliff);
        }
        overlappingCollider++;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        overlappingCollider--;
        if(overlappingCollider <= 0)
        {
            isStuckAtCliff = true;
            blackboard.Set(BBK.StuckAtCliff, isStuckAtCliff);
        }
        if(overlappingCollider < 0)     // 오류 감지
        {
            Debug.LogError($"{gameObject.name}: Overlapping Collider가 0개 미만임!");
        }
    }
}
