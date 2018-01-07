using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "EnemyName.asset", menuName = "Puzzle RPG/Enemy")]
public class Enemy : Encounter{

    public int ID = 0;
    public int Level = 1;
    public int BaseHealth = 100;
    public int AttackDamageMin = 1;
    public int AttackDamageMax = 10;
    public int AttackSpeed = 3;
    public Sprite EnemySprite;
    public Sprite HurtSprite;


}
