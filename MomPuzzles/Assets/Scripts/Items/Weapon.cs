using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Weapon.asset", menuName = "Puzzle RPG/Weapon")]
public class Weapon : Equippable {
    public int BlockClearModifier = 0;
    public int MinDamage = 0;
    public int MaxDamage = 0;
}
