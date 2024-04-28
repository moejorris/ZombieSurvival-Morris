using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ZombieEntrance : InteractableObject
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
            CancelInvoke("AddPlank");
            return;
        }

        for(int i = 0; i < planks.Length; i++)
        {
            // Debug.Log("Build iteration : " + i);
            if(!planks[i].activeInHierarchy)
            {
                planks[i].SetActive(true);
                PlayerManager.instance.GetComponent<PlayerInteractionHandler>().PlayBuySoundEffect(); //messy way to call this function, but this is likely going to be the only time an external script calls a method of this class.
                NavMeshObstacleCheck();
                // Debug.Log("Added Plank");
                GameManager.instance.PlayerScore(10);
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

        if(enabledPlanks >= planks.Length)
        {
            currentlyInteractable = false;
        }
        else
        {
            currentlyInteractable = true;
        }

        return enabledPlanks;
    }

    public override void Interact()
    {
        InvokeRepeating("AddPlank", 1, 1);
        // Debug.Log("Player Bulding");

    }

    public override void CancelInteract()
    {
        CancelInvoke("AddPlank");
        Debug.Log("Player stopped building");
    }

    void OnTriggerExit(Collider other)
    {
        ZombieAttack zombie = other.gameObject.GetComponent<ZombieAttack>();

        if(zombie)
        {
            StopRemovingBarrier();
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

    public bool BeginRemovingBarrier(ZombieAttack zombieAttack)
    {
        ZombieAttack zombie = zombieAttack;

        if(!alreadyRunning && zombie.canRemoveBarrier)
        {
            InvokeRepeating("RemovePlank", zombie.barrierRemovalRate, zombie.barrierRemovalRate);
            alreadyRunning = true;
            return true;
        }

        return false;
    }

    public void StopRemovingBarrier() //also called by ZombieAttack on death
    {
        CancelInvoke("RemovePlank");
        alreadyRunning = false;
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
