using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public bool isStackable = true;
    public ItemCode itemCode;
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