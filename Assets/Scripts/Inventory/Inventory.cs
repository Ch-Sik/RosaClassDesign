using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어가 소유한 아이템을 보관하는 클래스. 
/// </summary>
public class Inventory : MonoBehaviour /// MB를 굳이 상속할 필요는 없지만 디버깅할 때 Inspector 상에서 보기 편하라고 MB 상속하게 함.
{
    
    private int berryCount;         // 현재 소유한 베리량
    private Dictionary<ItemCode, int> bag;       // TODO: Dictionary를 Serializable container로 바꿀 것. 에셋 스토어의 SerializableDictionary를 쓰면 될 듯?

    Inventory()
    {
        bag = new Dictionary<ItemCode, int>();
    }

    public void AddBerry(int count) { }
    public bool RemoveBerry(int count) { return false; }      // 차감할 베리가 없다면 false 리턴, 정상적으로 차감하면 true 리턴
    public int GetBerryCount() { return berryCount; }                
    
    public void AddItem(ItemCode itemCode, int count) { }
    public bool RemoveItem(ItemCode itemCode, int count) { return false; }    // 제거할 아이템이 없다면 false 리턴, 정상적으로 제거하면 true 리턴
    public int GetItemCount(ItemCode itemCode) { return bag[itemCode]; }
}


