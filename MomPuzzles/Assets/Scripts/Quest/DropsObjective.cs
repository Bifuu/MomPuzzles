using UnityEngine;
using System.Collections;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DropsObjective : QuestObjective {
    public Enemy Target;
    public float ChanceAtDrop;
    public int Amount;
    public string ItemName;

    public override string ObjectiveString(float progress)
    {
        string str;
        str = string.Format("{0} / {1} {2}{3} collected.", progress, Amount, ItemName, Amount > 1 ? "s" : "");

        return str;
    }

#if UNITY_EDITOR
    public override void DoLayout()
    {
        ItemName = EditorGUILayout.TextField("Item Name", ItemName);
        Amount = EditorGUILayout.IntField("Amount", Amount);
        Target = (Enemy)EditorGUILayout.ObjectField("Enemy", Target, typeof(Enemy), true);
        ChanceAtDrop = EditorGUILayout.Slider("Chance of Drop", ChanceAtDrop, 0f, 100f);
    }
#endif
}
