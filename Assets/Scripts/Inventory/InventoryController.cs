using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

/// <summary>
/// Inventory 클래스와 InventoryUI 클래스를 총괄하며, 아이템을 추가하고 뺄 수 있는 클래스
/// </summary>

public class InventoryController : MonoBehaviour
{
    //인벤토리 데이터
    public Inventory inventory;
    //인벤토리 UI 스크립트
    private InventoryUI inventoryUI;

    public List<Item> items = new List<Item>();

    private void Start()
    {
        inventoryUI = GetComponent<InventoryUI>();

        LoadInventory();

        Init();
    }

    //저장된 데이터로부터 인벤토리를 로드함
    private void LoadInventory()
    {
        /*
        if(인벤토리 세이브 데이터를 로드할 수 있다면,)
            Inventory = new Inventroy(데이터);
        else
         */
        inventory = new Inventory();
    }

    private void Init()
    {
        inventoryUI.Init();
    }

    public void AddItem(ItemCode itemCode, int quantity)
    { 
        inventory.AddItem(itemCode, quantity);
        UpdateItemUI(itemCode);
    }

    public bool RemoveItem(ItemCode itemCode, int quantity)
    {
        if (!inventory.RemoveItem(itemCode, quantity))
            return false;

        UpdateItemUI(itemCode);
        return true;
    }

    //아이템에 변화(개수)가 생김을 감지했을 때, UI를 업데이트해주는 역할
    public void UpdateItemUI(ItemCode itemCode) { inventoryUI.itemUIs[itemCode].SetQuantity(inventory.GetQuantity(itemCode)); }
}
