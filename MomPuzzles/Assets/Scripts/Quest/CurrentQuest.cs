using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CurrentQuest {

    public Quest Quest;
    //public List<float> Progress;
    public List<CurrentQuestObjective> Objectives;

    public CurrentQuest(Quest quest)
    {
        this.Quest = quest;

        //Progress = new List<float>();
        Objectives = new List<CurrentQuestObjective>();
        for (int i = 0; i < Quest.QuestObjectives.Count; i++)
        {
            //Progress.Add(0f);
            Objectives.Add(new CurrentQuestObjective());
        }
    }

    public bool QuestComplete
    {
        get
        {
            
            return (Objectives.TrueForAll(q => q.Completed));
        }
    }
}
