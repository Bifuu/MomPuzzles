using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Linq;

[CustomEditor(typeof(Quest))]
public class ItemEditor : Editor
{
    // Holds QuestObjective types for popup:
    private string[] quesObjectiveNames = new string[0];
    private int questObjIndex = -1;

    private void OnEnable()
    {
        // Fill the popup list:
        Type[] types = Assembly.GetAssembly(typeof(QuestObjective)).GetTypes();
        quesObjectiveNames = (from Type type in types where type.IsSubclassOf(typeof(QuestObjective)) select type.FullName).ToArray();
    }

    public override void OnInspectorGUI()
    {
        var quest = target as Quest;

        quest.QuestName = EditorGUILayout.TextField("Quest Name", quest.QuestName);
        EditorGUILayout.SelectableLabel(quest.ID.ToString());
        quest.MainQuest = EditorGUILayout.Toggle("Is Main Quest", quest.MainQuest);

        EditorGUILayout.LabelField("Quest Description");
        quest.QuestDescription = EditorGUILayout.TextArea(quest.QuestDescription, GUILayout.MaxHeight(200));

        // Draw attributes with a delete button below each one:
        int indexToDelete = -1;
        for (int i = 0; i < quest.QuestObjectives.Count; i++)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            if (quest.QuestObjectives[i] != null) quest.QuestObjectives[i].DoLayout();
            if (GUILayout.Button("Delete")) indexToDelete = i;
            EditorGUILayout.EndVertical();
        }
        if (indexToDelete > -1) quest.QuestObjectives.RemoveAt(indexToDelete);

        // Draw a popup and button to add a new attribute:
        EditorGUILayout.BeginHorizontal();
        questObjIndex = EditorGUILayout.Popup(questObjIndex, quesObjectiveNames);
        if (GUILayout.Button("Add"))
        {
            // A little tricky because we need to record it in the asset database, too:
            
            var newObj = CreateInstance(quesObjectiveNames[questObjIndex]) as QuestObjective;
            newObj.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(newObj, quest);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(newObj));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            quest.QuestObjectives.Add(newObj);
            
        }
        EditorGUILayout.EndHorizontal();

        if (GUI.changed) EditorUtility.SetDirty(quest);
    }

}
