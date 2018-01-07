using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;

public class EnemyEditor : EditorWindow
{
    public EnemyDatabase enemyDatabase;
    string editorPref = "EnemyDatabase";
    string enemyPath = "Assets/Databases/Enemies";

    Vector2 scrollPos;
    Enemy tempEnemy;
    Enemy editingEnemy;
    int selectedIndex = -1;

    float AttackDamageMinLimit = 0;
    float AttackDamageMaxLimit = 1000;

    // Holds QuestObjective types for popup:
    private string[] quesObjectiveNames = new string[0];
    private int questObjIndex = -1;

    [MenuItem("Window/Enemy Editor")]
    static void Init()
    {
        EnemyEditor window = (EnemyEditor)EditorWindow.GetWindow(typeof(EnemyEditor));
        window.Show();
    }

    void OnEnable()
    {

        if (!AssetDatabase.IsValidFolder(enemyPath))
        {
            AssetDatabase.CreateFolder("Assets/Databases", "Enemies");
        }

        if (EditorPrefs.HasKey(editorPref))
        {
            string path = EditorPrefs.GetString(editorPref);
            enemyDatabase = AssetDatabase.LoadAssetAtPath(path, typeof(EnemyDatabase)) as EnemyDatabase;
        }


        if (!EditorPrefs.HasKey("NextEnemyID"))
        {
            EditorPrefs.SetInt("NextEnemyID", 1);
        }
    }

    void OnDisable()
    {
        ClearTempEnemy();
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Enemy Editor: ", EditorStyles.boldLabel);
        if (enemyDatabase)
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
                enemyDatabase = NewDatabase();
            }
            if (GUILayout.Button("Load"))
            {
                OpenDatabase();
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        //Edit fields for quest stuff.
        if (enemyDatabase)
        {
            EditorGUILayout.BeginHorizontal();
            //List of Enemies
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(100));
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, EditorStyles.textArea, GUILayout.Width(100));

            for (int i = 0; i < enemyDatabase.DB.Count; i++)
            {
                Enemy e = enemyDatabase.DB[i];
                if (GUILayout.Button(e.name))
                {
                    //tempQuest.Clone(q);
                    selectedIndex = i;
                    editingEnemy = e;
                }
            }



            EditorGUILayout.EndScrollView();
            if (GUILayout.Button("New"))
            {
                //Create a new TempQuest.
                selectedIndex = -1;
                CreateTempEnemy();
                editingEnemy = tempEnemy;
            }
            EditorGUILayout.EndVertical();

            if (editingEnemy)
            {
                //MAin Editor area
                EditorGUILayout.BeginVertical();
                EditorGUI.BeginChangeCheck();

                editingEnemy.Name = EditorGUILayout.TextField("Enemy Name:", editingEnemy.Name);
                EditorGUILayout.SelectableLabel(editingEnemy.ID.ToString());
                editingEnemy.Level = EditorGUILayout.IntField("Level:", editingEnemy.Level);
                editingEnemy.BaseHealth = EditorGUILayout.IntField("Health:", editingEnemy.BaseHealth);
                editingEnemy.AttackDamageMin = EditorGUILayout.IntField("Min Damage:", editingEnemy.AttackDamageMin);
                editingEnemy.AttackDamageMax = EditorGUILayout.IntField("Max Damage:", editingEnemy.AttackDamageMax);
                editingEnemy.AttackSpeed = EditorGUILayout.IntField("Attack Speed:", editingEnemy.AttackSpeed);
                editingEnemy.EnemySprite = EditorGUILayout.ObjectField("Enemy Sprite:", editingEnemy.EnemySprite, typeof(Sprite), true) as Sprite;
                editingEnemy.HurtSprite = EditorGUILayout.ObjectField("Hurt Sprite:", editingEnemy.HurtSprite, typeof(Sprite), true) as Sprite;

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(editingEnemy);
                }
                EditorGUILayout.EndVertical();

                //if (GUI.changed) EditorUtility.SetDirty(editingQuest);
            }
            EditorGUILayout.EndHorizontal();


        }

        //StatusBar
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.Height(25));
        GUILayout.Label("Ready");
        if (tempEnemy)
        {
            if (GUILayout.Button("Save"))
            {
                SaveNewAsset();
            }
            if (GUILayout.Button("Cancel"))
            {
                ClearTempEnemy();
            }
        }
        
        if (selectedIndex > -1)
        {
            if (GUILayout.Button("Delete"))
            {
                if (EditorUtility.DisplayDialog("Delete Enemy?", "Are you sure you want to delete this?", "Delete"))
                {
                    string questToBeDestroyed = AssetDatabase.GetAssetPath(enemyDatabase.DB[selectedIndex]);
                    enemyDatabase.DB.RemoveAt(selectedIndex);
                    EditorUtility.SetDirty(enemyDatabase);
                    AssetDatabase.DeleteAsset(questToBeDestroyed);
                    editingEnemy = null;
                    selectedIndex = -1;
                }
            }
        }

        EditorGUILayout.EndHorizontal();

    }





    void SaveNewAsset()
    {
        string assetName = tempEnemy.Name.Replace(" ", string.Empty);
        string newAssetPath = enemyPath + "/" + assetName + ".asset";
        tempEnemy.ID = GetNextID();
        AssetDatabase.CopyAsset(enemyPath + "/_temp.asset", newAssetPath);
        AssetDatabase.SaveAssets();
        Enemy newEnemy = AssetDatabase.LoadAssetAtPath(newAssetPath, typeof(Enemy)) as Enemy;
        enemyDatabase.DB.Add(newEnemy);
        EditorUtility.SetDirty(enemyDatabase);
        selectedIndex = enemyDatabase.DB.Count - 1;
        ClearTempEnemy();
    }

    void CreateTempEnemy()
    {
        //Make new temp quest holder
        tempEnemy = ScriptableObject.CreateInstance<Enemy>();
        //string path = "Assets/Databases/FlagDatabase.asset";
        AssetDatabase.CreateAsset(tempEnemy, enemyPath + "/_temp.asset");
        AssetDatabase.SaveAssets();
    }

    void ClearTempEnemy()
    {
        //Delete the Temp
        string questToBeDestroyed = AssetDatabase.GetAssetPath(tempEnemy);
        AssetDatabase.DeleteAsset(questToBeDestroyed);

        tempEnemy = null;
    }

    int GetNextID()
    {
        int i = EditorPrefs.GetInt("NextEnemyID");
        EditorPrefs.SetInt("NextEnemyID", i + 1);
        return i;
    }

    EnemyDatabase NewDatabase()
    {
        Debug.Log("New Enemy Database");
        EnemyDatabase asset = ScriptableObject.CreateInstance<EnemyDatabase>();

        string path = "Assets/Databases/" + editorPref + ".asset";
        AssetDatabase.CreateAsset(asset, path);
        asset.DB = new List<Enemy>();
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
            enemyDatabase = AssetDatabase.LoadAssetAtPath(relPath, typeof(EnemyDatabase)) as EnemyDatabase;
            if (enemyDatabase.DB == null)
            {
                enemyDatabase.DB = new List<Enemy>();
                EditorUtility.SetDirty(enemyDatabase);
            }
            if (enemyDatabase)
            {
                Debug.Log(relPath);
                EditorPrefs.SetString(editorPref, relPath);
            }
        }
    }

    void CloseDatabase()
    {
        enemyDatabase = null;
        if (EditorPrefs.HasKey(editorPref))
        {
            EditorPrefs.DeleteKey(editorPref);
        }
    }

    void SelectDatabase()
    {
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = enemyDatabase;
    }
}
