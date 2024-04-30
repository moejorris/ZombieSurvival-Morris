

using UnityEngine;

public class ZombieSpawnPointCreator : MonoBehaviour
{
    //At runtime, this script uses a top down camera and mouse clicks to create empty game objects which represents spawn points for the zombie. This was faster than creating an empty and then manually moving it to the desired position.
    [SerializeField] Transform zombieSpawnPointsParent;
    [SerializeField] GameObject empty;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Debug.Log("Try for spawn point creation");
            RaycastHit hit;
            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 99999f))
            {
                Instantiate(empty, hit.point, Quaternion.identity, zombieSpawnPointsParent).name = "Spawn Point(" + zombieSpawnPointsParent.childCount + ")";
                Debug.Log("Created spawn point.");
            }
        }
    }
}
