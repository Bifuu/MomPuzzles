using UnityEngine;

public class Equippable : ScriptableObject{

    public string ItemName = "Item Name";
    public int ID = 0;
    public ItemRarity Rarity;
    public Sprite IconSprite = null;
    public int LevelRequirement = 0;
    public BlockColor Attunement = BlockColor.None;
}
