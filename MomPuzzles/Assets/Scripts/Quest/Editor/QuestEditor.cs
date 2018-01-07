using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;

public class QuestEditor : EditorWindow
{
    public QuestDatabase questDatabase;
    string editorPref = "QuestDatabase";
    string questPath = "Assets/Databases/Quests";

    Vector2 scrollPos;
    Quest tempQuest;
    Quest editingQuest;
    int selectedIndex = -1;

    // Holds QuestObjective types for popup:
    private string[] quesObjectiveNames = new string[0];
    private int questObjIndex = -1;

    [MenuItem("Window/Quest Editor")]
    static void Init()
    {
        QuestEditor window = (QuestEditor)EditorWindow.GetWindow(typeof(QuestEditor));
        window.Show();
    }

    void OnEnable()
    {
        
        if (!AssetDatabase.IsValidFolder(questPath))
        {
            AssetDatabase.CreateFolder("Assets/Databases", "Quests");
        }

        if (EditorPrefs.HasKey(editorPref))
        {
            string path = EditorPrefs.GetString(editorPref);
            questDatabase = AssetDatabase.LoadAssetAtPath(path, typeof(QuestDatabase)) as QuestDatabase;
        }


        if (!EditorPrefs.HasKey("NextID"))
        {
            EditorPrefs.SetInt("NextID", 1);
        }

        

        // Fill the popup list:
        Type[] types = Assembly.GetAssembly(typeof(QuestObjective)).GetTypes();
        quesObjectiveNames = (from Type type in types where type.IsSubclassOf(typeof(QuestObjective)) select type.FullName).ToArray();
    }

    void OnDisable()
    {
        ClearTempQuest();
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Quest Editor: ", EditorStyles.boldLabel);
        if (questDatabase)
        {
            if (GUILayout.Button("Loaded"))
            {
                SelectDatabase();
            }
            if (GUILayout.Button("Close"))
            {
                CloseDatabase();
            }
        }
        else
        {
            if (GUILayout.Button("New"))
            {
                questDatabase = NewDatabase();
            }
            if (GUILayout.Button("Load"))
            {
                OpenDatabase();
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        //Edit fields for quest stuff.
        if (questDatabase)
        {
            EditorGUILayout.BeginHorizontal();
            //List of Quests
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(100));
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, EditorStyles.textArea, GUILayout.Width(100));

            for (int i = 0; i < questDatabase.Quests.Count; i++)
            {
                Quest q = questDatabase.Quests[i];
                if (GUILayout.Button(q.name))
                {
                    //tempQuest.Clone(q);
                    selectedIndex = i;
                    editingQuest = q;
                }
            }



            EditorGUILayout.EndScrollView();
            if (GUILayout.Button("New"))
            {
                //Create a new TempQuest.
                selectedIndex = -1;
                CreateTempQuest();
                editingQuest = tempQuest;
            }
            EditorGUILayout.EndVertical();



            
            

            if (editingQuest)
            {
                //MAin Editor area
                EditorGUILayout.BeginVertical();
                EditorGUI.BeginChangeCheck();

                editingQuest.QuestName = EditorGUILayout.TextField("Quest Name:", editingQuest.QuestName);
                EditorGUILayout.SelectableLabel(editingQuest.ID.ToString());
                editingQuest.MainQuest = EditorGUILayout.Toggle("Is Main Quest:", editingQuest.MainQuest);
                GUILayout.Label("Quest Description:");
                editingQuest.QuestDescription = EditorGUILayout.TextArea(editingQuest.QuestDescription, GUILayout.Height(100));

                int indexToDelete = -1;
                for (int i = 0; i < editingQuest.QuestObjectives.Count; i++)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    if (editingQuest.QuestObjectives[i] != null) editingQuest.QuestObjectives[i].DoLayout();
                    if (GUILayout.Button("Delete")) indexToDelete = i;
                    EditorGUILayout.EndVertical();
                }
                if (indexToDelete > -1)
                {
                    editingQuest.QuestObjectives.RemoveAt(indexToDelete);
                }
            


                // Draw a popup and button to add a new attribute:
                EditorGUILayout.BeginHorizontal();
                questObjIndex = EditorGUILayout.Popup(questObjIndex, quesObjectiveNames);
                if (GUILayout.Button("Add"))
                {
                    if (questObjIndex > -1)
                    {
                        // A little tricky because we need to record it in the asset database, too:
                        var newObj = CreateInstance(quesObjectiveNames[questObjIndex]) as QuestObjective;
                        newObj.hideFlags = HideFlags.HideInHierarchy;
                        AssetDatabase.AddObjectToAsset(newObj, editingQuest);
                        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(newObj));
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        editingQuest.QuestObjectives.Add(newObj);
                    }
                }
                EditorGUILayout.EndHorizontal();

                editingQuest.RewardedExperience = EditorGUILayout.IntField("Rewarded Experience", editingQuest.RewardedExperience);


                EditorGUILayout.Space();
                EditorGUILayout.Separator();


                //Required Quests
                int requiredIndexToDel = -1;
                for (int i = 0; i < editingQuest.RequiredQuests.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    editingQuest.RequiredQuests[i] = EditorGUILayout.ObjectField(editingQuest.RequiredQuests[i], typeof(Quest), true) as Quest;
                    if (GUILayout.Button("Delete")) requiredIndexToDel = i;
                    EditorGUILayout.EndHorizontal();
                }
                if (requiredIndexToDel > -1)
                {
                    editingQuest.RequiredQuests.RemoveAt(requiredIndexToDel);
                }
                if (GUILayout.Button("Add Required Quest"))
                {
                    editingQuest.RequiredQuests.Add(null);
                }

                EditorGUILayout.Space();
                EditorGUILayout.Separator();

                //Rewared Items
                int rewardIndexToDel = -1;
                for (int i = 0; i < editingQuest.RewardedItems.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    editingQuest.RewardedItems[i] = EditorGUILayout.ObjectField(editingQuest.RewardedItems[i], typeof(Equippable), true) as Equippable;
                    if (GUILayout.Button("Delete")) rewardIndexToDel = i;
                    EditorGUILayout.EndHorizontal();
                }
                if (rewardIndexToDel > -1)
                {
                    editingQuest.RewardedItems.RemoveAt(rewardIndexToDel);
                }
                if (GUILayout.Button("Add Rewarded Item"))
                {
                    editingQuest.RewardedItems.Add(null);
                }


                //Nothing below here for Quest editing stuff
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(editingQuest);
                }
                EditorGUILayout.EndVertical();

