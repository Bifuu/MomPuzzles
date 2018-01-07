using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EquipmentController : MonoBehaviour {

    public UIItem UIItemPrefab;
    public ArmorDatabase ArmorDB;
    List<Armor> availableArmor;
    public WeaponDatabase WeaponDB;
    List<Weapon> availableWeapon;
    public Transform ContentTransform;

    Weapon inspectingWeapon;
    Armor inspectingArmor;

    public float TabSelectedOffset = 15f;
    public enum EquipmentTab{ WeaponTab, ArmorTab };
    public EquipmentTab selectedTab = EquipmentTab.WeaponTab;

    //UI stuff
    public Button WeaponTab;
    public Button ArmorTab;
    public Button EquipButton;
    public EquipmentCard EquippedCard;
    public EquipmentCard SelectedCard;

    Player player;

	// Use this for initialization
	void Start () {
        player = PlayerController.Controller.Player;
        //EquippedCard.SetData("Item Name", "Item Stuff here", null);

        GetAvailableLists();
        SelectTab(EquipmentTab.WeaponTab);
        
	}
	
	// Update is called once per frame
	void Update () {


        //Close window
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseEquipment();
        }
    }

    void SelectTab(EquipmentTab tab)
    {
        ClearChildren();

        selectedTab = tab;
        if (tab == EquipmentTab.ArmorTab)
        {
            Debug.Log("EqC: Armor Tab");
            ClearInspection();
            //ARMOR
            if (player.EquippedArmor)
            {
                EquippedCard.SetData(player.EquippedArmor.ItemName, "", player.EquippedArmor.IconSprite, player.EquippedArmor.DamageBlock);
            }
            else
            {
                EquippedCard.SetData("Nude!?", "", null, player.BlockDamage);
            }
            //ArmorTab.transform.position += new Vector3(0, -1 * TabSelectedOffset, 0);
            (ArmorTab.transform as RectTransform).anchoredPosition = new Vector2(0, -1 * TabSelectedOffset);
            (WeaponTab.transform as RectTransform).anchoredPosition = Vector2.zero;

            FillAvailableArmors();
        }
        else
        {
            Debug.Log("EqC: Weapon Tab");
            ClearInspection();
            //WEAPON
            if (player.EquippedWeapon)
            {
                EquippedCard.SetData(player.EquippedWeapon.ItemName, "", player.EquippedWeapon.IconSprite, player.EquippedWeapon.MinDamage, player.EquippedWeapon.MaxDamage);
            }
            else
            {
                EquippedCard.SetData("Unarmed", "", null, player.AttackDamage, player.AttackDamage);
            }
            (ArmorTab.transform as RectTransform).anchoredPosition = Vector2.zero;
            (WeaponTab.transform as RectTransform).anchoredPosition = new Vector2(0, -1 * TabSelectedOffset);

            FillAvailableWeapons();
        }
    }

    public void CloseEquipment()
    {
        gameObject.SetActive(false);
    }

    public void ArmorTabClicked()
    {
        if (selectedTab != EquipmentTab.ArmorTab)
        {
            ClearInspection();
            SelectTab(EquipmentTab.ArmorTab);
        }
    }

    public void WeaponTabClicked()
    {
        if (selectedTab != EquipmentTab.WeaponTab)
        {
            ClearInspection();
            SelectTab(EquipmentTab.WeaponTab);
        }
    }

    void ClearInspection()
    {
        inspectingArmor = null;
        inspectingWeapon = null;

        SelectedCard.gameObject.SetActive(false);
        EquipButton.interactable = false;
    }

    void GetAvailableLists()
    {
        availableArmor = new List<Armor>();
        availableWeapon = new List<Weapon>();
        foreach (Armor armor in ArmorDB.DB)
        {
            if (player.AvailableArmorIDs.Contains(armor.ID)) //Make sure we havnt already completed it
            {
                availableArmor.Add(armor);
            }
        }

        foreach (Weapon weapon in WeaponDB.DB)
        {
            if (player.AvailableWeaponIDs.Contains(weapon.ID)) //Make sure we havnt already completed it
            {
                availableWeapon.Add(weapon);
            }
        }
    }
    
    void ClearChildren()
    {
        
        for (int i = ContentTransform.childCount - 1; i > -1; i--)
        {
            Destroy(ContentTransform.GetChild(i).gameObject);
        }
    }

    void FillAvailableWeapons()
    {
        foreach (Weapon weapon in availableWeapon)
        {
            UIItem item = Instantiate(UIItemPrefab, ContentTransform, false);
            item.Setup(weapon);
            if (player.CanEquipWeapon(weapon) && player.EquippedWeapon != weapon)
            {
                item.gameObject.GetComponent<Button>().onClick.AddListener(() => { InspectWeapon(weapon); });
            }
        }
    }

    void FillAvailableArmors()
    {
        foreach (Armor armor in availableArmor)
        {
            UIItem item = Instantiate(UIItemPrefab, ContentTransform, false);
            item.Setup(armor);
            if (player.CanEquipArmor(armor) && player.EquippedWeapon != armor)
            {
                item.gameObject.GetComponent<Button>().onClick.AddListener(() => { InspectArmor(armor); });
            }
        }
    }

    void UpdateCards()
    {

    }

    void InspectArmor(Armor armor)
    {
        inspectingArmor = armor;
        SelectedCard.SetData(armor.ItemName, "", null, armor.DamageBlock);
        SelectedCard.gameObject.SetActive(true);

        EquipButton.interactable = true;
    }

    void InspectWeapon(Weapon weapon)
    {
        inspectingWeapon = weapon;
        SelectedCard.SetData(weapon.ItemName, "", null, weapon.MinDamage, weapon.MaxDamage);
        SelectedCard.gameObject.SetActive(true);
        

        EquipButton.interactable = true;
    } 

    public void HavePlayerEquipWeapon()
    {
        Debug.Log("Equip?!");
        //player.EquipWeapon(inspectingWeapon);
        if (inspectingArmor)
        {
            player.EquipArmor(inspectingArmor);
        }
        if (inspectingWeapon)
        {
            player.EquipWeapon(inspectingWeapon);
        }
        SelectTab(selectedTab); // Reselct current tab to update cards and listings...
    }
}


