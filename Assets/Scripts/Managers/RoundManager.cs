
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static RoundManager instance;
    [SerializeField] int currentRound = 0;

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
        if(Input.GetKeyDown(KeyCode.M))
        {
            GetComponent<AudioSource>().Stop();
            ZombieSpawnManager.instance.SkipRound();
            GameManager.instance.PlayerScore(1000);
        }
    }

    void BeginNextRound()
    {
        GetComponent<AudioSource>().PlayOneShot(roundStartSound);

        currentRound++;

        UiController.instance.UpdateRoundText(currentRound);

        //if the current round is past the max number of entries in the zombiesToSpawnPerRound array, then the max number of zombies to spawn each round will be used for the zombie count.
        int zombiesThisRound = currentRound >= zombiesToSpawnPerRound.Length ? maxNumberOfZombiesToSpawnPerRound : zombiesToSpawnPerRound[currentRound];
        
        UiController.instance.UpdateZombiesLeftText(zombiesThisRound);

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

        StartCoroutine(ZombieSpawnManager.instance.SpawnZombies(currentHealth, zombiesThisRound, maxNumberOfZombiesAliveAtOnce, currentSpawnRate, 0));
        Debug.Log("Round " + currentRound + ": " + zombiesThisRound + " Zombies will spawn with " + currentHealth + " health with a spawn rate of " + currentSpawnRate + " seconds");
    }

    public void RoundOver(bool endImmediately = false)
    {
        if(endImmediately)
        {
            BeginNextRound();
            return;
        }

        GetComponent<AudioSource>().PlayOneShot(roundEndSound);
        float timeTilSFXOver = roundEndSound.length;

        Debug.Log("Round " + currentRound + " over. Round " + (currentRound + 1) + " will begin in " + timeTilSFXOver + 1 + " seconds");
        Invoke("BeginNextRound", timeTilSFXOver + 1);
    }
}
