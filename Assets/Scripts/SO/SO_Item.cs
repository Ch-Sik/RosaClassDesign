using UnityEngine;

[CreateAssetMenu(fileName = "Item_0", menuName = "Inventory/Item")]
public class SO_Item : ScriptableObject
{
    public bool isStackable = true;
    public ItemCode itemCode;
    public Sprite itemImage;
    public string itemName;
    public ItemRarity rarity;
    [TextArea(0, 10)] public string itemDescription;
}

public enum ItemRarity
{
    normal,
    speical
}