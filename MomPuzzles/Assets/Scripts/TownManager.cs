using UnityEngine;
using System.Collections;

public class TownManager : MonoBehaviour {

    public GameObject WorldMapUI;
    public GameObject ShopUI;
    public GameObject JobBoardUI;
    public GameObject EquipmentUI;

	// Use this for initialization
	void Start () {
        Debug.Log("Town Start");
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (WorldMapUI.activeSelf)
            {
                WorldMapUI.SetActive(false);
            }
        }
	}

    public void ButtonGoPressed()
    {
        WorldMapUI.SetActive(true);
    }

    public void ButtonItemShopPressed()
    {
        ShopUI.SetActive(true);
    }

    public void ButtonWeaponShopPressed()
    {
        ShopUI.SetActive(true);
    }

    public void ButtonJobBoardPressed()
    {
        JobBoardUI.SetActive(true);
    }

    public void ButtonEquipmentPressed()
    {
        EquipmentUI.SetActive(true);
    }
}
