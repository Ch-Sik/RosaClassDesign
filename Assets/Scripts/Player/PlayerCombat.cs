using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

/// <summary>
/// ButterFlyAction을 위해서라도, 얘는 최종부모에 넣어두자.
/// </summary>

public class PlayerCombat : MonoBehaviour
{
    public bool showGizmo = false;                  //기즈모 가시 여부

    [System.Serializable]
    public class ComboAttack
    {
        public GameObject attackEntity;             // 공격 오브젝트
        public PlayerDamageInflictor attackHandler; // 공격 핸들러

        [Header("Combat Properties")]
        public float attackStartupTime;               // 공격 선딜레이
        public float attackActiveTime;              // 공격 유지시간
        public float attackRecoveryTime;            // 공격 후딜레이
        public float attackDistance;                // 공격 사거리
        public float attackDamagePercent;           // 공격 데미지 비중
        public void SetAttackHandler(GameObject entity)
        {
            attackHandler = entity.GetComponentInChildren<PlayerDamageInflictor>();
        }
    }

    [Title("컴포넌트 레퍼런스")]
    [ReadOnly, SerializeField] PlayerController playerControl;
    [ReadOnly, SerializeField] PlayerRef playerRef;

    [Title("플래그")]
    public bool isDoingAttack = false;              //공격중이라면 true, 아니라면 false;
    public bool isRidingButterfly = false;          //나비를 타고 있다면 true, 아니라면 false;
    public bool canInteraction = true;              //좌클릭으로 인터렉션 가능하면 true, 아니라면 false //공격시 범위 내에 적이 있다면 false

    [Title("레이어 옵션")]
    public LayerMask mask_wall;
    public LayerMask mask_attackable;             //공격가능한 대상 레이어 마스크
    public LayerMask mask_butterfly;                     //나비 레이어 마스크

    [Title("콤보 옵션")]
    public List<ComboAttack> comboAttacks = new List<ComboAttack>();              // 콤보 공격 배열
    public float comboResetTime = 1.0f;             // 콤보 리셋 시간
    [ReadOnly, SerializeField] public int comboStep = 0;                      // 현재 콤보 단계
    [ReadOnly, SerializeField] private float lastAttackTime;                   // 마지막 공격 시간

    // 마우스 관련 변수
    private Vector2 mouse;                          // 마우스 월드 좌표
    private float angle;                            // 마우스의 Euler Angle
    private Vector2 direction;                      // 마우스 방향벡터
    private InputAction aimInput;

    // 공격 버튼 관련
    public bool attackTrigger = false;              // 공격 버튼

    public void SetAttackTrigger(bool value)
    {
        attackTrigger = value;
    }

    //시작하면서 AttackEntity에 존재하는 attackObject를 얻어오며, attackObject를 Init해준다.
    private void Start()
    {
        // 필드 초기화
        playerRef = PlayerRef.Instance;
        playerControl = playerRef.Controller;
        aimInput = InputManager.Instance._inputAsset.FindActionMap("ActionDefault").FindAction("Aim");

        // 공격 오브젝트가 flip에 영향받지 않도록 하기 위해 게임이 시작되면 공격을 플레이어의 자식이 아니도록 설정
        for (int i = 0; i < comboAttacks.Count; i++)
        {
            if (comboAttacks[i].attackEntity == null)
            {
                Debug.LogError($"Combo attack entity at index {i} is not set.");
                continue;
            }
            comboAttacks[i].attackEntity.transform.SetParent(null);
            comboAttacks[i].attackEntity.SetActive(false);

            comboAttacks[i].SetAttackHandler(comboAttacks[i].attackEntity);
            if (comboAttacks[i].attackHandler == null)
            {
                Debug.LogError($"PlayerDamageInflictor not found in attack entity at index {i}.");
                continue;
            }
            comboAttacks[i].attackHandler.Init(this, comboAttacks[i].attackDamagePercent, LayerMask.GetMask("Wall"), LayerMask.GetMask("Attackable"), LayerMask.GetMask("Butterfly"));
        }
    }

    private void FixedUpdate()
    {
        // 공격 시전 가능한지 체크
        if(attackTrigger && CheckAttackable())
        {
            // 가능하다면 공격 시전
            Attack();
        }
    }

    private bool CheckAttackable()
    {
        return !isDoingAttack;
    }

