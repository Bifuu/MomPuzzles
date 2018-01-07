using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class JobBoardListing : MonoBehaviour {

    public Text JobTitle;
    public Text JobDescription;

    Quest quest;
    Button button;

	// Use this for initialization
	void Awake () {
        button = GetComponent<Button>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Setup(Quest quest)
    {
        JobTitle.text = quest.QuestName;
        JobDescription.text = quest.QuestDescription;

        this.quest = quest;

        button.onClick.AddListener(QuestChosen);
    }

    public void QuestChosen()
    {
        PlayerController.Controller.Player.AcceptQuest(quest);
        Destroy(gameObject);
    }
}
