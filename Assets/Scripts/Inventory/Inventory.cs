using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어가 소유한 아이템을 보관하는 클래스. 
/// 베리도 Stackable Item으로 취급하여 동등하게 관리
/// </summary>

[Serializable]
public class Inventory/// MB를 굳이 상속할 필요는 없지만 디버깅할 때 Inspector 상에서 보기 편하라고 MB 상속하게 함.
{
    [ShowInInspector] private Dictionary<ItemCode, int> bag;       // ItemCode를 Int(Enum)으로 할지 String으로 할지 고민중
    // TODO: Dictionary를 Serializable container로 바꿀 것. 에셋 스토어의 SerializableDictionary를 쓰면 될 듯?

    public Inventory()
    {
        bag = new Dictionary<ItemCode, int>();
    }

    public Inventory(Dictionary<ItemCode, int> data)
    {
        this.bag = data;
    }

    public void AddItem(ItemCode itemCode, int quantity) { bag[itemCode] += quantity; }

    // 제거할 아이템이 없다면 false 리턴, 정상적으로 제거하면 true 리턴
    public bool RemoveItem(ItemCode itemCode, int quantity)
    {
        if ((bag[itemCode] -= quantity) < 0)
            return false;

        bag[itemCode] -= quantity;
        return true;
    }
    public int GetQuantity(ItemCode itemCode) { return bag[itemCode]; }
}