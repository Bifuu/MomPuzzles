using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class QuestObjective : ScriptableObject
{
    public abstract string ObjectiveString(float progress);

#if UNITY_EDITOR
    public abstract void DoLayout();
#endif
}

[System.Serializable]
public enum QuestObjectiveType
{
    EnemyKills,
    BlockTypeCleared,
    TotalBlocksCleared
}
