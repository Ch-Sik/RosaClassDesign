using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어가 소유한 아이템을 보관하는 클래스. 
/// 베리도 Stackable Item으로 취급하여 동등하게 관리
/// </summary>
public class Inventory : MonoBehaviour /// MB를 굳이 상속할 필요는 없지만 디버깅할 때 Inspector 상에서 보기 편하라고 MB 상속하게 함.
{
    private Dictionary<ItemCode, int> bag;       // ItemCode를 Int(Enum)으로 할지 String으로 할지 고민중
    // TODO: Dictionary를 Serializable container로 바꿀 것. 에셋 스토어의 SerializableDictionary를 쓰면 될 듯?

    Inventory()
    {
        bag = new Dictionary<ItemCode, int>();
    }

    Inventory(Dictionary<ItemCode, int> data)
    {
        this.bag = data;
    }

    public void AddItem(ItemCode itemCode, int count) { }
    public bool RemoveItem(ItemCode itemCode, int count) { return false; }    // 제거할 아이템이 없다면 false 리턴, 정상적으로 제거하면 true 리턴
    public int GetItemCount(ItemCode itemCode) { return bag[itemCode]; }
}


