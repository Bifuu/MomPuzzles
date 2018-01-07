using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIItem : MonoBehaviour {

    public Equippable Item;
    public Text ItemName;
    public Image ItemIcon;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Setup(Equippable item)
    {
        Item = item;
        ItemName.text = Item.ItemName;
        ItemName.color = Item.Rarity ? Item.Rarity.RarityColor : Color.white;
        ItemIcon.sprite = Item.IconSprite;
        
    }
}
