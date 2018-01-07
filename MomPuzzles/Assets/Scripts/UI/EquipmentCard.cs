using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentCard : MonoBehaviour {

    public Text ItemName;
    public Text Description;
    public bool SelectedWeapon;
    public GameObject WeaponCard;
    public Text WeaponMin;
    public Text WeaponMax;
    public GameObject ArmorCard;
    public Text ArmorMax;
    public Image Icon;



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetData(string name, string desc, Sprite icon, int minDamage, int maxDamage)
    {
        ArmorCard.SetActive(false);
        WeaponCard.SetActive(true);
        WeaponMin.text = minDamage.ToString();
        WeaponMax.text = maxDamage.ToString();
        SetData(name, desc, icon);
    }

    public void SetData(string name, string desc, Sprite icon, int armor)
    {
        ArmorCard.SetActive(true);
        WeaponCard.SetActive(false);
        ArmorMax.text = armor.ToString();
        SetData(name, desc, icon);
    }

    public void SetData(string name, string desc, Sprite icon)
    {
        ItemName.text = name;
        Description.text = desc;
        Icon.sprite = icon;
    }
}
