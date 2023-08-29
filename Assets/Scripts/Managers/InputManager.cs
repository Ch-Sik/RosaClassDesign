using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InputState
{
    UICONTROL,
    DIALOGUE,
    PLAYER_WALK,
    PLAYER_CLIMB,
    PLAYER_MONKEY,
    IGNORE /* 레어 아이템 획득 연출 등 잠시동안 모든 입력이 무시되어야 하는 경우.
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
    private static InputManager _instance;
    public static InputManager Instance { get { return _instance; } }

    // InputAction Asset
    public InputActionAsset _inputAsset;
    // InputActionMap
    private InputActionMap uiActionMap;
    private InputActionMap dialogueActionMap;
    private InputActionMap playerWalkActionMap;
    private InputActionMap playerCLIMBActionMap;
    private InputActionMap playerMonkeyActionMap;
    private InputActionMap playerDefaultActionMap;
    
    // 상태
    [ContextMenuItem("switch inputState", "Test")]
    public InputState state;

    private void Awake()
    {
        _instance = this;
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

        // 테스트: UI 상태에선 Attack action이 없어서 1회성임에 주의할 것
        _inputAsset.FindAction("Attack").performed += Test;
    }

    // 테스트: 마우스 클릭하면 PLAYERWALK -> UI 전환.
    public void Test(InputAction.CallbackContext context)
    {
        if (state == InputState.PLAYER_WALK)
            ChangeInputState(InputState.UICONTROL);
        else
            ChangeInputState(InputState.PLAYER_WALK);
    }

    public void ChangeInputState(InputState newState)
    {
        if (state == newState) return;

        state = newState;
        Debug.Log("ChangeInputState");

        switch (state)
        {
            case InputState.UICONTROL:
                _inputAsset.Disable();      // 모두 비활성화하고,
                uiActionMap.Enable();       // 'UI' action map에 해당하는 action들만 활성화하기
                break;
            case InputState.DIALOGUE:
                _inputAsset.Disable();
                dialogueActionMap.Enable();
                break;
            case InputState.PLAYER_WALK:
                _inputAsset.Disable();
                playerWalkActionMap.Enable();
                playerDefaultActionMap.Enable();
                break;
            case InputState.PLAYER_CLIMB:
                _inputAsset.Disable();
                playerCLIMBActionMap.Enable();
                playerDefaultActionMap.Enable();
                break;
            case InputState.PLAYER_MONKEY:
                _inputAsset.Disable();
                playerMonkeyActionMap.Enable();
                playerDefaultActionMap.Enable();
                break;
            case InputState.IGNORE:
                _inputAsset.Disable();
                break;
        }
    }
}

