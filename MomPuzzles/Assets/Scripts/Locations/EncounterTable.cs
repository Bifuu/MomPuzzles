using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EncounterTable.asset", menuName = "Puzzle RPG/Encounter Table")]
public class EncounterTable : ScriptableObject {
    public List<EncounterTableRow> Table; //EncounterTableRow defined below.
}

[System.Serializable]
public class EncounterTableRow
{
    public Encounter Encounter;
    public int Probability = 1;
}