using UnityEngine;
using System.Collections;

[System.Serializable]
public class InventoryItem {

    public Item Item;
    public int StackCount;

    public InventoryItem(Item item, int amount)
    {
        Item = item;
        StackCount = amount;
    }

    public InventoryItem(Item item)
    {
        Item = item;
        StackCount = 1;
    }
}
