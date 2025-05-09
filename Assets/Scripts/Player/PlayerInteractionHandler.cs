//////////////////////////////////////////////
//Assignment/Lab/Project: Zombie Survival
//Name: Joe Morris
//Section: SGD.213.0021
//Instructor: Professor Locklear
//Date: 05/02/2024
/////////////////////////////////////////////

using UnityEngine;

public class PlayerInteractionHandler : MonoBehaviour
{
    //Controls how the player interacts with interactable objects, like doors, weapon wall buys, mystery box, weapon upgrader.
    [SerializeField] KeyCode interactKey = KeyCode.F;
    [SerializeField] float keyHoldTime = 0.1f;
    [SerializeField] LayerMask interactableLayers;
    [SerializeField] InteractableObject currentInteractable;
    [SerializeField] AudioClip chaChingSound;
    bool keyHeld;
    bool canInteract = false;

    void OnTriggerEnter(Collider other)
    {
        InteractableObject newInteractable = other.GetComponent<InteractableObject>();
        if(newInteractable)
        {
            if(newInteractable != currentInteractable && newInteractable.currentlyInteractable)
            {

                currentInteractable = newInteractable;
                canInteract = true;
                UpdateInteractText();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        InteractableObject detectedInteractable = other.GetComponent<InteractableObject>();
        if(detectedInteractable && detectedInteractable == currentInteractable)
        {
            currentInteractable = null;
            canInteract = false;
            UpdateInteractText();
        }
    }

    void FixedUpdate()
    {
        if (currentInteractable == null)
        {
            return;
        }

        UpdateInteractText();

        if(!currentInteractable.currentlyInteractable)
        {
            return;
        }


        if(currentInteractable.mustLookAt)
        {
            Physics.Raycast(Camera.main.transform.position + Camera.main.transform.forward * 0.1f, Camera.main.transform.forward, out RaycastHit hit, 100, interactableLayers);
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 100);
            canInteract = hit.transform != null && hit.transform.GetComponent<InteractableObject>() == currentInteractable;

            UpdateInteractText();
        }
    }

    void Update()
    {
        if(currentInteractable != null && canInteract)
        {
            if(!currentInteractable.currentlyInteractable) return;

            
            if(Input.GetKey(interactKey) && keyHeld == false)
            {
                keyHeld = true;
                if(currentInteractable.holdInteractKey)
                {
                    Invoke("InteractIfKeyIsHeld", keyHoldTime);
                }
                else
                {
                    ActivateInteractable();
                }
            }

            if(Input.GetKeyUp(interactKey))
            {
                keyHeld = false;
                CancelInvoke("InteractIfKeyIsHeld");
                currentInteractable.CancelInteract();
            }
        }
        else
        {
            keyHeld = false;
        }
    }

    void ActivateInteractable()
    {
        if(GameManager.instance.SpendPoints(currentInteractable.price))
        {
            if(currentInteractable.price > 0)
            {
                PlayBuySoundEffect();
            }
            currentInteractable.Interact();
            if(!currentInteractable.stayActiveAfterInteract)
            {
                currentInteractable = null;
                canInteract = false;
            }
            UpdateInteractText();
        }
    }

    void InteractIfKeyIsHeld() //checks to see if the interact key is still held when it is invoked
    {
        if(Input.GetKey(interactKey))
        {
            ActivateInteractable();
        }
    }

    void UpdateInteractText()
    {
        if(canInteract && currentInteractable.currentlyInteractable)
        {
            UiController.instance.UpdateInteractText(currentInteractable.interactMessage);
        }
        else
        {
            UiController.instance.UpdateInteractText("");
        }
    }

    public void PlayBuySoundEffect()
    {
        GetComponent<AudioSource>().PlayOneShot(chaChingSound);
    }
}
