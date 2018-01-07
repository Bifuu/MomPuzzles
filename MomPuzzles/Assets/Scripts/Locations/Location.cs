using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LocationName.asset", menuName = "Puzzle RPG/Location")]
public class Location : ScriptableObject {

    public string Name;
    public int ID;
    public int MaxFloors = 20;
    public List<EncounterData> EncounterData;

}

[System.Serializable]
public class EncounterData
{
    public Encounter Encounter;
    public int Probability;
    public float minFloor;
    public float maxFloor;

    public EncounterData(Encounter encounter, int probability, int minFloor, int maxFloor)
    {
        Encounter = encounter;
        Probability = probability;
        this.minFloor = minFloor;
        this.maxFloor = maxFloor;
    }

    public int MinFloor
    {
        get
        {
            return (int)minFloor;
        }
    }

    public int MaxFloor
    {
        get
        {
            return (int)maxFloor;
        }
    }
}