using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextButtonUI : MonoBehaviour
{
    public TextMeshProUGUI text;

    public Color enableColor;
    public Color disableColor;

    private void Start() { if (text == null) text = GetComponent<TextMeshProUGUI>(); text.color = disableColor; }
    public void OnIn() { text.color = enableColor; }
    public void OnOut() { text.color = disableColor; }
}
