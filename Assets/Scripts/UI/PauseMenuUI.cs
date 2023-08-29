using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 일시정지 메뉴
/// </summary>
public class PauseMenuUI : MonoBehaviour
{
    GameObject uiObject;

    InputState lastInputState;

    public void OpenPauseMenu()
    {
        uiObject.SetActive(true);
        // 일시정지 메뉴 열기 전 상태(걷기/기어오르기 등)을 저장해뒀다가 일시정지 해제되면 복구
        lastInputState = InputManager.Instance.state;
        InputManager.Instance.ChangeInputState(InputState.UICONTROL);
    }

    public void ClosePauseMenu()
    {
        uiObject.SetActive(false);
        InputManager.Instance.ChangeInputState(lastInputState);
    }
}
