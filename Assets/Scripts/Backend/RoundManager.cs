
//////////////////////////////////////////////
//Assignment/Lab/Project: Zombie Survival
//Name: Joe Morris
//Section: SGD.213.0021
//Instructor: Professor Locklear
//Date: 05/02/2024
/////////////////////////////////////////////

using UnityEngine;

public class RoundManager : MonoBehaviour
{
    //controls the rounds. Tells the zombie spawn manager the parameters for spawning them, and how much health they should have
    public static RoundManager instance;
    [SerializeField] int currentRound = 0;

    public int CurrentRound
    {
        get { return currentRound; }
    }

    [Header("Zombie Health Parameters")]
    [SerializeField] float currentHealth = 150; //the current health of the zombies. Used as the starting value
    [SerializeField] int healthAdditionPerRound = 100; //how much health to add to zombies each round.
    [SerializeField] float healthMultiplierPerRound = 1.1f; //how much to multiply the health by each round.
    [SerializeField] int roundToStartMultiplyingHealth = 10; //if the current round is less than this number, health will be added to. other wise the zombies health will be multiplied by the factor above.
    
    [Header("Spawn Rate Parameters")]
    [SerializeField] float currentSpawnRate = 2; //the time in seconds between each zombie spawn/instantiation
    [SerializeField] float spawnRateMultiplierPerRound = 0.95f; //the spawn rate will be multiplied by this each round.
    [SerializeField] float spawnRateCap = 0.08f; //the minimum time in seconds the spawn rate can be.

    [Header("Amount of Zombies To Spawn")]
    [SerializeField] int[] zombiesToSpawnPerRound; //an index of how many zombies will spawn each round formatted by zombiesToSpawnPerRound[currentRound] = the amount of zombies that will spawn in the current round.
    [SerializeField] int maxNumberOfZombiesToSpawnPerRound = 24; //the maximum amount of zombies that will spawn per round
    [SerializeField] int maxNumberOfZombiesAliveAtOnce = 24; //the maximum amount of zombies that can be alive per round. if the current number of zombies to spawn is greater than this number, then the zombie spawner will wait until a zombie dies to spawn another.

    [Header("Sound Effects")]
    [SerializeField] AudioClip roundStartSound; 
    [SerializeField] AudioClip roundEndSound;
    
    int zombiesThisRound = 0;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Invoke("BeginNextRound", 1f);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M)) //cheat code to skip rounds. the current round is ended, a new round starts, and the player is given 1000 points. Not intended to be used for normal play purposes, only in testing/development purposes
        {
            GetComponent<AudioSource>().Stop();
            ZombieSpawnManager.instance.SkipRound();
            GameManager.instance.PlayerScore(1000);
        }
    }

    void BeginNextRound()
    {
        CancelInvoke();
        
        GetComponent<AudioSource>().PlayOneShot(roundStartSound);

        currentRound++;

        Invoke("NewRoundTextUpdate", 3); //wait to updates the round after 3 seconds, which is when the "beat drops" in the round start sound effect

        //if the current round is past the max number of entries in the zombiesToSpawnPerRound array, then the max number of zombies to spawn each round will be used for the zombie count.
        zombiesThisRound = currentRound >= zombiesToSpawnPerRound.Length ? maxNumberOfZombiesToSpawnPerRound : zombiesToSpawnPerRound[currentRound];
        

        if(currentRound > 1)
        {
            if(currentRound < roundToStartMultiplyingHealth)
            {
                currentHealth += healthAdditionPerRound;
            }
            else
            {
                currentHealth *= healthMultiplierPerRound;
            }

            if(currentSpawnRate > spawnRateCap)
            {
                currentSpawnRate *= spawnRateMultiplierPerRound;
                
                if(currentSpawnRate <= spawnRateCap)
                {
                    currentSpawnRate = spawnRateCap;
                }
            }
        }

        StartCoroutine(ZombieSpawnManager.instance.SpawnZombies(currentHealth, zombiesThisRound, maxNumberOfZombiesAliveAtOnce, currentSpawnRate, currentRound > 5));
        Debug.Log("Round " + currentRound + ": " + zombiesThisRound + " Zombies will spawn with " + currentHealth + " health with a spawn rate of " + currentSpawnRate + " seconds");
    }

    void NewRoundTextUpdate()
    {
        UiController.instance.UpdateRoundText(currentRound, Color.red);
        UiController.instance.UpdateZombiesLeftText(zombiesThisRound);
    }

    public void RoundOver(bool endImmediately = false)
    {
        if(endImmediately)
        {
            BeginNextRound();
            return;
        }

        UiController.instance.UpdateRoundText(currentRound, Color.white);

        GetComponent<AudioSource>().PlayOneShot(roundEndSound);
        float timeTilSFXOver = roundEndSound.length;

        Debug.Log("Round " + currentRound + " over. Round " + (currentRound + 1) + " will begin in " + timeTilSFXOver + 1 + " seconds");
        Invoke("BeginNextRound", timeTilSFXOver + 1);
    }
}
