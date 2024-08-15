using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // public 필드
    public PlayerMoveState currentMoveState
    {
        get { return inputManager._moveState; }
    }

    public PlayerActionState currentActionState
    {
        get { return inputManager._actionState; }
    }

    public PlayerCubeActionState currentCubeActionState = PlayerCubeActionState.DEFAULT;

    // 컴포넌트
    [ReadOnly, SerializeField] public InputActionAsset inputAsset;
    [ReadOnly, SerializeField] PlayerMovement playerMove;
    [ReadOnly, SerializeField] PlayerCombat playerCombat;
    [ReadOnly, SerializeField] Rigidbody2D rb;
    [ReadOnly, SerializeField] PlayerAnimation playerAnim;
    private UnityEvent interactFunction = new UnityEvent();

    // private 변수
    private Vector2 moveVector = Vector2.zero;      // 하향점프 판단을 위해 값 보관

    //싱글톤
    [ReadOnly, SerializeField] PlayerRef playerRef;
    InputManager inputManager;
    #region 초기화
    // Start is called before the first frame update
    void Start()
    {
        InitFields();
        RegisterInputEventHandler();
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

    private void RegisterInputEventHandler()
    {
        inputAsset = inputManager._inputAsset;

        // MoveDefault 액션맵 바인딩
        inputManager.AM_MoveDefault.FindAction("Move").performed += OnMove;
        inputManager.AM_MoveDefault.FindAction("Move").canceled += OnCancelMove;
        inputManager.AM_MoveDefault.FindAction("Jump").performed += OnJump;
        inputManager.AM_MoveDefault.FindAction("Jump").canceled += OnCancelJump;
        inputManager.AM_MoveDefault.FindAction("Interact").performed += OnInteract;
        inputManager.AM_MoveDefault.FindAction("Gliding").performed += OnGliding;
        inputManager.AM_MoveDefault.FindAction("Gliding").canceled += OnCancleGliding;


        // Climb 액션맵 바인딩
        inputManager.AM_MoveClimb.FindAction("Move").performed += OnClimbMove;
        inputManager.AM_MoveClimb.FindAction("Move").canceled += OnCancelClimbMove;
        inputManager.AM_MoveClimb.FindAction("Jump").performed += OnClimbJump;
        inputManager.AM_MoveClimb.FindAction("Jump").canceled += OnCancelJump;

        // SuperDash 관련 액션맵 바인딩
        inputManager.AM_MoveSuperDashReady.FindAction("Launch").performed += OnSuperDashLaunch;
        inputManager.AM_MoveSuperDashReady.FindAction("Cancel").performed += OnSuperDashCancelBeforeLaunch;
        inputManager.AM_MoveSuperDash.FindAction("Cancel").performed += OnSuperDashCancelAfterLaunch;
        inputManager.AM_MoveSuperDash.FindAction("Move").performed += OnSuperDashMoveAfterLaunch;

        // ActionDefault 액션맵 바인딩
        inputManager.AM_ActionDefault.FindAction("Attack").performed += OnMeleeAttack;
        inputManager.AM_ActionDefault.FindAction("Attack").canceled += OnCancelMeleeAttack;
        inputManager.AM_ActionDefault.FindAction("MagicSelect").performed += OnMagicSelect;
        inputManager.AM_ActionDefault.FindAction("MagicReady").performed += OnMagicReady;

        // MagicReady 액션맵 바인딩
        inputManager.AM_ActionMagicReady.FindAction("MagicSelect").performed += OnMagicSelect;
        inputManager.AM_ActionMagicReady.FindAction("MagicExecute").performed += OnMagicExecute;
        inputManager.AM_ActionMagicReady.FindAction("MagicCancel").performed += OnMagicCancel;

        // UI 관련 input action은 PlayerController에서 관리하지 않음.
    }
    #endregion

    // PlayerController.ChangeMoveState를 참조하는 게 많아서 프록시(?)를 두긴 했는데
    // 가능하면 inputManager.ChangeMoveState를 사용할 것.
    public void SetMoveState(PlayerMoveState newMoveState)
    {
        inputManager.SetMoveInputState(newMoveState);
    }

    public void SetActionState(PlayerActionState newActionState)
    {
        inputManager.SetActionInputState(newActionState);
    }

    #region InputAction 이벤트 핸들러
    #region PlayerMove ActionMap 핸들러
    public void OnMove(InputAction.CallbackContext context)
    {
        moveVector = context.ReadValue<Vector2>();
        // 지상, 또는 공중에 있을 경우 좌우 이동

        playerMove.Walk(moveVector);
    }

    // Grounded에서 다른 상태로 넘어갔을 때 moveVector 초기화용
    public void OnCancelMove(InputAction.CallbackContext context)
    {
        moveVector = context.ReadValue<Vector2>();
        // 무브 캔슬
        playerMove.Walk(moveVector);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        playerMove.OnJump(moveVector.y < -float.Epsilon);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        //특정 상태에서 인터렉트 안 할 시 이곳에서 처리
        if (interactFunction == null)
            return;
        interactFunction.Invoke();
    }

    public void OnGliding(InputAction.CallbackContext context)
    {
        playerMove.Gliding();
    }

    public void OnCancleGliding(InputAction.CallbackContext context)
    {
        playerMove.CancleGliding();
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
        playerMove.StopClimb(moveVector);
    }

    public void OnCancelJump(InputAction.CallbackContext context)
    {
        playerMove.FinishJumpUp();        // 점프 종료
    }

    public void OnClimbJump(InputAction.CallbackContext context)
    {
        playerMove.WallJump(); // 벽점프
    }

    public void OnAim(InputAction.CallbackContext context)
    {

    }

    public void OnSuperDashLaunch(InputAction.CallbackContext context)
    {
        playerMove.LaunchSuperDash(context.ReadValue<float>() < 0 ? LR.LEFT : LR.RIGHT);
    }

    public void OnSuperDashCancelBeforeLaunch(InputAction.CallbackContext context)
    { }

    public void OnSuperDashCancelAfterLaunch(InputAction.CallbackContext context)
    { }

    public void OnSuperDashMoveAfterLaunch(InputAction.CallbackContext context)
    {
        playerMove.OnMoveDuringSuperDash(context.ReadValue<Vector2>().toLR());
    }
    #endregion
    #region PlayerAction ActionMap 핸들러
    public void OnMeleeAttack(InputAction.CallbackContext context)
    {
        if (playerMove.isGrabCube)
            PlayerRef.Instance.GrabCube.UnGrab();

        if (currentMoveState != PlayerMoveState.CLIMBING)
        {
            //if(!playerCombat.isAttack) playerAnim.SetTrigger("Attack");
            playerCombat.SetAttackTrigger(true);
        }
        
        /*
        if (context.performed)
        {
            if (currentMoveState != PlayerMoveState.CLIMBING)
            {
                PlayerRef.Instance.combat.Attack();
                playerAnim.SetTrigger("Attack");
            }
        }
        */
    }

    public void OnCancelMeleeAttack(InputAction.CallbackContext context)
    {
        playerCombat.SetAttackTrigger(false);
    }

    public void OnMagicSelect(InputAction.CallbackContext context)
    {
        PlayerRef.Instance.Magic.SelectMagic();
    }

    public void OnMagicReady(InputAction.CallbackContext context)
    {
        PlayerRef.Instance.Magic.ReadyMagic();
    }

    public void OnMagicExecute(InputAction.CallbackContext context)
    {
        PlayerRef.Instance.Magic.ExecuteMagic();
    }

    public void OnMagicCancel(InputAction.CallbackContext context)
    {
        PlayerRef.Instance.Magic.CancelMagic();
    }
    #endregion
    #endregion

    #region Interaction관련
    public void ResetInteraction(bool isGrabAction = false)
    {
        if (playerMove.isGrabCube && !isGrabAction)
            return;

        if (interactFunction != null)
            interactFunction.RemoveAllListeners();
    }

    public void SetInteraction(UnityAction action, bool isGrabAction = false)
    {
        if (playerMove.isGrabCube && !isGrabAction)
            return;

        interactFunction.AddListener(action);
    }
    #endregion
}