                //if (GUI.changed) EditorUtility.SetDirty(editingQuest);
            }
            EditorGUILayout.EndHorizontal();

            
        }

        //StatusBar
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.Height(25));
        GUILayout.Label("Ready");
        if (tempQuest)
        {
            if (GUILayout.Button("Save"))
            {
                SaveNewAsset();
            }
            if (GUILayout.Button("Cancel"))
            {
                ClearTempQuest();
            }
        }        
        if (selectedIndex > -1)
        {
            if (GUILayout.Button("Delete"))
            {
                if (EditorUtility.DisplayDialog("Delete Quest?", "Are you sure you want to delete this?", "Delete"))
                {
                    string questToBeDestroyed = AssetDatabase.GetAssetPath(questDatabase.Quests[selectedIndex]);
                    questDatabase.Quests.RemoveAt(selectedIndex);
                    EditorUtility.SetDirty(questDatabase);
                    AssetDatabase.DeleteAsset(questToBeDestroyed);
                    editingQuest = null;
                    selectedIndex = -1;
                }
            }
        }
        
        EditorGUILayout.EndHorizontal();
        
    }





    void SaveNewAsset()
    {
        string assetName = tempQuest.QuestName.Replace(" ", string.Empty);
        char[] arr = assetName.Where(c => (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '-')).ToArray();
        assetName = new string(arr);
        string newAssetPath = questPath + "/" + assetName + ".asset";
        tempQuest.ID = GetNextID();
        AssetDatabase.CopyAsset(questPath + "/_temp.asset", newAssetPath);
        AssetDatabase.SaveAssets();
        Quest newQuest = AssetDatabase.LoadAssetAtPath(newAssetPath, typeof(Quest)) as Quest;
        questDatabase.Quests.Add(newQuest);
        EditorUtility.SetDirty(questDatabase);
        selectedIndex = questDatabase.Quests.Count - 1;
        ClearTempQuest();
    }

    void CreateTempQuest()
    {
        //Make new temp quest holder
        tempQuest = ScriptableObject.CreateInstance<Quest>();
        //string path = "Assets/Databases/FlagDatabase.asset";
        AssetDatabase.CreateAsset(tempQuest, questPath + "/_temp.asset");
        AssetDatabase.SaveAssets();
    }

    void ClearTempQuest()
    {
        //Delete the Temp
        string questToBeDestroyed = AssetDatabase.GetAssetPath(tempQuest);
        AssetDatabase.DeleteAsset(questToBeDestroyed);

        tempQuest = null;
    }

    int GetNextID()
    {
        int i = EditorPrefs.GetInt("NextID");
        EditorPrefs.SetInt("NextID", i + 1);
        return i;
    }

    QuestDatabase NewDatabase()
    {
        Debug.Log("New Quest Database");
        QuestDatabase asset = ScriptableObject.CreateInstance<QuestDatabase>();

        string path = "Assets/Databases/" + editorPref + ".asset";
        AssetDatabase.CreateAsset(asset, path);
        asset.Quests = new List<Quest>();
        AssetDatabase.SaveAssets();

        EditorPrefs.SetString(editorPref, path);
        return asset;
    }

    void OpenDatabase()
    {
        string absPath = EditorUtility.OpenFilePanel("Select Database", "", "");
        Debug.Log(absPath);

        if (absPath.StartsWith(Application.dataPath))
        {
            string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
            questDatabase = AssetDatabase.LoadAssetAtPath(relPath, typeof(QuestDatabase)) as QuestDatabase;
            if (questDatabase.Quests == null)
            {
                questDatabase.Quests = new List<Quest>();
                EditorUtility.SetDirty(questDatabase);
            }
            if (questDatabase)
            {
                Debug.Log(relPath);
                EditorPrefs.SetString(editorPref, relPath);
            }
        }
    }

    void CloseDatabase()
    {
        questDatabase = null;
        if (EditorPrefs.HasKey(editorPref))
        {
            EditorPrefs.DeleteKey(editorPref);
        }
    }

    void SelectDatabase()
    {
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = questDatabase;
    }
}
