using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// InventoryUI를 관리함.
/// </summary>

public class InventoryUI : MonoBehaviour
{
    [Header("UIs")]
    public Image backGround;
    public RectTransform UI;
    Sequence openEvent;

    [Header("SlotPanel")]
    public UIItem weapon;
    public List<UISkill> magics = new List<UISkill>();
    public List<UIItem> slots = new List<UIItem>();

    [Header("DescriptionPanel")]
    public Image itemImage;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDescription;

    [Header("SlotDatas")]
    [ShowInInspector] public Dictionary<SkillCode, UISkill> skillUIs = new Dictionary<SkillCode, UISkill>();
    [ShowInInspector] public Dictionary<ItemCode, UIItem> itemUIs = new Dictionary<ItemCode, UIItem>();
    public List<SOSkill> skillList = new List<SOSkill>();
    public List<SOItem> itemList = new List<SOItem>();

    private void Start()
    {
        SetSequence();
    }

    private void SetSequence()
    {
        openEvent = DOTween.Sequence()
        .AppendCallback(() => UI.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -1080, 0))
        .Append(UI.GetComponent<RectTransform>().DOAnchorPosY(0, 0.35f).SetEase(Ease.InQuad))
        .Join(backGround.DOColor(new Color(0, 0, 0, 200f / 255f), 0.35f).SetEase(Ease.InQuad))
        .SetAutoKill(false);

        openEvent.Pause();
    }

    public void ResetInventoryUI()
    {
        ResetAllBorder();
        ResetDescription();
    }

    //기본적인 데이터들을 셋함. 
    public void Init()
    {
        //패널에 데이터를 주고받기 위해 변수에 본인을 할당시켜줌.
        weapon.inventoryUI = this;
        for (int i = 0; i < magics.Count; i++)
            magics[i].inventoryUI = this;
        for(int i = 0; i < slots.Count; i++)
            slots[i].inventoryUI = this;

        //데이터 청소
        skillUIs.Clear();
        itemUIs.Clear();

        //스킬 개수 파악
        SkillCode[] skillCodes = (SkillCode[])Enum.GetValues(typeof(SkillCode));

        //스킬과 스킬UI연결
        for (int i = 0; i < skillCodes.Length; i++)
        {
            for (int j = 0; j < skillList.Count; j++)
            {
                if (skillList[j].skillCode == skillCodes[i])
                {
                    skillUIs.Add(skillCodes[i], magics[i]);
                    skillUIs[skillCodes[i]].Init(skillList[i]);
                }
            }
        }

        //아이템 개수 파악
        ItemCode[] itemCodes = (ItemCode[])Enum.GetValues(typeof(ItemCode));

        //아이템과 아이템UI연결
        for (int i = 0; i < itemCodes.Length; i++)
        {
            for (int j = 0; j < itemList.Count; j++)
            {
                if (itemList[j].itemCode == itemCodes[i])
                {
                    itemUIs.Add(itemCodes[i], slots[i]);
                    //Init을 통해 여러 데이터를 한 번에 넘겨줌
                    itemUIs[itemCodes[i]].Init(itemList[i]);
                }
            }
        }

        //선택과 설명을 초기화 시킴
        ResetAllBorder();
        ResetDescription();
    }

    //선택을 초기화함 
    public void ResetAllBorder()
    {
        weapon.ResetBorder();

        for (int i = 0; i < magics.Count; i++)
            magics[i].ResetBorder();

        for(int i = 0; i < slots.Count; i++)
            slots[i].ResetBorder();
    }

    //설명을 설정함. 개수를 받는 이유는 아이템의 개수에 따라 보이는 정보가 달라지기 때문이다.
    public void SetDescsription(SOItem item, int quantity)
    {
        ResetDescription();
        itemImage.color = Color.white;

        if (item == null) return;

        if (quantity <= 0)
        {
            itemImage.color = Color.black;
            itemImage.sprite = item.itemImage;
            itemName.text = "???";
            itemDescription.text = "";
        }
        else
        {
            //itemImage.color = Color.white;
            itemImage.sprite = item.itemImage;
            itemName.text = $"{item.itemName}";
            itemDescription.text = $"{item.itemDescription}";
        }
    }

    public void SetDescsription(SOSkill skill)
    {
        ResetDescription();
        itemImage.color = Color.white;

        if (skill == null) return;

        if (skill.isUnlock)
        {
            itemImage.sprite = skill.skillImage;
            itemName.text = $"{skill.skillName}";
            itemDescription.text = $"{skill.skillDescription}";
        }
        else
        {
            itemImage.color = Color.black; ;
            itemImage.sprite = skill.skillImage;
            itemName.text = "???";
            itemDescription.text = "";
        }
    }

    //설명을 초기화 함.
    public void ResetDescription()
    {
        itemImage.color = Color.white;
        itemImage.sprite = null;
        itemName.text = "";
        itemDescription.text = "";
    }

    public void UnlockSkill(SkillCode skillCode)
    {
        for (int i = 0; i < skillList.Count; i++)
            if (skillList[i].skillCode == skillCode)
            {
                skillList[i].isUnlock = true;
                skillUIs[skillCode].SetUnlock();

                return;
            }
    }

    public SOSkill GetSkill(SkillCode skillCode)
    {
        for (int i = 0; i < skillList.Count; i++)
            if (skillList[i].skillCode == skillCode)
                return skillList[i];

        return null;
    }

    public SOItem GetItem(ItemCode itemCode)
    {
        for (int i = 0; i < itemList.Count; i++)
            if (itemList[i].itemCode == itemCode)
                return itemList[i];

        return null;
    }

    public void Show()
    {
        UI.gameObject.SetActive(true);
        backGround.gameObject.SetActive(true);

        ResetInventoryUI();

        openEvent.Play();
    }

    public void Hide()
    {
        ResetInventoryUI();

        openEvent.Restart();
        openEvent.Pause();

        UI.anchoredPosition = new Vector3(0, -1080, 0);
        backGround.color = new Color(0, 0, 0, 0);

        UI.gameObject.SetActive(false);
        backGround.gameObject.SetActive(false);
    }
}
