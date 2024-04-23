using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ZombieEntrance : MonoBehaviour
{
    [SerializeField] GameObject[] planks;
    [SerializeField] NavMeshObstacle navMeshObstacle;

    [SerializeField] bool alreadyRunning;

    // Start is called before the first frame update
    void Start()
    {
        alreadyRunning = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RemovePlank()
    {
        if(GetActivePlanks() <= 0)
        {
            return;
        }
        
        for(int i = 0; i < planks.Length; i++)
        {
            if(planks[i].activeInHierarchy)
            {
                planks[i].SetActive(false);
                NavMeshObstacleCheck();
                Debug.Log("Removed Plank");
                return;
            }
        }
    }

    public void AddPlank()
    {
        if(GetActivePlanks() >= planks.Length)
        {
            return;
        }

        for(int i = 0; i < planks.Length; i++)
        {
            // Debug.Log("Build iteration : " + i);
            if(!planks[i].activeInHierarchy)
            {
                planks[i].SetActive(true);
                NavMeshObstacleCheck();
                Debug.Log("Added Plank");
                return;
            }
        }
    }

    void NavMeshObstacleCheck()
    {
        if(GetActivePlanks() > 0)
        {
            navMeshObstacle.enabled = true;
        }
        else
        {
            navMeshObstacle.enabled = false;
        }
    }

    int GetActivePlanks()
    {
        int enabledPlanks = 0;

        for(int i = 0; i < planks.Length; i++)
        {
            if(planks[i].activeInHierarchy)
            {
                enabledPlanks++;
            }
        }

        return enabledPlanks;
    }

    void OnTriggerEnter(Collider other)
    {
        BeginRemovingBarrier(other.GetComponent<ZombieAttack>());

        if(other.CompareTag("Player"))
        {
            Debug.Log("Player Bulding");
            InvokeRepeating("AddPlank", 1, 1);
        }
    }

    void OnTriggerExit(Collider other)
    {
        ZombieAttack zombie = other.gameObject.GetComponent<ZombieAttack>();

        if(zombie)
        {
            CancelInvoke("RemovePlank");
            alreadyRunning = false;
        }

        if(other.CompareTag("Player")) //Rework to include player must interact
        {
            Debug.Log("Player Left");
            CancelInvoke("AddPlank");
        }
 
    }

    void OnTriggerStay(Collider other)
    {
        BeginRemovingBarrier(other.GetComponent<ZombieAttack>());
    }

    void BeginRemovingBarrier(ZombieAttack zombieAttack)
    {
        if(zombieAttack == null) return;

        ZombieAttack zombie = zombieAttack;

        if(!alreadyRunning && zombie.canRemoveBarrier)
        {
            InvokeRepeating("RemovePlank", zombie.barrierRemovalRate, zombie.barrierRemovalRate);
            alreadyRunning = true;
        }
    }


    // if(Input.GetKeyDown(interactKey))
    // {
    //     currentInteractable.Interact();
    // }

    // if(!Input.GetKey(interactKey) && currentInteractable.holdInteract)
    // {
    //     currentInteractable.StopInteracting();
    // }
    // void StartBuilding()
    // {
    //     InvokeRepeating("AddPlank", 1, 1);
    // }

    // void StopBuilding()
    // {
    //     CancelInvoke("AddPlank");
    // }
}
