using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionHandler : MonoBehaviour
{
    [SerializeField] InteractableObject currentInteractable;
    bool canInteract = false;

    void OnTriggerEnter(Collider other)
    {
        InteractableObject newInteractable = other.GetComponent<InteractableObject>();
        if(newInteractable)
        {
            if(newInteractable != currentInteractable)
            {
                if(newInteractable.mustLookAt && Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit))
                {
                    if(hit.transform.GetComponent<InteractableObject>() == newInteractable)
                    {
                        currentInteractable = newInteractable;
                        UpdateInteractText();
                    }
                }
                else
                {
                    currentInteractable = newInteractable;
                    UpdateInteractText();
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        InteractableObject detectedInteractable = other.GetComponent<InteractableObject>();
        if(detectedInteractable && detectedInteractable == currentInteractable)
        {
            currentInteractable = null;
            UpdateInteractText();
        }
    }

    void Update()
    {
        if (currentInteractable == null) return;
        if(currentInteractable.mustLookAt && Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit))
        {
            if(hit.transform.GetComponent<InteractableObject>() != currentInteractable)
            {
                currentInteractable = null;
                UpdateInteractText();
            }
        }
    }

    void UpdateInteractText()
    {
        if(currentInteractable && canInteract)
        {
            UiController.instance.UpdateInteractText(currentInteractable.interactMessage);
        }
        else
        {
            UiController.instance.UpdateInteractText("");
        }
    }
}
