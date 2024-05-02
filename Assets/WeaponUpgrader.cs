using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUpgrader : InteractableObject
{
    public override void Interact()
    {
        PlayerGun.currentGun.UpgradeWeapon();
    }

    void OnTriggerEnter()
    {
        if(PlayerGun.currentGun.isUpgraded)
        {
            currentlyInteractable = false;
        }
        else
        {
            currentlyInteractable = true;
            interactMessage = "Press and Hold F to upgrade " + PlayerGun.currentGun.GetWeaponName();
        }
    }
}
