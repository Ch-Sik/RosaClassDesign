using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InputState
{
    UICONTROL,
    DIALOGUE,
    PLAYER_WALK,
    PLAYER_CLIMB,
    PLAYER_MONKEY,
    IGNORE /* 레어 아이템 획득 연출 등 잠시동안 모든 입력이 무시되어야 하는 경우
            * .
            * IGNORE 상태는 오래 유지하지 말 것. */
}

/// <summary>
/// Unity에서 제공하는 Input System를 사용하기 편하게 InputState로 추상화.
/// 현재의 InputState에 해당하는 InputActionMap들만 활성화하고 나머지는 비활성화함.
/// 이를 통해 Player를 참조하지 않고도, UI 상태에서 Player input을 block할 수 있음.
/// </summary>
public class InputManager : MonoBehaviour
{
    // 싱글톤
    private static InputManager instance;
    //public static InputManager Instance { get { return _instance; } }

    public static InputManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<InputManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    instance = obj.AddComponent<InputManager>();
                }
            }
            return instance;
        }
    }

    // 컴포넌트
    // public PlayerRef player { get { return player; } set { player = value; playerMove = value.Move; } }
    // public PlayerMovement playerMove;

    // 에셋
    //[SerializeField] InputActionAsset inputActions;
    private InputActionMap playerInputActions;
    private InputActionMap uiInputActions;
    // InputAction Asset
    public InputActionAsset _inputAsset;
    // InputActionMap
    public InputActionMap uiActionMap;
    public InputActionMap dialogueActionMap;
    public InputActionMap playerWalkActionMap;
    public InputActionMap playerCLIMBActionMap;
    public InputActionMap playerMonkeyActionMap;
    public InputActionMap playerDefaultActionMap;
    
    // 상태
    [ContextMenuItem("switch inputState", "Test")]
    public InputState state;

    private void Awake()
    {
        instance = this;
        InitInput();
    }

    private void InitInput()
    {
        // Find Action Maps
        uiActionMap = _inputAsset.FindActionMap("UI");
        dialogueActionMap = _inputAsset.FindActionMap("Dialogue");
        playerWalkActionMap = _inputAsset.FindActionMap("PlayerWalk");
        playerCLIMBActionMap = _inputAsset.FindActionMap("PlayerClimb");
        playerMonkeyActionMap = _inputAsset.FindActionMap("PlayerMonkey");
        playerDefaultActionMap = _inputAsset.FindActionMap("PlayerDefault");

        // 모두 비활성화하고 PlayerWalk와 PlayerDefault만 활성화
        _inputAsset.Disable();
        _inputAsset.FindActionMap("PlayerWalk").Enable();
        _inputAsset.FindActionMap("PlayerDefault").Enable();
    }

    public void ChangeInputState(InputState newState)
    {
        if (state == newState) return;
        state = newState;
        Debug.Log("ChangeInputState");

        switch (state)
        {
            case InputState.PLAYER_WALK:
                _inputAsset.Disable();
                playerDefaultActionMap.Enable();
                playerWalkActionMap.Enable();
                break;
            case InputState.PLAYER_CLIMB:
                _inputAsset.Disable();
                playerDefaultActionMap.Enable();
                playerCLIMBActionMap.Enable();
                break;
            case InputState.PLAYER_MONKEY:
                _inputAsset.Disable();
                playerDefaultActionMap.Enable();
                playerMonkeyActionMap.Enable();
                break;
            case InputState.DIALOGUE:
                _inputAsset.Disable();
                playerDefaultActionMap.Enable();
                dialogueActionMap.Enable();
                break;
            case InputState.IGNORE:
                _inputAsset.Disable();
                playerDefaultActionMap.Enable();
                uiActionMap.Enable();
                break;
            case InputState.UICONTROL:
                _inputAsset.Disable();
                break;
        }
    }

    /*
    public void Test()
    {
        if (state == InputState.PLAYERMOVE)
            ChangeInputState(InputState.UICONTROL);
        else
            ChangeInputState(InputState.PLAYERMOVE);
    }
    */
}

