using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScrollbarUI : MonoBehaviour
{
    OptionUI option;

    public Scrollbar bar;
    public TextMeshProUGUI value;

    public void SetValue(float var) { bar.value = var; OnValueChanged(); }
    public float GetValue() { return bar.value; }
    public string GetValueAsString() { return ((int)(bar.value * 100)).ToString(); }
    public void OnValueChanged() { value.text = GetValueAsString(); option.OnScrollChanged(this); }
}
