using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("Interactable Parameters")]
    public bool mustLookAt;
    public bool holdInteractKey;
    public string interactMessage = "Press F to...";
    public int price = 500;
    public virtual void Interact()
    {
    
    }
}
