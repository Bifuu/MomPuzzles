using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Player {

    public int level = 1;
    public int Experience = 0;
    public int CurrentHP = 100;
    public int MaxHP = 100;
    public int AttackDamage = 10;
    public int BlockDamage = 0;
    public int HealPower = 10;
    public int MaxQuests = 3;
    public List<CurrentQuest> Quests;
    public List<int> CompletedQuestIDS;
    public List<int> AvailableWeaponIDs;
    public List<int> AvailableArmorIDs;
    public Weapon EquippedWeapon;
    public Armor EquippedArmor;
    public float GlancingBlowMultiplier = 0.25f;
    public float AttackBlockChance = 0.10f;
    public float HealBlockChance = 0.05f;
    public AnimationCurve EXPGrowth;
    
    //public List<InventoryItem> Inventory;
    public int Currency;

    public bool HasWeaponEquipped
    {
        get { return EquippedWeapon != null; }
    }

    public bool HasArmorEquipped
    {
        get { return EquippedArmor != null; }
    }

    public int MinDamage
    {
        get
        {
            int damage = 0;
            if (this.HasWeaponEquipped)
            {
                damage += EquippedWeapon.MinDamage + AttackDamage;
            }
            else
            {
                damage += Mathf.CeilToInt(AttackDamage);
            }

            return damage;
        }
    }

    public int MaxDamage
    {
        get
        {
            int damage = 0;
            if (this.HasWeaponEquipped)
            {
                damage += EquippedWeapon.MaxDamage + AttackDamage;
            }
            else
            {
                damage += Mathf.CeilToInt(AttackDamage);
            }

            return damage;
        }
    }

    public int Defense
    {
        get
        {
            int def = BlockDamage;
            if (this.HasArmorEquipped)
            {
                def += EquippedArmor.DamageBlock;
            }

            return def;
        }
    }

    public int Attack(BlocksClearedEventArgs args)
    {
        int damage = 0;

        damage = Random.Range(MinDamage * args.AmountCleared, MaxDamage * args.AmountCleared + 1);

        return damage;
    }

    public void TakeDamage(int amount)
    {
        Debug.Log("Player takes " + amount + " damage");
        CurrentHP -= amount;
    }

    public void Heal(int amount)
    {
        Debug.Log("Player heals for " + amount);
        CurrentHP = Mathf.Clamp(CurrentHP + amount, 0, MaxHP);
    }

    public int Level
    {
        get
        {
            return Experience / 1000 + 1;
        }
    }

    public bool CanEquipArmor(Armor armor)
    {
        if (armor.LevelRequirement > level)
        {
            return false;
        }
        return true;
    }

    public bool CanEquipWeapon(Weapon weapon)
    {
        if (weapon.LevelRequirement > level)
        {
            return false;
        }
        return true;
    }

    public void EquipWeapon(Weapon weapon)
    {
        EquippedWeapon = weapon;
    }

    public void EquipArmor(Armor armor)
    {
        EquippedArmor = armor;
    }

    public void AcceptQuest(Quest quest)
    {
        if (Quests.Count < MaxQuests)
        {
            Quests.Add(new CurrentQuest(quest));
            Debug.Log("Accepted Quest: " + quest.QuestName);
        }        
    }
}
