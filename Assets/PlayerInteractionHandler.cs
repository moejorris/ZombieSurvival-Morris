using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionHandler : MonoBehaviour
{
    [SerializeField] KeyCode interactKey = KeyCode.F;
    [SerializeField] float keyHoldTime = 0.1f;
    [SerializeField] LayerMask interactableLayers;
    [SerializeField] InteractableObject currentInteractable;
    bool canInteract = false;

    void OnTriggerEnter(Collider other)
    {
        InteractableObject newInteractable = other.GetComponent<InteractableObject>();
        if(newInteractable)
        {
            if(newInteractable != currentInteractable)
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
        if (currentInteractable == null || !currentInteractable.mustLookAt)
        {
            return;
        }

        Physics.Raycast(Camera.main.transform.position + Camera.main.transform.forward * 0.1f, Camera.main.transform.forward, out RaycastHit hit, 100, interactableLayers);
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 100);
        canInteract = hit.transform != null && hit.transform.GetComponent<InteractableObject>() == currentInteractable;

        UpdateInteractText();
    }

    void Update()
    {
        if(currentInteractable != null && canInteract)
        {
            if(Input.GetKeyDown(interactKey))
            {
                if(currentInteractable.holdInteractKey)
                {
                    Invoke("InteractIfKeyIsHeld", keyHoldTime);
                }
                else
                {
                    currentInteractable.Interact();
                }
            }
        }
    }

    void InteractIfKeyIsHeld() //checks to see if the interact key is still held when it is invoked
    {
        if(Input.GetKey(interactKey))
        {
            currentInteractable.Interact();
        }
    }

    void UpdateInteractText()
    {
        if(canInteract)
        {
            UiController.instance.UpdateInteractText(currentInteractable.interactMessage);
        }
        else
        {
            UiController.instance.UpdateInteractText("");
        }
    }
}
