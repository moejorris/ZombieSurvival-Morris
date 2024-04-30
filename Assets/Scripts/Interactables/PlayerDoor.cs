using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDoor : InteractableObject
{
    [SerializeField] GameObject[] objectsToDestroyOnInteract; //destroys objects when this door is opened. Objects like obstacles that tell zombies not to enter the area the door goes to, doors that go to the same area, etc.

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

        Destroy(gameObject);
    }
}
