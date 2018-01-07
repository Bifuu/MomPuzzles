using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShopController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseShopWindow();
        }
	}

    public void CloseShopWindow()
    {
        gameObject.SetActive(false);
    }
}
