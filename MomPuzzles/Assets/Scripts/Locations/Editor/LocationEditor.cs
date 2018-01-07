using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LocationEditor : EditorWindow
{

    public LocationDatabase locationDatabase;
    string editorPref = "LocationDatabase";
    string idPref = "NextLocationID";
    string locationPath = "Assets/Databases/Locations";

    Vector2 scrollPos;
    Location tempLocation;
    Location editingLocation;
    int selectedIndex = -1;
    Encounter newEncounter;
    int newProbability;
    int newMainFloor;


    [MenuItem("Window/Puzzle RPG/Location Editor")]
    static void Init()
    {
        LocationEditor window = (LocationEditor)EditorWindow.GetWindow(typeof(LocationEditor));
        window.Show();
    }

    void OnEnable()
    {

        if (!AssetDatabase.IsValidFolder(locationPath))
        {
            AssetDatabase.CreateFolder("Assets/Databases", "Locations");
        }

        if (EditorPrefs.HasKey(editorPref))
        {
            string path = EditorPrefs.GetString(editorPref);
            locationDatabase = AssetDatabase.LoadAssetAtPath(path, typeof(LocationDatabase)) as LocationDatabase;
        }


        if (!EditorPrefs.HasKey(idPref))
        {
            EditorPrefs.SetInt(idPref, 1);
        }

    }

    void OnDisable()
    {
        ClearTempLocation();
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Location Editor: ", EditorStyles.boldLabel);
        if (locationDatabase)
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
                locationDatabase = NewDatabase();
            }
            if (GUILayout.Button("Load"))
            {
                OpenDatabase();
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        //Edit fields for quest stuff.
        if (locationDatabase)
        {
            EditorGUILayout.BeginHorizontal();
            //List of Weapons
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(100));
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, EditorStyles.textArea, GUILayout.Width(100));

            for (int i = 0; i < locationDatabase.DB.Count; i++)
            {
                Location loc = locationDatabase.DB[i];
                if (GUILayout.Button(loc.name))
                {
                    //tempQuest.Clone(q);
                    selectedIndex = i;
                    editingLocation = loc;
                }
            }



            EditorGUILayout.EndScrollView();
            if (GUILayout.Button("New"))
            {
                //Create a new TempQuest.
                selectedIndex = -1;
                CreateTempLocation();
                editingLocation = tempLocation;
            }
            EditorGUILayout.EndVertical();

            if (editingLocation)
            {
                //MAin Editor area
                EditorGUILayout.BeginVertical();
                EditorGUI.BeginChangeCheck();

                
                editingLocation.Name = EditorGUILayout.TextField("Location Name:", editingLocation.Name);
                EditorGUILayout.SelectableLabel(editingLocation.ID.ToString());
                editingLocation.MaxFloors = EditorGUILayout.IntField("Max Floor:", editingLocation.MaxFloors);

                int indexToDelete = -1;
                for (int i = 0; i < editingLocation.EncounterData.Count; i++)
                {
                    EncounterData ed = editingLocation.EncounterData[i];
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    ed.Encounter = EditorGUILayout.ObjectField("Encounter:", ed.Encounter, typeof(Encounter), false) as Encounter;
                    //ed.MinFloor = EditorGUILayout.IntField("Main Floor:", ed.MainFloor);
                   

                    //ed.MinFloor = Mathf.Floor(ed.MinFloor);
                    //ed.MaxFloor = Mathf.Floor(ed.MaxFloor);
                    EditorGUILayout.MinMaxSlider(ref ed.minFloor, ref ed.maxFloor, 1, editingLocation.MaxFloors);
                    EditorGUILayout.SelectableLabel("MinFloor: " + ed.MinFloor + ", MaxFloor: " + ed.MaxFloor); //TODO: Crash using the property?

                    ed.Probability = EditorGUILayout.IntField("Probability:", ed.Probability);

                    if (GUILayout.Button("Delete")) indexToDelete = i;
                    EditorGUILayout.EndVertical();
                }
                if (indexToDelete > -1)
                {
                    editingLocation.EncounterData.RemoveAt(indexToDelete);
                }

                if (GUILayout.Button("Add empty encounter data"))
                {
                    editingLocation.EncounterData.Add(new EncounterData(null, 0, 0, editingLocation.MaxFloors));
                }

                /*
                editingLocation.LevelRequirement = EditorGUILayout.IntField("Required Level:", editingLocation.LevelRequirement);
                editingLocation.MinDamage = EditorGUILayout.IntField("Min Damage:", editingLocation.MinDamage);
                editingLocation.MaxDamage = EditorGUILayout.IntField("Max Damage:", editingLocation.MaxDamage);
                editingLocation.BlockClearModifier = EditorGUILayout.IntField("Clear Block Modifier:", editingLocation.BlockClearModifier);
                editingLocation.Attunement = (BlockColor)EditorGUILayout.EnumPopup("Atunement:", editingLocation.Attunement);
                editingLocation.Rarity = (ItemRarity)EditorGUILayout.ObjectField("Rarity", editingLocation.Rarity, typeof(ItemRarity), false);
                editingLocation.IconSprite = EditorGUILayout.ObjectField("Enemy Sprite:", editingLocation.IconSprite, typeof(Sprite), true) as Sprite;
                */

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(editingLocation);
                }
                EditorGUILayout.EndVertical();

                //if (GUI.changed) EditorUtility.SetDirty(editingQuest);
            }
            EditorGUILayout.EndHorizontal();


        }

        //StatusBar
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.Height(25));
        GUILayout.Label("Ready");
        if (tempLocation)
        {
            if (GUILayout.Button("Save"))
            {
                SaveNewAsset();
            }
            if (GUILayout.Button("Cancel"))
            {
                ClearTempLocation();
            }
        }

        if (selectedIndex > -1)
        {
            if (GUILayout.Button("Delete"))
            {
                if (EditorUtility.DisplayDialog("Delete Enemy?", "Are you sure you want to delete this?", "Delete"))
                {
                    string itemToBeDestroyed = AssetDatabase.GetAssetPath(locationDatabase.DB[selectedIndex]);
                    locationDatabase.DB.RemoveAt(selectedIndex);
                    EditorUtility.SetDirty(locationDatabase);
                    AssetDatabase.DeleteAsset(itemToBeDestroyed);
                    editingLocation = null;
                    selectedIndex = -1;
                }
            }
        }

        EditorGUILayout.EndHorizontal();

    }





    void SaveNewAsset()
    {
        string assetName = tempLocation.Name.Replace(" ", string.Empty);
        string newAssetPath = locationPath + "/" + assetName + ".asset";
        tempLocation.ID = GetNextID();
        AssetDatabase.CopyAsset(locationPath + "/_temp.asset", newAssetPath);
        AssetDatabase.SaveAssets();
        Location newLocation = AssetDatabase.LoadAssetAtPath(newAssetPath, typeof(Location)) as Location;
        locationDatabase.DB.Add(newLocation);
        EditorUtility.SetDirty(locationDatabase);
        selectedIndex = locationDatabase.DB.Count - 1;
        ClearTempLocation();
    }

    void CreateTempLocation()
    {
        //Make new temp quest holder
        tempLocation = ScriptableObject.CreateInstance<Location>();
        //string path = "Assets/Databases/FlagDatabase.asset";
        AssetDatabase.CreateAsset(tempLocation, locationPath + "/_temp.asset");
        AssetDatabase.SaveAssets();
    }

    void ClearTempLocation()
    {
        //Delete the Temp
        string itemToBeDestroyed = AssetDatabase.GetAssetPath(tempLocation);
        AssetDatabase.DeleteAsset(itemToBeDestroyed);

        tempLocation = null;
    }

    int GetNextID()
    {
        int i = EditorPrefs.GetInt(idPref);
        EditorPrefs.SetInt(idPref, i + 1);
        return i;
    }

    LocationDatabase NewDatabase()
    {
        Debug.Log("New Location Database");
        LocationDatabase asset = ScriptableObject.CreateInstance<LocationDatabase>();

        string path = "Assets/Databases/" + editorPref + ".asset";
        AssetDatabase.CreateAsset(asset, path);
        asset.DB = new List<Location>();
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
            locationDatabase = AssetDatabase.LoadAssetAtPath(relPath, typeof(LocationDatabase)) as LocationDatabase;
            if (locationDatabase.DB == null)
            {
                locationDatabase.DB = new List<Location>();
                EditorUtility.SetDirty(locationDatabase);
            }
            if (locationDatabase)
            {
                Debug.Log(relPath);
                EditorPrefs.SetString(editorPref, relPath);
            }
        }
    }

    void CloseDatabase()
    {
        locationDatabase = null;
        if (EditorPrefs.HasKey(editorPref))
        {
            EditorPrefs.DeleteKey(editorPref);
        }
    }

    void SelectDatabase()
    {
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = locationDatabase;
    }
}
