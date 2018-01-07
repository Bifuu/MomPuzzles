using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Item.asset", menuName = "Puzzle RPG/Item")]
public class Item : ScriptableObject {
    public string Name = "Item Name";
    public ItemRarity ItemRarity;
    public Sprite Icon;
    public int MaxStack = 20;
    public bool QuestItem = false;
    public bool Unique = false;

}
