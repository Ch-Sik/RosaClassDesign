using System.Collections;
using System.Collections.Generic;
using UnityEditor.AssetImporters;
using UnityEngine;

/// <summary>
/// 처음 스폰되었을 때에는 공격판정이 없고 공격 범위 표시의 역할만 하다가,
/// ExecuteAttack이 호출되면 비로소 스프라이트가 바뀌면서 공격판정이 생김
/// </summary>
public class MonsterAOE : MonoBehaviour
{
    [SerializeField]
    private new Collider2D collider;
    [SerializeField]
    private new SpriteRenderer renderer;

    public void Init()
    {
        if(collider == null)
        {
            collider = GetComponent<Collider2D>();
            Debug.Assert(collider != null, $"{gameObject.name}: Collider2D를 찾을 수 없음");
        }
        collider.enabled = false;
    }

    public void ExecuteAttack()
    {
        Debug.Log("범위 공격 수행");
        collider.enabled = true;
        renderer.color = new Color(0.6f, 1, 0.6f);
        Destroy(gameObject, 1f);
    }

    public void CancelAttack()
    {
        Debug.Log("공격 취소");
        Destroy(gameObject);
    }
}
