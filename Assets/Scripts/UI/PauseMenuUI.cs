using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 일시정지 메뉴
/// </summary>
public class PauseMenuUI : MonoBehaviour
{
    GameObject uiObject;

    public void OpenPauseMenu()
    {
        uiObject.SetActive(true);
        // TODO: 일시정지 메뉴 열기 전 상태(걷기/기어오르기 등)을 저장해뒀다가 일시정지 해제되면 복구
        InputManager.Instance.SetUiInputState(UiState.MENU);
    }

    public void ClosePauseMenu()
    {
        uiObject.SetActive(false);
        InputManager.Instance.SetUiInputState(UiState.IN_GAME);
    }
}
