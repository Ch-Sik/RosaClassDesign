using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TextChoiceButtonController : MonoBehaviour
{
    public OptionUI option;

    public Color enableColor;
    public Color disableColor;

    public TextChoiceButtonUI choiceBtn;
    public int index;
    public List<TextChoiceButtonUI> btns;


    public void Choice(int index)
    {
        for (int i = 0; i < btns.Count; i++)
            if (i == index)
                choiceBtn = btns[i];

        Choice(choiceBtn);
    }

    public void Choice(TextChoiceButtonUI choiceBtn)
    {
        this.choiceBtn = choiceBtn;
        for (int i = 0; i < btns.Count; i++)
            if (btns[i] != choiceBtn)
            {
                btns[i].SetDisable();
                btns[i].state = false;
            }
            else
                index = i;
        choiceBtn.SetEnable();

        option.OnChoiceButtonChanged(this);
    }
}