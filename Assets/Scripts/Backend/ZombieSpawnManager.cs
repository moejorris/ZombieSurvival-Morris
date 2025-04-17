//////////////////////////////////////////////
//Assignment/Lab/Project: Zombie Survival
//Name: Joe Morris
//Section: SGD.213.0021
//Instructor: Professor Locklear
//Date: 05/02/2024
/////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZombieSpawnManager : MonoBehaviour
{
    //Used by RoundManager, spawns zombies and controls their health, how many should spawn, how many zombies can be alive at once, and if a zombie should be a running zombie or not.
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


    public IEnumerator SpawnZombies(float health, int targetSpawnCount, int maxZombiesAlive, float spawnRate, bool runningZombies)
    {
        currentZombiesToSpawn = targetSpawnCount;
        zombiesAlive = 0;
        zombiesSpawned = 0;
        zombiesKilled = 0;

        while(zombiesSpawned < currentZombiesToSpawn)
        {
            if(zombiesAlive < maxZombiesAlive)
            {
                Vector3 spawnPoint = GetOneOfFiveClosestSpawnPoints();
                GameObject newZombie = Instantiate(zombiePrefab, spawnPoint, Quaternion.identity);
                newZombie.name = "Zombie " + zombiesSpawned; 
                
                if(Random.Range(0, 1f) > 0.5f && runningZombies) //50% chance the zombie will be a running zombie after round 5
                {
                    newZombie.GetComponent<ZombieMovement>().running = true;
                    newZombie.name = "Running Zombie " + zombiesSpawned;
                }

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

    Vector3 GetOneOfFiveClosestSpawnPoints() //sorts through the spawnpoints by CURRENT distance to the player, and returns one of the 5 closest (at random) positions to avoid too many zombies spawning from the same place
    {
        //get all spawn points
        List<Transform> closestSpawnPoints = spawnPoints.ToList();

        for(int i = closestSpawnPoints.Count - 1; i >= 0; i--) //starts at the top of the list and removes all spawn points that are inactive (spawn points that are inactive due to zombies being unable to reach the player until a room is opened)
        {
            if(!closestSpawnPoints[i].gameObject.activeInHierarchy)
            {
                // Debug.Log(closestSpawnPoints[i].name);
                closestSpawnPoints.Remove(closestSpawnPoints[i]);
            }
        }

        //get player position
        Vector3 playerPosition = PlayerManager.instance.transform.position;
        //sort through list by closest to the player
        closestSpawnPoints.Sort(delegate (Transform t1, Transform t2) { return Vector3.Distance(playerPosition, t1.position).CompareTo(Vector3.Distance(playerPosition, t2.position)); });
        //return the position from index 0, 1, or 2 randomly. This way zombies are not only spawning at the closest spawn point.
        return closestSpawnPoints[Random.Range(0, 5)].position;
    }

    public void CheckForRoundOver()
    {
        zombiesAlive--;
        zombiesKilled++;
        
        UiController.instance.UpdateZombiesLeftText(currentZombiesToSpawn - zombiesKilled);
        
        if(zombiesKilled >= currentZombiesToSpawn) //check if all zombies have spawned and all zombies are dead
        {
            KillAllZombies(); //sometimes one or a few extra zombies are spawned. I cannot figure out why. This way when the required amount of zombies are killed, any extras are destroyed.
            RoundManager.instance.RoundOver();
        }
    }

    public void KillAllZombies()
    {
        ZombieHealth[] allZombies = Object.FindObjectsByType<ZombieHealth>(FindObjectsSortMode.None);

        if(allZombies.Length < 1) return;

        for(int i = 0; i < allZombies.Length; i++)
        {
            allZombies[i].Die(true);
        }
    }

    public void StopRound()
    {
        KillAllZombies();
        
        StopCoroutine("SpawnZombies");
        
        UiController.instance.UpdateZombiesLeftText(currentZombiesToSpawn - zombiesKilled);
    }

    public void SkipRound() //used when using the M key cheat code to skip rounds.
    {
        KillAllZombies();
        
        StopCoroutine("SpawnZombies");
        
        UiController.instance.UpdateZombiesLeftText(currentZombiesToSpawn - zombiesKilled);
        RoundManager.instance.RoundOver(true);
    }

    public void PlayerDied()
    {
        StopAllCoroutines();
        KillAllZombies();
    }
}
