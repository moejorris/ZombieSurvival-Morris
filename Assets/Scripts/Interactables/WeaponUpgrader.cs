//////////////////////////////////////////////
//Assignment/Lab/Project: Zombie Survival
//Name: Joe Morris
//Section: SGD.213.0021
//Instructor: Professor Locklear
//Date: 05/02/2024
/////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUpgrader : InteractableObject
{

    //when the player interacts with the weapon upgrader (and has enough points), this tells the weapon to upgrade and use it's own upgrade modifiers.
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
            interactMessage = "Press and Hold F to upgrade " + PlayerGun.currentGun.GetWeaponName() + " for " + price + " points";
        }
    }
}
