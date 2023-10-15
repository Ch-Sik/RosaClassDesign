using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [ReadOnly, SerializeField] private PlayerMoveState currentState;

    private InputAction moveAction;
    // public 필드
    public PlayerMoveState MoveState
    {
        get { return currentState; }
        set { if (value != currentState) Debug.Log(value); currentState = value; }
    }

    // 컴포넌트
    [ReadOnly, SerializeField] public InputActionAsset inputAsset;
    [ReadOnly, SerializeField] PlayerMovement playerMove;
    [ReadOnly, SerializeField] PlayerCombat playerCombat;
    [ReadOnly, SerializeField] Rigidbody2D rb;
    [ReadOnly, SerializeField] PlayerAnimation playerAnim;

    // private 변수
    [SerializeField, ReadOnly]
    //private PlayerMoveState state = PlayerMoveState.GROUNDED;
    private Vector2 moveVector = Vector2.zero;      // 하향점프 판단을 위해 값 보관
    private GameObject platformBelow;

    // 플래그
    private bool isJumpingUp = false;
    private bool isDoingMagic = false;

    //싱글톤
    [ReadOnly, SerializeField] PlayerRef playerRef;
    InputManager inputManager;
    #region 초기화
    // Start is called before the first frame update
    void Start()
    {
        InitFields();
        InitInput();
    }


    private void InitFields()
    {
        playerRef = PlayerRef.Instance;
        inputManager = InputManager.Instance;
        playerMove = playerRef.Move;
        playerCombat = playerRef.combat;
        playerAnim = playerRef.Animation;
        rb = playerRef.rb;
        
    }

    private void InitInput()
    {
        inputAsset = inputManager._inputAsset;

        //Walk 상태 바인딩
        inputManager.playerWalkActionMap.FindAction("Walk").performed += OnMove;
        inputManager.playerWalkActionMap.FindAction("Walk").canceled += OnCancelMove;
        inputManager.playerWalkActionMap.FindAction("Jump").performed += OnJump;
        inputManager.playerWalkActionMap.FindAction("Jump").canceled += OnCancelJump;
        inputManager.playerWalkActionMap.FindAction("Attack").performed += OnMeleeAttack;

        //Climb 상태 바인딩
        inputManager.playerCLIMBActionMap.FindAction("Walk").performed += OnClimbMove;
        inputManager.playerCLIMBActionMap.FindAction("Walk").canceled += OnCancelClimbMove;
        inputManager.playerCLIMBActionMap.FindAction("Jump").performed += OnClimbJump;
        inputManager.playerCLIMBActionMap.FindAction("Jump").canceled += OnCancelJump;

        // inputAsset 내 다른 액션맵에 있는 액션도 바인딩 해야 함

    }
    #endregion

    public void ChangeMoveState(PlayerMoveState newMoveState)
    {
        if (newMoveState == currentState) { return; }
        currentState = newMoveState;
        switch (currentState)
        {
            case PlayerMoveState.WALK:
                InputManager.Instance.ChangeInputState(InputState.PLAYER_WALK);
                break;
            case PlayerMoveState.MIDAIR:
                //MIDAIR 상태에서 키바인딩은 WALK 상태와 다르지 않기 때문에 WALK
                InputManager.Instance.ChangeInputState(InputState.PLAYER_WALK);
                break;
            case PlayerMoveState.CLIMBING:
                InputManager.Instance.ChangeInputState(InputState.PLAYER_CLIMB);
                break;
            case PlayerMoveState.MONKEY:
                InputManager.Instance.ChangeInputState(InputState.PLAYER_MONKEY);
                break;
            case PlayerMoveState.CANNOTMOVE:
                InputManager.Instance.ChangeInputState(InputState.IGNORE);
                break;
        }
    }

    #region InputAction 이벤트 핸들러
    public void OnMove(InputAction.CallbackContext context)
    {
        moveVector = context.ReadValue<Vector2>();
         // 지상, 또는 공중에 있을 경우 좌우 이동
        playerMove.Walk(moveVector);
    }

    public void OnCancelMove(InputAction.CallbackContext context)
    {
        moveVector = context.ReadValue<Vector2>();
        // 무브 캔슬
        playerMove.Walk(moveVector);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        switch (currentState)
        {
            case PlayerMoveState.WALK:
                if (moveVector.y >= 0 || !(platformBelow?.CompareTag("Platform") == true))
                {
                    isJumpingUp = true;
                    playerMove.JumpUp();        // 상향 점프
                }
                else
                {
                    playerMove.JumpDown();      // 하향 점프
                }
                break;
            case PlayerMoveState.MIDAIR:
                if (playerMove.isDoingHooking && playerMove.isHitHookingTarget)
                {
                    playerMove.JumpUp();
                }
                playerMove.ReserveJump();       // 공중에서는 점프 선입력
                break;
        }
        
    }

    public void OnClimbMove(InputAction.CallbackContext context)
    {
        moveVector = context.ReadValue<Vector2>();
        // 벽에 붙어있을 경우 기어오르기/내리기
        playerMove.Climb(moveVector);
    }

    public void OnCancelClimbMove(InputAction.CallbackContext context)
    {
        moveVector = context.ReadValue<Vector2>();
        // 무브 캔슬
        playerMove.Climb(moveVector);
    }

    public void OnCancelJump(InputAction.CallbackContext context)
    {
        if (isJumpingUp)
        {
                isJumpingUp = false;
                playerMove.FinishJumpUp();        // 점프 종료
        }
    }

    public void OnClimbJump(InputAction.CallbackContext context)
    {
        playerMove.WallJump(); // 벽점프
    }

    public void OnAim(InputAction.CallbackContext context)
    {

    }

    public void OnMeleeAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (currentState != PlayerMoveState.CLIMBING)
            {
                //anim.SetTrigger("MeleeAttackTrigger");
                //playerCombat.MeleeAttack();
            }
        }
    }

    /*
    public void OnRangeAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (state != PlayerMoveState.CLIMBING)
            {
                anim.SetTrigger("RangeAttackTrigger");
                playerCombat.RangeAttack();
            }

        }
    }
    */

    public void OnInteract(InputAction.CallbackContext context)
    { }

    public void OnMagic(InputAction.CallbackContext context)
    { }
    #endregion

    #region 그라운드 체크. PlayerGroundCheck.cs에서 호출되는 함수들
    public void SetIsGrounded(GameObject belowObject)
    {
        //해당 값이 0일 경우, 플랫폼에 평행하게 진입할 경우 바로 리턴되는 아래와 같은 문제가 있었음.
        //1. 이동하면서 더블점프하면 점프가 안 되는 버그
        //2. A방향으로 더블점프를 착지 전에 -A 방향으로 전환 시 점프 리셋이 안 됌.
        if (rb.velocity.y > 0.1f) return; // 1-way platform의 groundcheck 방지
        if (currentState == PlayerMoveState.CLIMBING) return; // climb 중 groundcheck 방지


        platformBelow = belowObject;
        if (currentState != PlayerMoveState.WALK)
        {
            // 막 착지했을 때
            playerMove.OnLanded();
        }
        ChangeMoveState(PlayerMoveState.WALK);
    }

    public void SetIsNotGrounded()
    {
        Debug.Log("Exited");
        platformBelow = null;
        if (currentState == PlayerMoveState.WALK)
            ChangeMoveState(PlayerMoveState.MIDAIR);
        // state가 Climbing일 경우 state를 수정하지 않음.
    }
    #endregion
}
