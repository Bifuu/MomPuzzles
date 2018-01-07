using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class WorldMap : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SelectLocation(Location loc)
    {
        PlayerController.Controller.CurrentLocation = loc;
        SceneManager.LoadSceneAsync("_BlockClear");
    }
}
