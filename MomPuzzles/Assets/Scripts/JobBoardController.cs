using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class JobBoardController : MonoBehaviour {

    public int MaxQuestsAvailable = 3;
    public JobBoardListing QuestPrefab;
    public Transform QuestListingContainer;
    public GameObject TurnInButton;

    public QuestDatabase QuestDB;
    public List<Quest> availableQuests;

    Player player;

	// Use this for initialization
	void Start () {
	    
	}

    void OnEnable()
    {
        player = PlayerController.Controller.Player;
        Debug.Log("Job Board Enabled.");
        GetAvailableQuests();

        
        TurnInButton.SetActive(false);

        foreach (CurrentQuest q in player.Quests)
        {
            if (q.QuestComplete)
            {
                TurnInButton.SetActive(true);
            }
        }

        DrawQuests();
    }
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseJobBoard();
        }
	}

    public void CloseJobBoard()
    {
        gameObject.SetActive(false);
    }

    public void TurnInQuests()
    {
        for (int i = player.Quests.Count - 1; i >= 0 ; i--)
        {
            if (player.Quests[i].QuestComplete)
            {
                player.CompletedQuestIDS.Add(player.Quests[i].Quest.ID);
                foreach (Equippable item in player.Quests[i].Quest.RewardedItems)
                {
                    if (item as Weapon)
                    {
                        player.AvailableWeaponIDs.Add(item.ID);
                    }
                    else if (item as Armor)
                    {
                        player.AvailableArmorIDs.Add(item.ID);
                    }
                }

                Debug.Log("Quest Turn In: " + player.Quests[i].Quest.QuestName);
                player.Quests.RemoveAt(i);

            }
            
        }

        GetAvailableQuests();
        DrawQuests();
        TurnInButton.SetActive(false);
    }

    void GetAvailableQuests()
    {
        availableQuests = new List<Quest>();
        foreach(Quest quest in QuestDB.Quests)
        {
            if (!player.CompletedQuestIDS.Contains(quest.ID)) //Make sure we havnt already completed it
            {
                bool available = true;
                foreach (Quest reqQuest in quest.RequiredQuests)
                {
                    if (!player.CompletedQuestIDS.Contains(reqQuest.ID))
                    {
                        available = false;
                    }
                }
                if (available)
                {
                    availableQuests.Add(quest);
                }                
            }
        }
    }

    void DrawQuests()
    {
        //Clear any garbage if needed
        int childrenCount = QuestListingContainer.childCount;
        for (int i = childrenCount - 1; i >= 0; i--)
        {
            //Debug.Log("Destroy Child...");
            Destroy(QuestListingContainer.GetChild(i).gameObject);
        }

        //Fill with new garbage
        for (int i = 0; i < MaxQuestsAvailable; i++)
        {
            if (i < availableQuests.Count)
            {
                //Debug.Log("Create Objective");
                if (!player.Quests.Exists(cq => cq.Quest == availableQuests[i]) && !player.CompletedQuestIDS.Contains(availableQuests[i].ID))
                {
                    JobBoardListing qoPrefab = Instantiate(QuestPrefab, QuestListingContainer, false) as JobBoardListing;
                    qoPrefab.Setup(availableQuests[i]);
                }

            }
        }
    }
}
