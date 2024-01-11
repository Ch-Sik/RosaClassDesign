using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 처음 스폰되었을 때에는 공격판정이 없고 공격 범위 표시의 역할만 하다가,
/// ExecuteAttack이 호출되면 비로소 스프라이트가 바뀌면서 공격판정이 생김
/// </summary>
public class MonsterAOE : MonoBehaviour
{
    public void Init()
    {
        
    }

    public void ExecuteAttack()
    {
        Debug.Log("범위 공격 수행");
        Destroy(gameObject, 1f);
    }

    public void CancelAttack()
    {
        Debug.Log("공격 취소");
        Destroy(gameObject);
    }
}
