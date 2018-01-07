using UnityEngine;
using System.Collections;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "KillObjective.asset", menuName = "Puzzle RPG/Quest/Objectives/Kill")]
public class KillObjective : QuestObjective {

    public Enemy Target;
    public int Amount;
    public bool AnyTarget = false;

    public override string ObjectiveString(float progress)
    {
        string str;
        str = string.Format("{0} / {1} {2}{3} killed.", progress, Amount, Target.Name, Amount > 1 ? "s":"");

        return str;
    }

#if UNITY_EDITOR
    public override void DoLayout()
    {
        AnyTarget = EditorGUILayout.Toggle("Any target?", AnyTarget);
        if (!AnyTarget) Target = (Enemy)EditorGUILayout.ObjectField("Enemy", Target, typeof(Enemy), true);
        Amount = EditorGUILayout.IntField("Amount", Amount);
    }
#endif
}
