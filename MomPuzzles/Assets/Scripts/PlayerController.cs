using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public static PlayerController Controller;

    public Player Player;
    public Location CurrentLocation;

    //public Item TestItem1;
    //public Item TestItem2;
    public Quest TestQuest;

    //Events
    //
    //public delegate void InventoryUpdated();
    //public static event InventoryUpdated OnInventoryUpdated;

    // Use this for initialization
    void Awake () {
	    if (Controller == null)
        {
            DontDestroyOnLoad(gameObject);
            Controller = this;
        }
         else if (Controller != this)
        {
            Destroy(this.gameObject);
        }

        if (TestQuest)
        {
            Player.AcceptQuest(TestQuest);
        }
	}
	
	// Update is called once per frame
	void Update () {

    }

    /*public void GiveItemToPlayer(Item item, int amount)
    {
        Player.AddItemToInventory(item, amount);
        if (OnInventoryUpdated != null)
        {
            OnInventoryUpdated();
        }
    }

    public void TakeItemToPlayer(Item item, int amount)
    {

    }*/
}
