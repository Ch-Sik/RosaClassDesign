using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkill : MonoBehaviour
{
    [Header("Data")]
    public SO_Skill skill;

    [Header("ItemUI")]
    public Image border;
    public Image skillImage;
    public Image backImage;

    //일부러 이벤트를 사용하지 않았다. 어차피 Init을 할테니 그냥 서로 연결시켜두는 것이 추후에도 나을 것.
    public InventoryUI inventoryUI;

    public void Init(SO_Skill skill)
    {
        ResetBorder();

        this.skill = skill;
        skillImage.sprite = skill.icon;
        backImage.sprite = skill.icon;
        SetUnlock();
    }

    //아이템의 개수에 따라 검은 아이템을 보여줄지, 혹은 이미지를 보여줄 것인지 선택하고, 아이템의 스태커블을 따져 표기함.
    public void SetUnlock()
    {
        if (skill.isUnlock)
            skillImage.gameObject.SetActive(true);
        else
            skillImage.gameObject.SetActive(false);
    }

    //아이템 선택 테두리 활성화
    public void SetBorder()
    {
        inventoryUI.ResetAllBorder();
        inventoryUI.SetDescsription(skill);
        border.gameObject.SetActive(true);
    }

    //아이템 선택 테두리 비활성화
    public void ResetBorder()
    {
        if (border.gameObject.activeSelf)
            border.gameObject.SetActive(false);
    }

    //클릭 시 이벤트 
    public void OnClick() { SetBorder(); }
}
