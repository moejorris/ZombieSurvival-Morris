//////////////////////////////////////////////
//Assignment/Lab/Project: Zombie Survival
//Name: Joe Morris
//Section: SGD.213.0021
//Instructor: Professor Locklear
//Date: 05/02/2024
/////////////////////////////////////////////

using UnityEngine;

public class PlayerDoor : InteractableObject
{
    //used by doors and debris. When the player interacts with it, it is removed, but when opening a new section of the level, it also activates areas for the zombies to spawn as well as traverse.
    [SerializeField] GameObject[] objectsToDestroyOnInteract; //destroys objects when this door is opened. Objects like obstacles that tell zombies not to enter the area the door goes to, doors that go to the same area, etc.
    [SerializeField] GameObject[] objectsToEnableOnInteract; //objects that are enabled when this door is opened, such as spawn points that would be impossible for the zombies to reach the player until this door is opened.

    void Start()
    {
        if(!interactMessage.Contains("points") && !interactMessage.Contains("Points"))
        {
            interactMessage += " for " + price + " points";
        }
    }
    public override void Interact()
    {
        if(objectsToDestroyOnInteract.Length >= 1)
        {
            for(int i = 0; i < objectsToDestroyOnInteract.Length; i++)
            {
                if(objectsToDestroyOnInteract[i] != null)
                {
                    Destroy(objectsToDestroyOnInteract[i]);
                }
            }
        }

        if(objectsToEnableOnInteract.Length >= 1)
        {
            for(int i = 0; i < objectsToEnableOnInteract.Length; i++)
            {
                if(objectsToEnableOnInteract[i] != null)
                {
                    objectsToEnableOnInteract[i].SetActive(true);
                }
            }
        }

        Destroy(gameObject);
    }
}
