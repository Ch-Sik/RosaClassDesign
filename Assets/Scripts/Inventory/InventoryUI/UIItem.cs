using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

/// <summary>
/// 인벤토리 UI를 열었을 때, 각각의 아이템 슬롯 데이터를 담음.
/// </summary>

public class UIItem : MonoBehaviour
{
    [Header("Data")]
    public SOItem item;
    public int quantity;

    [Header("ItemUI")]
    public Image border;
    public Image itemImage;
    public Image backImage;
    public TextMeshProUGUI itemQuantity;

    //일부러 이벤트를 사용하지 않았다. 어차피 Init을 할테니 그냥 서로 연결시켜두는 것이 추후에도 나을 것.
    public InventoryUI inventoryUI;

    public void Init(SOItem item, int quantity = 0)
    {
        ResetBorder();

        this.item = item;
        itemImage.sprite = item.itemImage;
        backImage.sprite = item.itemImage;
        SetQuantity(quantity);
    }

    //아이템의 개수에 따라 검은 아이템을 보여줄지, 혹은 이미지를 보여줄 것인지 선택하고, 아이템의 스태커블을 따져 표기함.
    public void SetQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            quantity = 0;
            itemImage.gameObject.SetActive(false);
        }
        else
        {
            this.quantity = quantity;
            itemImage.gameObject.SetActive(true);

            if (item.isStackable)
                itemQuantity.text = quantity.ToString();
            else
                itemQuantity.text = "";
        }
    }

    //아이템 선택 테두리 활성화
    public void SetBorder()
    {
        inventoryUI.ResetAllBorder();
        inventoryUI.SetDescsription(item, quantity);
        border.gameObject.SetActive(true);
    }

    //아이템 선택 테두리 비활성화
    public void ResetBorder()
    {
        if(border.gameObject.activeSelf)
            border.gameObject.SetActive(false);
    }

    //클릭 시 이벤트 
    public void OnClick() { SetBorder(); }
}
