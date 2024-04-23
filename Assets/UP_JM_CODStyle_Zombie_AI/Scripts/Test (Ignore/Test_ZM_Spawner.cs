
using UnityEngine;

public class Test_ZM_Spawner : MonoBehaviour
{
    //Not for use in actual zombies. Used simply to randomly spawn zombies in the map, outside of playable area.
    [SerializeField] int spawnCount = 10;
    [SerializeField] BoxCollider noSpawnZone;
    [SerializeField] float minSpawnPos = -60f, maxSpawnPos = 60f;
    [SerializeField] GameObject zombie;


    // Start is called before the first frame update
    void Start()
    {
        SpawnZombies();
    }

    void SpawnZombies()
    {
        int spawned = 0;
        while(spawned < spawnCount)
        {
            Vector3 spawnPoint = new Vector3(Random.Range(minSpawnPos, maxSpawnPos), 0, Random.Range(minSpawnPos, maxSpawnPos));
            Vector3 spawnerPosition = spawnPoint;
            spawnerPosition.y = 50f; 
            if(Physics.Raycast(spawnerPosition, Vector3.down, out RaycastHit hit, 100f))
            {
                if(hit.collider == noSpawnZone)
                {
                    
                }
                else
                {
                    GameObject newZombie = Instantiate(zombie, spawnPoint, Quaternion.identity);
                    newZombie.name = "Zombie " + spawned;
                    spawned++;
                }
            }
            else
            {
                GameObject newZombie = Instantiate(zombie, spawnPoint, Quaternion.identity);
                newZombie.name = "Zombie " + spawned;
                spawned++;
            }
        }
    }
}
