using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;

public class GenericEditor<T> : EditorWindow {

    //EditorPref String
    //
    public ScriptableObjectDatabase database;
    Type databaseType;
    string editorPref = "";
    string databaseFolder = "";
    string asssetDatabasePath = "Assets/Database/";
    string databaseFullPath = "Assets/Database/";

    public virtual void Init(string editorPrefName, string DBPath, Type DBType)
    {
        editorPref = editorPrefName;
        databaseFolder = DBPath;
        databaseType = DBType;
        databaseFullPath = asssetDatabasePath + databaseFolder;
    }

    void OnEnable()
    {

        if (!AssetDatabase.IsValidFolder(databaseFullPath))
        {
            AssetDatabase.CreateFolder(asssetDatabasePath, databaseFolder);
        }

        if (EditorPrefs.HasKey(editorPref))
        {
            string path = EditorPrefs.GetString(editorPref);
            //database = AssetDatabase.LoadAssetAtPath(path, databaseType);
        }


        if (!EditorPrefs.HasKey("NextEnemyID"))
        {
            EditorPrefs.SetInt("NextEnemyID", 1);
        }
    }
}
