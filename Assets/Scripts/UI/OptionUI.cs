using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionUI : MonoBehaviour
{
    public GameObject UI;

    public OptionSetting defaultOption = new OptionSetting();
    public OptionSetting savedOption;           //저장된 옵션
    public OptionSetting currentOption;         //현재 적용된 옵션

    public TextChoiceButtonController Window;
    public TextChoiceButtonController Resolution;
    public ScrollbarUI MasterVolume;
    public ScrollbarUI BGM;
    public ScrollbarUI SFX;

    private void Start()
    {
        //Load();
    }

    [Button]
    public void SetDefault()
    {
        currentOption = defaultOption.MakeCopy();
        SetByCurrentOption();
    }

    #region Save/Load
    public void SetByCurrentOption()
    {
        SetWindow();
        SetResolution();
        SetMasterVolume();
        SetBGM();
        SetSFX();
    }

    public void Apply()
    {
        SetByCurrentOption();
        Save();
    }

    public void Reset_()
    {
        currentOption = savedOption.MakeCopy();
        SetByCurrentOption();
    }

    public void Default()
    {
        currentOption = defaultOption.MakeCopy();
        SetByCurrentOption();
    }

    public void Save()
    {
        SaveLoadManager.Instance.SaveOptionData(currentOption);
    }

    public void Load()
    {
        if (SaveLoadManager.Instance == null)
            return;

        OptionSetting loadedOption = SaveLoadManager.Instance.LoadOptionData();
        if (loadedOption == null)
        {
            savedOption = defaultOption.MakeCopy();
            currentOption = savedOption.MakeCopy();
            return;
        }

        savedOption = loadedOption.MakeCopy();
        currentOption = savedOption.MakeCopy();

        SetByCurrentOption();
    }

    #endregion

    #region Display

    public void OnChoiceButtonChanged(TextChoiceButtonController cont)
    {
        if (cont == Window)
            currentOption.window = Window.index;
        else if (cont == Resolution)
            currentOption.resolution = Resolution.index;
    }

    public void SetWindow() { Window.Choice(currentOption.window); }

    public void SetResolution() { Resolution.Choice(currentOption.resolution); }

    #endregion

    #region Sound

    public void OnScrollChanged(ScrollbarUI scro)
    {
        if (scro == MasterVolume)
            currentOption.vol = scro.GetValue();
        else if (scro == BGM)
            currentOption.bgm = scro.GetValue();
        else if (scro == SFX)
            currentOption.sfx = scro.GetValue();
    }

    public void SetMasterVolume() { MasterVolume.SetValue(currentOption.vol); }

    public void SetBGM() { BGM.SetValue(currentOption.bgm); }

    public void SetSFX() { SFX.SetValue(currentOption.sfx); }

    #endregion

    #region UI Set
    [Button]
    public void Open() { UI.SetActive(true); }

    [Button]
    public void Close() {  UI.SetActive(false); }

    [Button]
    public void OpenClose()
    {
        if (UI.activeSelf)
            Close();
        else
            Open();
    }
    #endregion
}

//Struct 써도 되는데, 비교 함수랑 저장 편의를 위해 클래스 사용
[Serializable]
public class OptionSetting
{
    public int window;
    public int resolution;
    public float vol;
    public float bgm;
    public float sfx;

    public OptionSetting MakeCopy()
    {
        return new OptionSetting()
        {
            window = window,
            resolution = resolution,
            vol = vol,
            bgm = bgm,
            sfx = sfx
        };
    }

    public bool isEqual(OptionSetting another)
    {
        if (window != another.window ||
            resolution != another.resolution ||
            vol != another.resolution ||
            bgm != another.bgm ||
            sfx != another.sfx)
            return false;

        return true;
    }

    //생성자로 Default Option 생성
    public OptionSetting()
    {
        window = 1;
        resolution = 1;
        vol = 0.5f;
        bgm = 0.5f;
        sfx = 0.5f;
    }
}