using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ArmorEditor : EditorWindow
{

    public ArmorDatabase armorDatabase;
    string editorPref = "ArmorDatabase";
    string idPref = "NextArmorID";
    string armorPath = "Assets/Databases/Armor";

    Vector2 scrollPos;
    Armor tempArmor;
    Armor editingArmor;
    int selectedIndex = -1;

    float AttackDamageMinLimit = 0;
    float AttackDamageMaxLimit = 1000;

    [MenuItem("Window/Puzzle RPG/Armor Editor")]
    static void Init()
    {
        ArmorEditor window = (ArmorEditor)EditorWindow.GetWindow(typeof(ArmorEditor));
        window.Show();
    }

    void OnEnable()
    {

        if (!AssetDatabase.IsValidFolder(armorPath))
        {
            AssetDatabase.CreateFolder("Assets/Databases", "Armor");
        }

        if (EditorPrefs.HasKey(editorPref))
        {
            string path = EditorPrefs.GetString(editorPref);
            armorDatabase = AssetDatabase.LoadAssetAtPath(path, typeof(ArmorDatabase)) as ArmorDatabase;
        }


        if (!EditorPrefs.HasKey(idPref))
        {
            EditorPrefs.SetInt(idPref, 1);
        }
    }

    void OnDisable()
    {
        ClearTempWeapon();
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Weapon Editor: ", EditorStyles.boldLabel);
        if (armorDatabase)
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
                armorDatabase = NewDatabase();
            }
            if (GUILayout.Button("Load"))
            {
                OpenDatabase();
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        //Edit fields for quest stuff.
        if (armorDatabase)
        {
            EditorGUILayout.BeginHorizontal();
            //List of Weapons
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(100));
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, EditorStyles.textArea, GUILayout.Width(100));

            for (int i = 0; i < armorDatabase.DB.Count; i++)
            {
                Armor a = armorDatabase.DB[i];
                if (GUILayout.Button(a.name))
                {
                    //tempQuest.Clone(q);
                    selectedIndex = i;
                    editingArmor = a;
                }
            }



            EditorGUILayout.EndScrollView();
            if (GUILayout.Button("New"))
            {
                //Create a new TempQuest.
                selectedIndex = -1;
                CreateTempWeapon();
                editingArmor = tempArmor;
            }
            EditorGUILayout.EndVertical();

            if (editingArmor)
            {
                //MAin Editor area
                EditorGUILayout.BeginVertical();
                EditorGUI.BeginChangeCheck();

                editingArmor.ItemName = EditorGUILayout.TextField("Weapon Name:", editingArmor.ItemName);
                EditorGUILayout.SelectableLabel(editingArmor.ID.ToString());
                editingArmor.LevelRequirement = EditorGUILayout.IntField("Required Level:", editingArmor.LevelRequirement);
                editingArmor.DamageBlock = EditorGUILayout.IntField("Damage Block:", editingArmor.DamageBlock);
                editingArmor.Attunement = (BlockColor)EditorGUILayout.EnumPopup("Atunement:", editingArmor.Attunement);
                editingArmor.Rarity = (ItemRarity)EditorGUILayout.ObjectField("Rarity", editingArmor.Rarity, typeof(ItemRarity), false);
                editingArmor.IconSprite = EditorGUILayout.ObjectField("Enemy Sprite:", editingArmor.IconSprite, typeof(Sprite), true) as Sprite;

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(editingArmor);
                }
                EditorGUILayout.EndVertical();

                //if (GUI.changed) EditorUtility.SetDirty(editingQuest);
            }
            EditorGUILayout.EndHorizontal();


        }

        //StatusBar
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.Height(25));
        GUILayout.Label("Ready");
        if (tempArmor)
        {
            if (GUILayout.Button("Save"))
            {
                SaveNewAsset();
            }
            if (GUILayout.Button("Cancel"))
            {
                ClearTempWeapon();
            }
        }

        if (selectedIndex > -1)
        {
            if (GUILayout.Button("Delete"))
            {
                if (EditorUtility.DisplayDialog("Delete Enemy?", "Are you sure you want to delete this?", "Delete"))
                {
                    string armorToBeDestroyed = AssetDatabase.GetAssetPath(armorDatabase.DB[selectedIndex]);
                    armorDatabase.DB.RemoveAt(selectedIndex);
                    EditorUtility.SetDirty(armorDatabase);
                    AssetDatabase.DeleteAsset(armorToBeDestroyed);
                    editingArmor = null;
                    selectedIndex = -1;
                }
            }
        }

        EditorGUILayout.EndHorizontal();

    }





    void SaveNewAsset()
    {
        string assetName = tempArmor.ItemName.Replace(" ", string.Empty);
        string newAssetPath = armorPath + "/" + assetName + ".asset";
        tempArmor.ID = GetNextID();
        AssetDatabase.CopyAsset(armorPath + "/_temp.asset", newAssetPath);
        AssetDatabase.SaveAssets();
        Armor newArmor = AssetDatabase.LoadAssetAtPath(newAssetPath, typeof(Armor)) as Armor;
        armorDatabase.DB.Add(newArmor);
        EditorUtility.SetDirty(armorDatabase);
        selectedIndex = armorDatabase.DB.Count - 1;
        ClearTempWeapon();
    }

    void CreateTempWeapon()
    {
        //Make new temp quest holder
        tempArmor = ScriptableObject.CreateInstance<Armor>();
        //string path = "Assets/Databases/FlagDatabase.asset";
        AssetDatabase.CreateAsset(tempArmor, armorPath + "/_temp.asset");
        AssetDatabase.SaveAssets();
    }

    void ClearTempWeapon()
    {
        //Delete the Temp
        string questToBeDestroyed = AssetDatabase.GetAssetPath(tempArmor);
        AssetDatabase.DeleteAsset(questToBeDestroyed);

        tempArmor = null;
    }

    int GetNextID()
    {
        int i = EditorPrefs.GetInt(idPref);
        EditorPrefs.SetInt(idPref, i + 1);
        return i;
    }

    ArmorDatabase NewDatabase()
    {
        Debug.Log("New Weapon Database");
        ArmorDatabase asset = ScriptableObject.CreateInstance<ArmorDatabase>();

        string path = "Assets/Databases/" + editorPref + ".asset";
        AssetDatabase.CreateAsset(asset, path);
        asset.DB = new List<Armor>();
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
            armorDatabase = AssetDatabase.LoadAssetAtPath(relPath, typeof(ArmorDatabase)) as ArmorDatabase;
            if (armorDatabase.DB == null)
            {
                armorDatabase.DB = new List<Armor>();
                EditorUtility.SetDirty(armorDatabase);
            }
            if (armorDatabase)
            {
                Debug.Log(relPath);
                EditorPrefs.SetString(editorPref, relPath);
            }
        }
    }

    void CloseDatabase()
    {
        armorDatabase = null;
        if (EditorPrefs.HasKey(editorPref))
        {
            EditorPrefs.DeleteKey(editorPref);
        }
    }

    void SelectDatabase()
    {
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = armorDatabase;
    }
}
