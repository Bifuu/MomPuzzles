using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LocationLevelObjective : QuestObjective {

    public Location RequiredLocation;
    public int RequiredLevel;

#if UNITY_EDITOR
    public override void DoLayout()
    {
        RequiredLocation = (Location)EditorGUILayout.ObjectField("Location", RequiredLocation, typeof(Location), true);
        RequiredLevel = EditorGUILayout.IntField("Level", RequiredLevel);
    }
#endif

    public override string ObjectiveString(float progress)
    {
        string str;
        str = string.Format("Reach Level {0} in the {1}. (Current Level {2}", RequiredLevel, RequiredLocation.Name, progress);

        return str;
    }
}
