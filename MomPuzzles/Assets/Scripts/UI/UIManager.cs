using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class UIManager : MonoBehaviour {

    public Text EnemyName;
    public Text EnemyHP;
    public Image EnemySprite;
    public DamageNumber DamageNum;
    public Text LocationLevel;

    public Text PlayerLVL;
    public ProgressBar PlayerHPBar;
    public Text PlayerHP;
    public Text PlayerEXP;
    public Text PlayerDamage;
    public Text PlayerArmor;
    public DamageNumber PlayerDamageNum;


    public GameObject PlayerPanel;

    public GameObject QuestPanel;
    public Text QuestTitle;
    public GameObject QuestObjectiveContainer;
    public Text QuestObjectivePrefab;
    public Text QuestDescription;
    private int trackedQuest = 0;

    public GameObject ItemPanel;
    public UIItem ItemPrefab;
    public GameObject ItemContent;

    public GameObject MenuPanel;

    private GameManager gameManager;
    private Player player;
    PanelState PanelState = PanelState.Player;
    
    void OnEnable()
    {
        //PlayerController.OnInventoryUpdated += PlayerInventoryUpdated;
        GameManager.OnEnemyTakeDamage += EnemyTakeDamage;
        GameManager.OnPlayerTakeDamage += PlayerTakeDamage;
        GameManager.OnPlayerHealDamage += PlayerHealDamage;
    }

    void OnDisable()
    {
        //PlayerController.OnInventoryUpdated -= PlayerInventoryUpdated;
        GameManager.OnEnemyTakeDamage -= EnemyTakeDamage;
        GameManager.OnPlayerTakeDamage -= PlayerTakeDamage;
        GameManager.OnPlayerHealDamage -= PlayerHealDamage;
    }

	// Use this for initialization
	void Start () {
        gameManager = FindObjectOfType<GameManager>();
        player = PlayerController.Controller.Player;
	}
	
	// Update is called once per frame
	void Update () {
        
        EnemyName.text = gameManager.CurrentEncounter.Name;
        if (gameManager.CurrentEncounter as Enemy)
        {
            EnemyHP.text = gameManager.enemyCurrentHealth + "/" + (gameManager.CurrentEncounter as Enemy).BaseHealth;
            EnemySprite.sprite = (gameManager.CurrentEncounter as Enemy).EnemySprite;
        }
        

        PlayerHP.text = player.CurrentHP + "/" + player.MaxHP;
        PlayerHPBar.FillPercent = ((float)player.CurrentHP / (float)player.MaxHP);
        PlayerLVL.text = player.Level.ToString();
        PlayerEXP.text = player.Experience.ToString();

        if (player.EquippedWeapon)
        {

        }
        else
        {

        }

        PlayerDamage.text = player.MinDamage + "-" + player.MaxDamage;
        PlayerArmor.text = player.Defense.ToString();

        if (player.EquippedArmor)
        {

        }
        else
        {

        }

        if (PanelState == PanelState.Quest)
        {
            UpdateQuestObjectives();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuPanel.SetActive(!MenuPanel.activeSelf);
        }

        LocationLevel.text = gameManager.LocationFloor.ToString();
    }

    public void PlayerButtonPressed()
    {
        PanelState = PanelState.Player;
        PlayerPanel.SetActive(true);
        QuestPanel.SetActive(false);
        ItemPanel.SetActive(false);
    }

    public void QuestButtonPressed()
    {
        if (player.Quests.Count <= 0)
        {
            return;
        }
        PanelState = PanelState.Quest;
        SetQuestText(trackedQuest);
        PlayerPanel.SetActive(false);
        QuestPanel.SetActive(true);
        ItemPanel.SetActive(false);
    }

    public void SetQuestText(int index)
    {
        if (index < player.Quests.Count)
        {
            trackedQuest = index;
            QuestTitle.text = player.Quests[index].Quest.QuestName;
            //Debug.Log("Quest Title: " + QuestTitle.text);

            QuestDescription.text = player.Quests[index].Quest.QuestDescription;

            //Clear Children in objective container if any
            int childrenCount = QuestObjectiveContainer.transform.childCount;
            //Debug.Log("Children To clear: " + childrenCount);
            for (int i = childrenCount - 1; i >= 0; i--)
            {
                //Debug.Log("Destroy Child...");
                Destroy(QuestObjectiveContainer.transform.GetChild(i).gameObject);
            }
            //populate onbjective container with text children
            //Debug.Log("Number of Objectives: " + player.Quests[index].Quest.QuestObjectives.Count);
            for (int i = 0; i < player.Quests[index].Quest.QuestObjectives.Count; i++)
            {
                //Debug.Log("Create Objective");
                Text qoPrefab = Instantiate(QuestObjectivePrefab, QuestObjectiveContainer.transform, false) as Text;
                qoPrefab.text = player.Quests[index].Quest.QuestObjectives[i].ObjectiveString(player.Quests[index].Objectives[i].Progress);
            }
        }        
    }

    void UpdateQuestObjectives()
    {
        for (int i = 0; i < player.Quests[trackedQuest].Quest.QuestObjectives.Count; i++)
        {
            Text qoText = QuestObjectiveContainer.transform.GetChild(i).gameObject.GetComponent<Text>(); //TODO: Is this too many GetComponent calls?
            qoText.text = player.Quests[trackedQuest].Quest.QuestObjectives[i].ObjectiveString(player.Quests[trackedQuest].Objectives[i].Progress);
        }
    }

    public void ItemsButtonPressed()
    {
        PanelState = PanelState.Item;
        PlayerPanel.SetActive(false);
        QuestPanel.SetActive(false);
        ItemPanel.SetActive(true);
    }

    public void MenuButtonReturnToTown()
    {
        SceneManager.LoadSceneAsync("_Town");
    }

    public void MenuButtonQuit()
    {
        Application.Quit();
    }

    private void EnemyTakeDamage(EnemyTakeDamageArgs args)
    {
        string str = "";

        str = args.amount.ToString();
        if (args.ResultedInDeath)
            str += "\nKilling Blow!";

        Debug.Log(str + " : " + args.ResultedInDeath);
        DamageNum.Show(str, args.ResultedInDeath);
    }

    private void PlayerTakeDamage(PlayerTakeDamageArgs args)
    {
        string str = args.amount.ToString();

        PlayerDamageNum.Show(str, false);
    }

    private void PlayerHealDamage(PlayerHealDamageArgs args)
    {
        string str = "+" + args.amount.ToString();

        PlayerDamageNum.Show(str, false, Color.green, false);

    }
}

public enum PanelState
{
    Player,
    Quest,
    Item
}


