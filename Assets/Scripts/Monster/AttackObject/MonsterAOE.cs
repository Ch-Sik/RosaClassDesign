using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <br>Attack Of Range. 몬스터의 범위 공격 판정을 관리하는 스크립트</br>
/// 처음 스폰되었을 때에는 공격판정이 없고 공격 범위 표시의 역할만 하다가,
/// ExecuteAttack이 호출되면 비로소 스프라이트가 바뀌면서 공격판정이 생김
/// </summary>
public class MonsterAOE : MonoBehaviour
{
    [SerializeField]
    private new Collider2D collider;
    [SerializeField]
    private new SpriteRenderer renderer;
    [SerializeField, Tooltip("공격이 완료/취소되었을 때 참이면 오브젝트 삭제, 거짓이면 오브젝트 비활성화")]
    private bool destroyOnAttackEnd = true;

    // 현재 디버깅용으로 공격활성화 시각화를 단순히 스프라이트 컬러 바꾸는 걸로 처리하는데 그거 처리용.
    // 공격 이펙트 제대로 적용되면 이건 삭제할 것.
    [SerializeField]
    private Color startupColor = Color.black;
    [SerializeField]
    private Color activatedColor = Color.white;

    public void Init()
    {
        gameObject.SetActive(true);
        if(collider == null)
        {
            collider = GetComponent<Collider2D>();
            Debug.Assert(collider != null, $"{gameObject.name}: Collider2D를 찾을 수 없음");
        }
        collider.enabled = false;
        // TODO: 공격 활성화의 시각화를 제대로 된 공격 이펙트로 바꾸기
        renderer.color = startupColor;
    }

    public void ExecuteAttack()
    {
        Debug.Log("범위 공격 수행");
        collider.enabled = true;
        // TODO: 공격 활성화의 시각화를 제대로 된 공격 이펙트로 바꾸기
        renderer.color = activatedColor;
        if (destroyOnAttackEnd)
            Destroy(gameObject, 1f);
        else
            StartCoroutine(SetActiveWithDelay(false, 1f));
    }

    public void CancelAttack()
    {
        Debug.Log("공격 취소");
        if (destroyOnAttackEnd)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }

    private IEnumerator SetActiveWithDelay(bool value, float delay)
    {
        yield return new WaitForSeconds( delay );
        gameObject.SetActive( value );
    }
}
