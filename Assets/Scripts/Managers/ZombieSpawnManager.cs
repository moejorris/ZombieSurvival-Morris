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

    int zombiesSpawned = 0;
    int zombiesAlive = 0;
    int currentZombiesToSpawn = 0;
    int zombiesKilled = 0;
    void Awake()
    {
        instance = this;
    }

    //FIX MORE THAN TOLD ZOMBIES SPAWNING PER ROUND
    //CHECK IF ZOMBIES ARE SPAWNING IN TUNNEL BEFORE ROOM IS ACCESSIBLE
    public IEnumerator SpawnZombies(float health, int targetSpawnCount, int maxZombiesAlive, float spawnRate, int amountOfRunningZombies)
    {
        currentZombiesToSpawn = targetSpawnCount;
        zombiesAlive = 0; //should fix too many zombies spawning
        zombiesSpawned = 0;
        zombiesKilled = 0;
        // int runnersSpawned = 0;

        while(zombiesSpawned < currentZombiesToSpawn)
        {
            GameObject newZombie;

            if(zombiesAlive < maxZombiesAlive)
            {
                Vector3 spawnPoint = GetOneOfThreeClosestSpawnPoints();
                newZombie = Instantiate(zombiePrefab, spawnPoint, Quaternion.identity);
                newZombie.name = "Zombie " + zombiesSpawned; 
                
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

        for(int i = closestSpawnPoints.Count - 1; i >= 0; i--) //starts at the top of the list and removes all spawn points that are inactive (spawn points that are inactive due to zombies being unable to reach the player until a room is opened)
        {
            if(!closestSpawnPoints[i].gameObject.activeInHierarchy)
            {
                Debug.Log(closestSpawnPoints[i].name);
                closestSpawnPoints.Remove(closestSpawnPoints[i]);
            }
        }

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
        zombiesKilled++;
        
        UiController.instance.UpdateZombiesLeftText(currentZombiesToSpawn - zombiesKilled);
        
        if(zombiesKilled >= currentZombiesToSpawn) //check if all zombies have spawned and all zombies are dead
        {
            RoundManager.instance.RoundOver();
        }
    }
}
