using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WeaponEditor : EditorWindow
{

    public WeaponDatabase weaponDatabase;
    string editorPref = "WeaponDatabase";
    string idPref = "NextWeaponID";
    string weaponPath = "Assets/Databases/Weapons";

    Vector2 scrollPos;
    Weapon tempWeapon;
    Weapon editingWeapon;
    int selectedIndex = -1;

    float AttackDamageMinLimit = 0;
    float AttackDamageMaxLimit = 1000;

    [MenuItem("Window/Puzzle RPG/Weapon Editor")]
    static void Init()
    {
        WeaponEditor window = (WeaponEditor)EditorWindow.GetWindow(typeof(WeaponEditor));
        window.Show();
    }

    void OnEnable()
    {

        if (!AssetDatabase.IsValidFolder(weaponPath))
        {
            AssetDatabase.CreateFolder("Assets/Databases", "Weapons");
        }

        if (EditorPrefs.HasKey(editorPref))
        {
            string path = EditorPrefs.GetString(editorPref);
            weaponDatabase = AssetDatabase.LoadAssetAtPath(path, typeof(WeaponDatabase)) as WeaponDatabase;
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
        if (weaponDatabase)
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
                weaponDatabase = NewDatabase();
            }
            if (GUILayout.Button("Load"))
            {
                OpenDatabase();
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        //Edit fields for quest stuff.
        if (weaponDatabase)
        {
            EditorGUILayout.BeginHorizontal();
            //List of Weapons
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(100));
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, EditorStyles.textArea, GUILayout.Width(100));

            for (int i = 0; i < weaponDatabase.DB.Count; i++)
            {
                Weapon w = weaponDatabase.DB[i];
                if (GUILayout.Button(w.name))
                {
                    //tempQuest.Clone(q);
                    selectedIndex = i;
                    editingWeapon = w;
                }
            }



            EditorGUILayout.EndScrollView();
            if (GUILayout.Button("New"))
            {
                //Create a new TempQuest.
                selectedIndex = -1;
                CreateTempWeapon();
                editingWeapon = tempWeapon;
            }
            EditorGUILayout.EndVertical();

            if (editingWeapon)
            {
                //MAin Editor area
                EditorGUILayout.BeginVertical();
                EditorGUI.BeginChangeCheck();

                editingWeapon.ItemName = EditorGUILayout.TextField("Weapon Name:", editingWeapon.ItemName);
                EditorGUILayout.SelectableLabel(editingWeapon.ID.ToString());
                editingWeapon.LevelRequirement = EditorGUILayout.IntField("Required Level:", editingWeapon.LevelRequirement);
                editingWeapon.MinDamage = EditorGUILayout.IntField("Min Damage:", editingWeapon.MinDamage);
                editingWeapon.MaxDamage = EditorGUILayout.IntField("Max Damage:", editingWeapon.MaxDamage);
                editingWeapon.BlockClearModifier = EditorGUILayout.IntField("Clear Block Modifier:", editingWeapon.BlockClearModifier);
                editingWeapon.Attunement = (BlockColor)EditorGUILayout.EnumPopup("Atunement:", editingWeapon.Attunement);
                editingWeapon.Rarity = (ItemRarity)EditorGUILayout.ObjectField("Rarity", editingWeapon.Rarity, typeof(ItemRarity), false);
                editingWeapon.IconSprite = EditorGUILayout.ObjectField("Enemy Sprite:", editingWeapon.IconSprite, typeof(Sprite), true) as Sprite;

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(editingWeapon);
                }
                EditorGUILayout.EndVertical();

                //if (GUI.changed) EditorUtility.SetDirty(editingQuest);
            }
            EditorGUILayout.EndHorizontal();


        }

        //StatusBar
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.Height(25));
        GUILayout.Label("Ready");
        if (tempWeapon)
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
                    string questToBeDestroyed = AssetDatabase.GetAssetPath(weaponDatabase.DB[selectedIndex]);
                    weaponDatabase.DB.RemoveAt(selectedIndex);
                    EditorUtility.SetDirty(weaponDatabase);
                    AssetDatabase.DeleteAsset(questToBeDestroyed);
                    editingWeapon = null;
                    selectedIndex = -1;
                }
            }
        }

        EditorGUILayout.EndHorizontal();

    }





    void SaveNewAsset()
    {
        string assetName = tempWeapon.ItemName.Replace(" ", string.Empty);
        string newAssetPath = weaponPath + "/" + assetName + ".asset";
        tempWeapon.ID = GetNextID();
        AssetDatabase.CopyAsset(weaponPath + "/_temp.asset", newAssetPath);
        AssetDatabase.SaveAssets();
        Weapon newEnemy = AssetDatabase.LoadAssetAtPath(newAssetPath, typeof(Weapon)) as Weapon;
        weaponDatabase.DB.Add(newEnemy);
        EditorUtility.SetDirty(weaponDatabase);
        selectedIndex = weaponDatabase.DB.Count - 1;
        ClearTempWeapon();
    }

    void CreateTempWeapon()
    {
        //Make new temp quest holder
        tempWeapon = ScriptableObject.CreateInstance<Weapon>();
        //string path = "Assets/Databases/FlagDatabase.asset";
        AssetDatabase.CreateAsset(tempWeapon, weaponPath + "/_temp.asset");
        AssetDatabase.SaveAssets();
    }

    void ClearTempWeapon()
    {
        //Delete the Temp
        string questToBeDestroyed = AssetDatabase.GetAssetPath(tempWeapon);
        AssetDatabase.DeleteAsset(questToBeDestroyed);

        tempWeapon = null;
    }

    int GetNextID()
    {
        int i = EditorPrefs.GetInt(idPref);
        EditorPrefs.SetInt(idPref, i + 1);
        return i;
    }

    WeaponDatabase NewDatabase()
    {
        Debug.Log("New Weapon Database");
        WeaponDatabase asset = ScriptableObject.CreateInstance<WeaponDatabase>();

        string path = "Assets/Databases/" + editorPref + ".asset";
        AssetDatabase.CreateAsset(asset, path);
        asset.DB = new List<Weapon>();
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
            weaponDatabase = AssetDatabase.LoadAssetAtPath(relPath, typeof(WeaponDatabase)) as WeaponDatabase;
            if (weaponDatabase.DB == null)
            {
                weaponDatabase.DB = new List<Weapon>();
                EditorUtility.SetDirty(weaponDatabase);
            }
            if (weaponDatabase)
            {
                Debug.Log(relPath);
                EditorPrefs.SetString(editorPref, relPath);
            }
        }
    }

    void CloseDatabase()
    {
        weaponDatabase = null;
        if (EditorPrefs.HasKey(editorPref))
        {
            EditorPrefs.DeleteKey(editorPref);
        }
    }

    void SelectDatabase()
    {
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = weaponDatabase;
    }
}
