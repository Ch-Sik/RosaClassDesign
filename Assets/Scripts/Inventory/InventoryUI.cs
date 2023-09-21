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
    [Header("SlotPanel")]
    public ItemUI weapon;
    public List<ItemUI> magics = new List<ItemUI>();
    public List<ItemUI> slots = new List<ItemUI>();

    [Header("DescriptionPanel")]
    public Image itemImage;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDescription;

    [Header("SlotDatas")]
    [ShowInInspector] public Dictionary<ItemCode, ItemUI> skillUIs = new Dictionary<ItemCode, ItemUI>();
    [ShowInInspector] public Dictionary<ItemCode, ItemUI> itemUIs = new Dictionary<ItemCode, ItemUI>();
    public List<Item> itemList = new List<Item>();

    public void Init()
    {
        weapon.inventoryUI = this;
        for (int i = 0; i < magics.Count; i++)
            magics[i].inventoryUI = this;
        for(int i = 0; i < slots.Count; i++)
            slots[i].inventoryUI = this;

        skillUIs.Clear();
        itemUIs.Clear();

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
                    itemUIs[itemCodes[i]].Init(itemList[i]);
                }
            }
        }

        ResetAllBorder();
        ResetDescription();
    }

    public void ResetAllBorder()
    {
        weapon.ResetBorder();

        for(int i = 0; i < magics.Count; i++)
            magics[i].ResetBorder();

        for(int i = 0; i < slots.Count; i++)
            slots[i].ResetBorder();
    }

    public void SetDescsription(Item item, int quantity)
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

    public void ResetDescription()
    {
        itemImage.sprite = null;
        itemName.text = "";
        itemDescription.text = "";
    }
}
