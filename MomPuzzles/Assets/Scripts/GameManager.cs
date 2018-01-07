using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    [HideInInspector]
    private Player Player;
    public Location CurrentLocation;
    public Encounter CurrentEncounter;
    public int enemyCurrentHealth;
    //public EncounterController EncounterController;

    //UIBoardManager boardManager;
    int encounterTableTotalProbability;

    //Location stats
    int locationKills = 0;
    int locationKillsPerFloor = 5;
    int locationFloor = 1;

    //Battle System
    float turnCount = 0;
    float partialTurnAmount = 0.33f;

    //Events
    //
    //Enemy Killed
    public delegate void EnemyKilled(Enemy enemy);
    public static event EnemyKilled OnEnemyKilled;
    //
    //Location Level Increased
    public delegate void LocationLevelUp(int level, Location location);
    public static event LocationLevelUp OnLocationLevelUp;
    //
    //Enemy Take Damage
    public delegate void EnemyTakeDamage(EnemyTakeDamageArgs args);
    public static event EnemyTakeDamage OnEnemyTakeDamage;
    //
    //Player Take Damage
    public delegate void PlayerTakeDamage(PlayerTakeDamageArgs args);
    public static event PlayerTakeDamage OnPlayerTakeDamage;
    //
    //Player Heal Damage
    public delegate void PlayerHealDamage(PlayerHealDamageArgs args);
    public static event PlayerHealDamage OnPlayerHealDamage;

    public int LocationFloor
    {
        get { return locationFloor; }
    }

    void OnEnable()
    {
        UIBoardManager.OnBlockCleared += BlockClear;
    }

    void OnDisable()
    {
        UIBoardManager.OnBlockCleared -= BlockClear;
    }

    // Use this for initialization
    void Start () {
        Player = PlayerController.Controller.Player;

        locationKills = 0; //Reset this each time we go adventure
        locationFloor = 1;

        //boardManager = FindObjectOfType<UIBoardManager>();

        //Player.EquippedWeapon = new Weapon(0, 0, 10, 0);
        //boardManager.AmountNeededToClear += Player.EquippedWeapon.BlockClearModifier;
        if (PlayerController.Controller.CurrentLocation != null)
        {
            SetCurrentLocation(PlayerController.Controller.CurrentLocation); //TODO: Super dumb debug bullshit fix this.
        }
        else
        {
            SetCurrentLocation(CurrentLocation); // By default this should be set to a location.
        }
        
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyUp(KeyCode.K))
        {
            Enemy enemy = (CurrentEncounter as Enemy);

            KillEnemy(enemy);
        }
	}

    void BlockClear(BlocksClearedEventArgs args)
    {
        Enemy enemy = (CurrentEncounter as Enemy);

        if (enemy)
        {
            if (args.AttackCount > 0)
            {
                if (DamageEnemy(Player.Attack(args)))
                {
                    KillEnemy(enemy);
                    return;
                }

            }
            if (args.HealCount > 0)
            {
                int healAmt = Player.HealPower * args.HealCount;
                if (OnPlayerHealDamage != null)
                {
                    PlayerHealDamageArgs healArgs;
                    healArgs.amount = healAmt;
                    OnPlayerHealDamage(healArgs);
                }
                Player.Heal(healAmt);
            }


            turnCount++;



            if (turnCount >= enemy.AttackSpeed)
            {
                //Enemy's turn to attack.
                int enemyDamage = Random.Range(enemy.AttackDamageMin, enemy.AttackDamageMax + 1);
                if (OnPlayerTakeDamage != null)
                {
                    PlayerTakeDamageArgs dmgArgs;
                    dmgArgs.amount = enemyDamage;
                    dmgArgs.attacker = CurrentEncounter as Enemy;
                    OnPlayerTakeDamage(dmgArgs);
                }
                Player.TakeDamage(enemyDamage);
                turnCount = 0;
            }
        }
        
    }

    public void SetCurrentLocation(Location Location)
    {
        CurrentLocation = Location;

        NextEncounter();
    }

    void NextEncounter()
    {
        turnCount = 0;
        Debug.Log("------GET ENCOUNTER------");
        float totalPob = GetTotalEncounterProbability(LocationFloor);
        float rndNum = Random.Range(1, totalPob);
        Debug.Log(rndNum + " out of " + totalPob);

        foreach (EncounterData ed in CurrentLocation.EncounterData)
        {
            float edProb = GetEnouncterProbability(ed, LocationFloor);
            rndNum -= edProb;
            Debug.Log(" rndNum at: "  + rndNum);
            if (rndNum <= 0)
            {
                SetEncounter(ed.Encounter);
                Debug.Log("------GOT ENCOUNTER------");
                break;
            }
        }
    }

    void SetEncounter(Encounter encounter)
    {
        Debug.Log("New Encounter: " + encounter.Name);
        CurrentEncounter = encounter;
        if (CurrentEncounter as Enemy)
        {
            enemyCurrentHealth = (CurrentEncounter as Enemy).BaseHealth;
        }
    }

    float GetTotalEncounterProbability(int currentFloor)
    {
        float result = 0.0f;

        foreach(EncounterData ed in CurrentLocation.EncounterData)
        {
            result += GetEnouncterProbability(ed, currentFloor);
            Debug.Log("Total: " + result);
        }

        return result;
    }

    float GetEnouncterProbability(EncounterData encounterData, float currentFloor)
    {
        //TODO: fix up this equation to change pre main floor probabilities
        float result = 0.0f;

        float minFraction = Mathf.Clamp01(currentFloor / encounterData.MinFloor);
        //float maxFraction = Mathf.Clamp01((CurrentLocation.MaxFloors - currentFloor) / ((CurrentLocation.MaxFloors + 1) - encounterData.MaxFloor));
        float maxFraction = (currentFloor <= encounterData.MaxFloor) ? 1 : 1 - ((currentFloor - encounterData.maxFloor) / (CurrentLocation.MaxFloors - encounterData.MaxFloor));
        float fraction = minFraction * maxFraction;

        fraction = fraction * fraction; //make Quadratic
        result = fraction * encounterData.Probability;
        Debug.Log(encounterData.Encounter.Name + " Probability: " + result + "    MnF: " + minFraction + "  MxF:" + maxFraction);

        return result;
    }

    public bool DamageEnemy(int amount)
    {
        bool death = false;
        enemyCurrentHealth -= amount;
        Debug.Log(CurrentEncounter.Name + " takes " + amount + " damage.");

        if (enemyCurrentHealth <= 0)
        {
            Debug.Log(CurrentEncounter.Name + " dies.");
            death =  true;
        }
        else
        {
            death = false;
        }

        if (OnEnemyTakeDamage != null)
        {
            //string reported = (death) ? (amount + " KILLED!") : amount.ToString();
            EnemyTakeDamageArgs args;
            args.amount = amount;
            args.enemy = CurrentEncounter as Enemy;
            args.ResultedInDeath = death;
            OnEnemyTakeDamage(args);
        }

        return death;
    }

    void KillEnemy(Enemy enemy)
    {
        if (OnEnemyKilled != null)
        {
            OnEnemyKilled(enemy);
        }

        locationKills++;
        //Debug.Log(locationKills + " >= " + (locationKillsPerLevel + (locationLevel * locationKillsPerLevel)));
        if (locationKills >= (locationKillsPerFloor + ((locationFloor - 1) * locationKillsPerFloor)))
        {
            LocationLeveLUp();
        }

        NextEncounter();
    }

    void LocationLeveLUp()
    {
        locationFloor++;
        if (locationFloor > CurrentLocation.MaxFloors)
        {
            locationFloor--;
            return;
        }
        Debug.Log("Reached Location Level " + locationFloor);

        if (OnLocationLevelUp != null)
        {
            OnLocationLevelUp(locationFloor, CurrentLocation);
        }
    }
}

public struct EnemyTakeDamageArgs
{
    public int amount;
    public bool ResultedInDeath;
    public Enemy enemy;
}

public struct PlayerTakeDamageArgs
{
    public int amount;
    public Enemy attacker;
}

public struct PlayerHealDamageArgs
{
    public int amount;
}
