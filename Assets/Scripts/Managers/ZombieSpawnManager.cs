using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;

public class ZombieSpawnManager : MonoBehaviour
{
    public static ZombieSpawnManager instance;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] GameObject zombiePrefab;
    // [SerializeField] List<GameObject> zombiesAlive = new List<GameObject>();

    int zombiesSpawned = 0;
    int zombiesAlive = 0;
    int currentZombiesToSpawn = 0;
    void Awake()
    {
        instance = this;
    }

    public IEnumerator SpawnZombies(float health, int targetSpawnCount, int maxZombiesAlive, float spawnRate, int amountOfRunningZombies)
    {
        currentZombiesToSpawn = targetSpawnCount;
        zombiesSpawned = 0;
        // int runnersSpawned = 0;

        while(zombiesSpawned < currentZombiesToSpawn)
        {
            GameObject newZombie;

            if(zombiesAlive < maxZombiesAlive)
            {
                Vector3 spawnPoint = GetOneOfThreeClosestSpawnPoints();
                newZombie = Instantiate(zombiePrefab, spawnPoint, Quaternion.identity);
                
                newZombie.GetComponent<ZombieHealth>().health = health;
                zombiesSpawned++;
                zombiesAlive++;
                // if(runnersSpawned < amountOfRunningZombies)
                // {
                //     newZombie.GetComponent<ZombieMovement>().running = true;
                // }
            }


            yield return new WaitForSeconds(spawnRate);
        }
    }

    Vector3 GetOneOfThreeClosestSpawnPoints() //sorts through the spawnpoints by CURRENT distance to the player, and returns one of the 3 closest (at random) positions
    {
        //get all spawn points
        List<Transform> closestSpawnPoints = spawnPoints.ToList();
        //get player position
        Vector3 playerPosition = PlayerManager.instance.transform.position;
        //sort through list by closest to the player
        closestSpawnPoints.Sort(delegate (Transform t1, Transform t2) { return Vector3.Distance(playerPosition, t1.position).CompareTo(Vector3.Distance(playerPosition, t2.position)); });
        //return the position from index 0, 1, or 2 randomly. This way zombies are not only spawning at the closest spawn point.
        return closestSpawnPoints[Random.Range(0, 3)].position;
    }

    public void CheckForRoundOver()
    {
        zombiesAlive--;
        if(zombiesAlive < 1 && zombiesSpawned >= currentZombiesToSpawn) //check if all zombies have spawned and all zombies are dead
        {
            RoundManager.instance.RoundOver();
        }
    }
}
