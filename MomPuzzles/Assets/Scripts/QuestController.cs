using UnityEngine;
using System.Collections;

public class QuestController : MonoBehaviour {


    Player player;

	// Use this for initialization
	void Start () {
        player = PlayerController.Controller.Player;
	}

    void OnEnable()
    {
        GameManager.OnEnemyKilled += EnemyKilled;
        UIBoardManager.OnBlockCleared += BlocksCleared;
        GameManager.OnLocationLevelUp += LocationLevelUp;
    }

    void OnDisable()
    {
        GameManager.OnEnemyKilled -= EnemyKilled;
        UIBoardManager.OnBlockCleared -= BlocksCleared;
        GameManager.OnLocationLevelUp -= LocationLevelUp;
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    void QuestCompleted(CurrentQuest quest)
    {
        Debug.Log("Quest Completed: " + quest.Quest.QuestName);
    }

    void BlocksCleared(BlocksClearedEventArgs args)
    {
        foreach (CurrentQuest currentQuest in player.Quests)
        {
            Quest q = currentQuest.Quest;

            for (int i = 0; i < q.QuestObjectives.Count; i++)
            {
                if (currentQuest.Objectives[i].Completed)
                {
                    continue; //Dont need to do any checking if the quest is completed.
                }
                
                QuestObjective qo = q.QuestObjectives[i];

                if (qo as BlockClearObjective)
                {
                    BlockClearObjective bco = (qo as BlockClearObjective);
                    if (args.Color == bco.BlockColorToClear)
                    {
                        currentQuest.Objectives[i].Progress += args.AmountCleared;
                        Debug.Log(bco.ObjectiveString(currentQuest.Objectives[i].Progress));

                        if (currentQuest.Objectives[i].Progress >= bco.AmountToClear)
                        {
                            currentQuest.Objectives[i].Completed = true;
                        }
                    }
                }
                
            }
        }
    }

    void EnemyKilled(Enemy enemy)
    {
        
        foreach (CurrentQuest currentQuest in player.Quests)
        {
            Quest q = currentQuest.Quest;

            for (int i = 0; i < q.QuestObjectives.Count; i++)
            {
                if (currentQuest.Objectives[i].Completed)
                {
                    continue; //Dont need to do any checking if the quest is completed.
                }

                QuestObjective qo = q.QuestObjectives[i]; //?!?!???!
                if (qo as KillObjective)
                {
                    KillObjective ko = (qo as KillObjective);
                    if (ko != null && (ko.AnyTarget || ko.Target == enemy))
                    {
                        currentQuest.Objectives[i].Progress++; //new
                        Debug.Log(ko.ObjectiveString(currentQuest.Objectives[i].Progress));

                        if (currentQuest.Objectives[i].Progress >= ko.Amount)
                        {
                            currentQuest.Objectives[i].Completed = true;
                            if (currentQuest.QuestComplete)
                            {
                                QuestCompleted(currentQuest);
                            }
                        }
                    }
                }
                else if (qo as DropsObjective)
                {
                    DropsObjective dro = qo as DropsObjective;
                    if (dro != null && dro.Target == enemy)
                    {
                        float rnd = Random.Range(0, 100);
                        Debug.Log(q.QuestName + ": Drop [" + dro.ItemName +  "] rolls a " + rnd + " (" + dro.ChanceAtDrop + ")");
                        if (rnd >= 100 - dro.ChanceAtDrop)
                        {
                            currentQuest.Objectives[i].Progress++;
                            Debug.Log(dro.ObjectiveString(currentQuest.Objectives[i].Progress));

                            if (currentQuest.Objectives[i].Progress >= dro.Amount)
                            {
                                currentQuest.Objectives[i].Completed = true;
                            }
                        }
                    }
                }
                
            }
        }
    }

    void LocationLevelUp(int level, Location location)
    {
        foreach (CurrentQuest currentQuest in player.Quests)
        {
            Quest q = currentQuest.Quest;

            for (int i = 0; i < q.QuestObjectives.Count; i++)
            {
                if (currentQuest.Objectives[i].Completed)
                {
                    continue; //Dont need to do any checking if the quest is completed.
                }

                QuestObjective qo = q.QuestObjectives[i];

                if (qo as LocationLevelObjective)
                {
                    LocationLevelObjective llo = (qo as LocationLevelObjective);
                    if (llo.RequiredLocation != location)
                    {
                        continue; //Not the right location
                    }

                    if (level >= currentQuest.Objectives[i].Progress)
                    {
                        currentQuest.Objectives[i].Progress = level;
                        Debug.Log(llo.ObjectiveString(currentQuest.Objectives[i].Progress));

                        if (currentQuest.Objectives[i].Progress >= llo.RequiredLevel)
                        {
                            currentQuest.Objectives[i].Completed = true;
                        }
                    }
                }

            }
        }
    }
}
