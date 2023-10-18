using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

/// <summary>
/// Inventory 클래스와 InventoryUI 클래스를 총괄하며, 아이템을 추가하고 뺄 수 있는 클래스
/// </summary>

//InventoryController가 굳이 싱글턴일 이유는 없다. 나중에 싱글턴으로 객체리퍼를 담아놔도 좋을 것이지만, 테스트를 위해 싱글턴 처리했다.
public class InventoryController : MonoBehaviour
{
    private static InventoryController instance;
    public static InventoryController Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    //인벤토리 UI 스크립트
    public InventoryUI inventoryUI;
    //아이템 획득 알림 스크립트
    public ItemEventController eventController;
    //인벤토리 데이터
    public Inventory inventory;

    //인벤토리 오픈 시퀀스 데이터
    private Sequence openEvent;

    private void Start()
    {
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

    [Button]
    public void UnlockSkill(SkillCode skillCode)
    {
        inventoryUI.UnlockSkill(skillCode);

        SOSkill skill = inventoryUI.GetSkill(skillCode);
        if (skill != null)
        {
            eventController.UnlockSkill(skill);
            UpdateInventoryUI(skillCode);
        }
    }

    [Button]
    public void AddItem(ItemCode itemCode, int quantity)
    { 
        inventory.AddItem(itemCode, quantity);

        SOItem item = inventoryUI.GetItem(itemCode);
        if (item != null)
        {
            eventController.AddItem(item, quantity);
            UpdateInventoryUI(itemCode);
        }
    }

    [Button]
    public bool RemoveItem(ItemCode itemCode, int quantity)
    {
        if (!inventory.RemoveItem(itemCode, quantity))
            return false;

        UpdateInventoryUI(itemCode);
        return true;
    }

    //아이템이나 스킬의 변화(개수/언락)가 생김을 감지했을 때, UI를 업데이트해주는 역할
    public void UpdateInventoryUI(ItemCode itemCode) { inventoryUI.itemUIs[itemCode].SetQuantity(inventory.GetQuantity(itemCode)); }
    public void UpdateInventoryUI(SkillCode skillCode) { inventoryUI.skillUIs[skillCode].SetUnlock(); }

    //인벤토리를 여닫는 액션
    [Button]
    public void InventoryAction()
    {
        if (inventoryUI.UI.gameObject.activeSelf)
        {
            inventoryUI.Hide();
            OnClose();
        }
        else
        {
            inventoryUI.Show();
            OnOpen();
        }
    }

    public void OnOpen() { }
    public void OnClose() { }
}
