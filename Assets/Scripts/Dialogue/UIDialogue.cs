using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

/// <summary>
/// DialogueSystem에서 데이터를 받아와 출력하는 스크립트
/// </summary>

public class UIDialogue : MonoBehaviour
{
    [FoldoutGroup("Pre-Input")] public GameObject UI;
    [FoldoutGroup("Pre-Input")] public Image backBoard;
    [FoldoutGroup("Pre-Input")] public CharacterImage charLef;
    [FoldoutGroup("Pre-Input")] public CharacterImage charCen;
    [FoldoutGroup("Pre-Input")] public CharacterImage charRig;
    [FoldoutGroup("Pre-Input")] public Image DialoguePanel;
    [FoldoutGroup("Pre-Input")] public TextMeshProUGUI charName;
    [FoldoutGroup("Pre-Input")] public TextMeshProUGUI Dialogue;
    [FoldoutGroup("Pre-Input")] public float startEventTime = 0.5f;
    [FoldoutGroup("Pre-Input")] public float endEventTime = 0.5f;
    [FoldoutGroup("Pre-Input")] public float outputTime = 0.025f;


    [Button]
    public void ResetPanels()
    {
        if (!UI.activeSelf)
            return;
        backBoard.color = new Color(0, 0, 0, 0);
        DialoguePanel.color = new Color(1, 1, 1, 0);
        charLef.image.enabled = false;
        charCen.image.enabled = false;
        charRig.image.enabled = false;
        charName.text = "";
        Dialogue.text = "";
    }

    [Button]
    public void StartAnimation()
    {
        Sequence start = DOTween.Sequence()
        .AppendCallback(() =>
        {
            UI.SetActive(true);
            ResetPanels();
        })
        .Append(backBoard.DOFade(200f / 255f, startEventTime))
        .Join(DialoguePanel.DOFade(1, startEventTime));
    }

    [Button]
    public void EndAnimation()
    {
        Sequence end = DOTween.Sequence()
        .Append(backBoard.DOFade(0, endEventTime))
        .Join(DialoguePanel.DOFade(0, endEventTime))
        .AppendCallback(() =>
        {
            ResetPanels();
            UI.SetActive(false);
        });
    }
}

[Serializable]
public class CharacterImage
{
    public D_Char character;
    public Image image;

    public void SetImage(D_Char character, Sprite sprite) { this.character = character; image.sprite = sprite; }
    public void ResetImage() { character = D_Char.Null; image.sprite = null; }
}