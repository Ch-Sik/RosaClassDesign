using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.UIElements;

public class PlayerCombat : MonoBehaviour
{
    public bool showGizmo = false;

    Sequence attack;
    public GameObject attackEntity;
    public AttackObject attackObject;
    public bool isAttack = false;

    [Header("CombatOptions")]
    public LayerMask attackableObjects;
    public LayerMask butterfly;

    [Header("Combat Properties")]
    public float attackDistance;
    public float attackTime;
    public float attackCooltime;

    public float angle;
    [HideInInspector] public Vector2 mouse;
    public Vector2 direction;

    private void Start()
    {
        attackObject = attackEntity.GetComponent<AttackObject>();
        attackObject.Init(this, attackableObjects, butterfly);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    void SetData()
    {
        mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        angle = Mathf.Atan2(mouse.y - transform.position.y, mouse.x - transform.position.x) * Mathf.Rad2Deg;
        direction = new Vector2(mouse.x - transform.position.x, mouse.y - transform.position.y).normalized;
    }

    public void Attack()
    {
        if (isAttack)
            return;

        SetData();

        attack = DOTween.Sequence()
        .AppendCallback(() =>
        {
            OnStartAttack();
            attackEntity.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            attackObject.StartAttack();
        })
        .Append(attackEntity.transform.DOLocalMove(attackDistance * direction, attackTime))
        .AppendCallback(() => attackEntity.transform.localPosition = Vector2.zero)
        .OnComplete(() =>
        {
            attackObject.EndAttack();
            OnEndAttack();
        });
    }

    [Button]
    public void StopAttack()
    {
        attack.Pause();
        attackEntity.transform.localPosition = Vector2.zero;
        attackObject.EndAttack();
        OnEndAttack();
    }

    private void OnStartAttack()
    {
        isAttack = true;
        Debug.Log("Start Attack");
    }

    private void OnEndAttack()
    {
        isAttack = false;
        //쿨타임은 여기 넣자.
        Debug.Log("End Attack");
    }

    public void CombatCalcultor(GameObject target)
    { 
    }

    private void OnDrawGizmos()
    {
        if (!showGizmo)
            return;


        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // 마우스 위치를 월드 좌표로 변환

        Vector3 direction = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y).normalized;

        // 기준점에서 직사각형의 중심을 계산
        Vector3 center = transform.position + direction * (attackDistance / 2);

        // 직사각형의 회전 각도 계산
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 회전된 직사각형 그리기
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(center, Quaternion.Euler(0, 0, angle), new Vector3(attackDistance, 1, 1));
        Gizmos.matrix = rotationMatrix;
        Gizmos.color = Color.red; // 직사각형 색상 설정
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one); // 회전된 직사각형 그리기
        Gizmos.matrix = Matrix4x4.identity; // 다음 Gizmo에 영향을 미치지 않도록 기본 매트릭스로 돌아가기
    }
}