    //공격 함수
    public void Attack()
    {
        // 플래그 설정
        isDoingAttack = true;

        // 마우스 위치 계산
        CalculateAttackDirection();

        // 콤보 계산
        if (Time.time - lastAttackTime > comboResetTime)
        {
            comboStep = 0;
        }
        else
        {
            comboStep = (comboStep + 1) % comboAttacks.Count;
        }
        lastAttackTime = Time.time;
        ComboAttack currentCombo = comboAttacks[comboStep];
        GameObject currentAttackEntity = currentCombo.attackEntity;
        PlayerDamageInflictor currentAttackHandler = currentCombo.attackHandler;

        // 공격 시퀀스
        Sequence attack = DOTween.Sequence()
        // 시작 즉시
        .AppendCallback(() =>
        {
            currentAttackEntity.SetActive(true);
            currentAttackEntity.transform.position = this.transform.position;
            currentAttackEntity.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            currentAttackHandler.transform.localPosition = Vector3.zero;
        })
        // 선딜 시간 적용
        .AppendInterval(currentCombo.attackStartupTime)
        // 선딜 끝나면 이벤트와 함께, 공격 판정 ON
        .AppendCallback(() =>
        {
            OnStartAttack();
            currentAttackHandler.SetAttackActive(angle);
        })
        // 공격 판정 발사
        .Append(currentAttackHandler.transform.DOMove(currentCombo.attackDistance * direction, currentCombo.attackActiveTime).SetRelative(true))
        // 공격이 끝난다면, 공격판정 회수
        .AppendCallback(() => {
            currentAttackHandler.transform.position = transform.position;
            currentAttackHandler.SetAttackInactive();
            currentAttackEntity.SetActive(false);
        })
        // 후딜 시간 적용
        .AppendInterval(currentCombo.attackRecoveryTime)
        // 시퀀스가 끝나며, 공격끝 이벤트와 함께 공격판정 OFF
        .OnComplete(() =>
        {
            OnEndAttack();
        });
    }

    //SetData는 호출되면, 현재의 마우스 위치를 토대로 각도와 방향벡터 Data를 Set해준다.
    void CalculateAttackDirection()
    {
        Vector2 mouseScreenPos2 = aimInput.ReadValue<Vector2>();
        float zDistance = transform.position.z - Camera.main.transform.position.z; // 플레이어와 카메라의 z좌표 차이
        mouse = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos2.x, mouseScreenPos2.y, zDistance));   //마우스의 월드 좌표 반환
        angle = Mathf.Atan2(mouse.y - transform.position.y, mouse.x - transform.position.x) * Mathf.Rad2Deg;    //해당 좌표 데이터를 기반으로 각도 얻음
        direction = new Vector2(mouse.x - transform.position.x, mouse.y - transform.position.y).normalized;     //해당 좌표 데이터를 기반으로 방향벡터 얻음
    }

    [Button]
    //플레이어가 바라보는 방향과 공격방향이 동일한지를 따진다.
    public bool IsPlayerLookAttackDirection(float angle)
    {
        if (transform.localScale.x > 0 && Mathf.Abs(angle) < 90)
            return true;
        else if (transform.localScale.x < 0 && Mathf.Abs(angle) > 90)
            return true;
        else
            return false;
    }

    [Button]
    //공격을 취소하기 위해 만듦. 공격판정체가 나비에 닿거나 임의로 공격을 중단시킬 경우 사용한다.
    public void StopAttack()
    {
        // 공격 판정이 어차피 비활성화된 상태면 계속 움직여도 딱히 상관 없음.
        // attack.Pause();
        // attackEntity.transform.localPosition = Vector2.zero;

        // 공격 판정 off
        foreach (var combo in comboAttacks)
        {
            combo.attackHandler.SetAttackInactive();
        }

        // 적에게 공격이 닿았다고 공격 이펙트/후딜레이가 없어지면 안됨.
        // OnEndAttack();
    }

    //공격 시작 이벤트
    private void OnStartAttack()
    {
        // 애니메이션 트리거 설정
        playerRef.Animation.SetAttackAnimTrigger();
    }

    //공격 종료 이벤트
    private void OnEndAttack()
    {
        // 이펙트 & 공격 판정 오브젝트 비활성화
        if(!attackTrigger) 
            playerControl.ChangeMoveState(PlayerMoveState.DEFAULT);
        isDoingAttack = false;
        canInteraction = true;
    }

    //추후에 공격시 대미지나 공격력을 계산하기 위해 만들었다. AttackObejct에서 충돌판정부에서 사용하자.
    public void CombatCalcultor(GameObject target)
    {
    }

    //AttackObject가 나비에 닿을 경우, 해당 나비데이터를 PlayerCombat에 전달해주고, 그 데이터를 나비에 전달하기 위한 함수
    public void RideButterFly(Butterfly butterFly)
    {
        if (isRidingButterfly)
            return;

        StopAttack();
        butterFly.ButterFlyAct(transform);
    }

    private void OnDrawGizmos()
    {
        if (!showGizmo)
            return;

        Vector3 direction = new Vector2(mouse.x - transform.position.x, mouse.y - transform.position.y).normalized;

        // 기준점에서 직사각형의 중심을 계산
        Vector3 center = transform.position + direction * (comboAttacks[comboStep].attackDistance / 2);

        // 직사각형의 회전 각도 계산
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 기즈모에서 그린 영역과 겹치는 충돌체 검사
        canInteraction = !Physics2D.OverlapBox(center, new Vector2(comboAttacks[comboStep].attackDistance, 1), angle, LayerMask.GetMask("Monster"));
        // 기즈모 색상 설정
        Gizmos.color = canInteraction ? Color.green : Color.red;

        // 회전된 직사각형 그리기
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(center, Quaternion.Euler(0, 0, angle), new Vector3(comboAttacks[comboStep].attackDistance, 1, 1));
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one); // 회전된 직사각형 그리기
        Gizmos.matrix = Matrix4x4.identity; // 다음 Gizmo에 영향을 미치지 않도록 기본 매트릭스로 돌아가기
    }
}