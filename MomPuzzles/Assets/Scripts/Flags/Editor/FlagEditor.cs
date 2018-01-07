using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class FlagEditor : EditorWindow {

    public FlagDatabase flagDatabase;
    List<Flag> displayedFlags;
    Flag flag;
    string newName;

	[MenuItem ("Window/Flag Editor")]
    static void Init()
    {
        FlagEditor window = (FlagEditor)EditorWindow.GetWindow(typeof(FlagEditor));
        window.Show();
    }

    void OnEnable()
    {
        if (EditorPrefs.HasKey("FlagDatabase"))
        {
            string path = EditorPrefs.GetString("FlagDatabase");
            flagDatabase = AssetDatabase.LoadAssetAtPath(path, typeof(FlagDatabase)) as FlagDatabase;
        }
        newName = "";
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Flag Editor: ", EditorStyles.boldLabel);
        if (flagDatabase)
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
                flagDatabase = NewDatabase();
            }
            if (GUILayout.Button("Load"))
            {
                OpenDatabase();
            }
        }
        EditorGUILayout.EndHorizontal();

        

        if (flagDatabase)
        {
            EditorGUILayout.TextField("Search");


            EditorGUILayout.Space();
            //Create a flag
            EditorGUILayout.BeginHorizontal();

            newName = EditorGUILayout.TextField(newName);
            if (GUILayout.Button("+"))
            {
                Flag newFlag = ScriptableObject.CreateInstance<Flag>();
                string path = "Assets/Databases/Flags";
                if (!AssetDatabase.IsValidFolder(path))
                {
                    AssetDatabase.CreateFolder("Assets/Databases", "Flags");
                }
                AssetDatabase.CreateAsset(newFlag, path + "/" + newName + ".asset");
                AssetDatabase.SaveAssets();
                newFlag.Name = newName;
                if (flagDatabase.Flags.Count > 0)
                {
                    newFlag.ID = flagDatabase.Flags[flagDatabase.Flags.Count - 1].ID + 1;
                }
                else { newFlag.ID = 1; }

                flagDatabase.Flags.Add(newFlag);
                
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            int indexToDelete = -1;
            for (int i = 0; i < flagDatabase.Flags.Count; i++)
            {
                Flag currentFlag = flagDatabase.Flags[i];
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.ObjectField(currentFlag, typeof(Flag), false);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(currentFlag.ID.ToString());
                if (GUILayout.Button("Edit"))
                {

                }
                if (GUILayout.Button("X"))
                {
                    if (EditorUtility.DisplayDialog("Delete Flag?", "Do you want to delete flag: " + currentFlag.Name, "Delete"))
                        indexToDelete = i;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();

                if (indexToDelete > -1)
                {
                    string flagToBeDestroyed = AssetDatabase.GetAssetPath(flagDatabase.Flags[indexToDelete]);
                    flagDatabase.Flags.RemoveAt(indexToDelete);
                    AssetDatabase.DeleteAsset(flagToBeDestroyed);
                }
            }
        }
       
        
    }

    FlagDatabase NewDatabase()
    {
        FlagDatabase asset = ScriptableObject.CreateInstance<FlagDatabase>();

        string path = "Assets/Databases/FlagDatabase.asset";
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();

        EditorPrefs.SetString("FlagDatabase", path);
        return asset;
    }

    void OpenDatabase()
    {
        string absPath = EditorUtility.OpenFilePanel("Select Database", "", "");
        Debug.Log(absPath);

        if (absPath.StartsWith(Application.dataPath))
        {
            string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
            flagDatabase = AssetDatabase.LoadAssetAtPath(relPath, typeof(FlagDatabase)) as FlagDatabase;
            if (flagDatabase.Flags == null)
            {
                flagDatabase.Flags = new List<Flag>();
            }
            if (flagDatabase)
            {
                Debug.Log(relPath);
                EditorPrefs.SetString("FlagDatabase", relPath);
            }
        }
    }

    void CloseDatabase()
    {
        flagDatabase = null;
        if (EditorPrefs.HasKey("FlagDatabase"))
        {
            EditorPrefs.DeleteKey("FlagDatabase");
        }
    }

    void SelectDatabase()
    {
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = flagDatabase;
    }
}
