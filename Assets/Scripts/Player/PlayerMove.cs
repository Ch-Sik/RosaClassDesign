using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// 플레이어의 움직임 담당.
/// </summary>
public class PlayerMove : MonoBehaviour
{
    public InputActionAsset inputAsset;

    private Rigidbody2D rb;

    private PlayerMoveState currentState;

    
    private InputAction moveAction;
    [SerializeField] private Vector2 moveVector;


    private void Awake()
    {
        InitInput();
    }

    private void InitInput()
    {
        inputAsset.Enable();        // 모든 입력 활성화
        moveAction = inputAsset.FindAction("Walk");         // 매 프레임 값을 읽어오는 방식
        inputAsset.FindAction("Jump").performed += OnJump;  // 콜백함수를 등록하는 방식
    }

    public void Update()    // FixedUpdate여야 했었던가?
    {
        moveVector = moveAction.ReadValue<Vector2>();
        // rb.velocity = new Vector2(moveVector.x, rb.velocity.y);
    }

    private void ChangeMoveState(PlayerMoveState newMoveState)
    {
        if(newMoveState == currentState) { return; }
        currentState = newMoveState;
        switch(currentState)
        {
            case PlayerMoveState.WALK:
                InputManager.Instance.ChangeInputState(InputState.PLAYER_WALK);
                break;
            case PlayerMoveState.CLIMB:
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

    public void OnJump(InputAction.CallbackContext context)
    {
        // 점프 수행
        Debug.Log("Jump");
    }
}