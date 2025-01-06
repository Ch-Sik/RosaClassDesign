using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextChoiceButtonUI : MonoBehaviour
{
    public TextChoiceButtonController controller;
    public TextMeshProUGUI text;
    public bool state = false;
    
    public void SetEnable() { text.color = controller.enableColor; }
    public void SetDisable() { text.color = controller.disableColor; }

    public void OnClick() { controller.Choice(this); state = true; }

    public void OnIn() { SetEnable(); }

    public void OnOut() { if (state) return; SetDisable(); }
}
