//////////////////////////////////////////////
//Assignment/Lab/Project: Zombie Survival
//Name: Joe Morris
//Section: SGD.213.0021
//Instructor: Professor Locklear
//Date: 05/02/2024
/////////////////////////////////////////////

using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    //parent class for interactable objects. Player checks if an object is interactable by using GetComponent<InteractableObject>()
    [Header("Interactable Parameters")]
    public bool currentlyInteractable = true;
    public bool stayActiveAfterInteract;
    public bool mustLookAt;
    public bool holdInteractKey;
    public string interactMessage = "Press F to...";
    public int price = 500;

    public virtual void Interact()
    {
        
    }

    public virtual void CancelInteract()
    {

    }
}
