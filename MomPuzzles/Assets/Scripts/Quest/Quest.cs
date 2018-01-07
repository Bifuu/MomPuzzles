using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Quest.asset", menuName = "Puzzle RPG/Quest/Quest")]
public class Quest : ScriptableObject
{
    public int ID = 0;
    public bool MainQuest = false;
    public string QuestName = "Quest Name";
    public string QuestDescription = "Quest description";
    public List<QuestObjective> QuestObjectives = new List<QuestObjective>();
    public List<Quest> RequiredQuests = new List<Quest>();
    public List<Equippable> RewardedItems = new List<Equippable>();
    public int RewardedExperience = 0;

    public void Clone(Quest questToClone)
    {
        ID = questToClone.ID;
        MainQuest = questToClone.MainQuest;
        QuestName = questToClone.QuestName;
        QuestDescription = questToClone.QuestDescription;
        QuestObjectives = questToClone.QuestObjectives;
    }
}
