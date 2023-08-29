using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public bool isStackable = true;
    public ItemCode itemCode;       // ItemCode를 Int(Enum)으로 할지 String으로 할지 고민중
    public Sprite itemImage;
    public string itemName;
    public ItemRarity rarity;
    public string itemDescription;
}

public enum ItemRarity
{
    normal,
    rare,
    epic
}