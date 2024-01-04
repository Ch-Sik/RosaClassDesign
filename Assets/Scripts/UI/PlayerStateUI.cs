using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 플레이어 체력 등 GamePlay 타임동안 항상 보이는 UI 담당
/// </summary>
public class PlayerStateUI : MonoBehaviour
{
    // 싱글턴
    private static PlayerStateUI _instance = null;
    public static PlayerStateUI Instance { get { return _instance; } }

    [SerializeField] TMP_Text text_selectedMagic;

    private void Awake()
    {
        _instance = this;
    }

    public void AddHPUI()
    {

    }

    public void RemoveHPUI()
    {

    }

    // TODO: 아이콘과 스프라이트 기반으로 기능 재구현
    public void UpdateSelectedMagic(SO_Magic selectedMagic)
    {
        text_selectedMagic.text = selectedMagic.skillName;
    }
}
