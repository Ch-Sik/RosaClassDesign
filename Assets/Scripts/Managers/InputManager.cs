using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InputState
{
    PLAYERMOVE,
    UICONTROL,
    IGNORE /* 레어 아이템 획득 연출 등 잠시동안 모든 입력이 무시되어야 하는 경우.
            * IGNORE 상태는 오래 유지하지 말 것. */
}

/// <summary>
/// Unity에서 제공하는 InputManager를 사용하기 편하게 InputState로 추상화합니다.
/// InputState에 해당하는 InputActionMap들만 활성화하고 나머지는 비활성화합니다.
/// 
/// </summary>
public class InputManager : MonoBehaviour
{
    // 싱글톤
    static InputManager instance;

    // 컴포넌트
    public PlayerRef player { get { return player; } set { player = value; playerMove = value.Move; } }
    public PlayerMove playerMove;
    public PlayerInput unityPlayerInput;

    // 에셋
    [SerializeField] InputActionAsset inputActions;
    private InputActionMap playerInputActions;
    private InputActionMap uiInputActions;
    
    // 상태
    [ContextMenuItem("switch inputState", "Test")]
    public InputState state;


    public static InputManager GetInstance() { return instance; }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // InputActionAsset에서 ActionMap(Action 묶음) 찾기
        playerInputActions = inputActions.FindActionMap("Player");
        uiInputActions = inputActions.FindActionMap("UI");
    }

    void ChangeInputState(InputState newState)
    {
        Debug.Log("ChangeInputState");
        if (state == newState) return;
        if(state == InputState.PLAYERMOVE)
        {
        //    playerMove.OnMove(Vector2.zero); // inputVector가 (1,0) 따위에서 업데이트 안되는 현상 방지
        }
        state = newState;

        if (state == InputState.PLAYERMOVE)
        {
            //unityPlayerInput.SwitchCurrentActionMap("Player");   // Player 액션맵은 활성화하고 나머지는 비활성화
            // 참고: CurrentActionMap은 PlayerInput 컴포넌트가 혼자 가지고 있는 개념임. ActionMapAsset 자체는 변하지 않음.
            playerInputActions.Enable();   
            uiInputActions.Disable();        
        }
        else if (state == InputState.UICONTROL)
        {
            //unityPlayerInput.SwitchCurrentActionMap("UI");
            playerInputActions.Disable();   // 액션맵의 활성화/비활성화를 수동으로 설정할 수도 있음.
            uiInputActions.Enable();        // 이 방식의 경우 2개 이상의 액션맵을 동시에 활성화 가능
        }
    }

    public void Test()
    {
        if (state == InputState.PLAYERMOVE)
            ChangeInputState(InputState.UICONTROL);
        else
            ChangeInputState(InputState.PLAYERMOVE);
    }
}

