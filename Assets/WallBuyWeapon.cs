using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WallBuyWeapon : InteractableObject
{
    [Header("Wallbuy Weapon")]
    string buyWeaponMessage;
    string refillWeaponMessage;

    int buyPrice;
    [SerializeField] int refillAmmoPrice;
    [SerializeField] GameObject weapon;
    string weaponName;
    PlayerWeaponManager player;
    int slotPlayerHasWeapon;

    void Start()
    {
        Destroy(transform.GetChild(0).gameObject); //destroys display model used for reference in editor.

        buyPrice = price;
        weaponName = weapon.GetComponent<PlayerGun>().GetWeaponName();
        buyWeaponMessage = "Press F to buy " + weaponName + " for " + buyPrice + " points";
        refillWeaponMessage = "Press F to buy " + weaponName + " ammo for " + refillAmmoPrice + " points";

        GameObject displayWeapon = Instantiate(weapon.GetComponent<PlayerGun>().displayModel, transform.position, transform.rotation, transform);
        
        displayWeapon.layer = 0; //take it out of WeaponCam layer so player cannot see it through walls.
        
        for(int i = 0; i < displayWeapon.transform.childCount; i++)
        {
            displayWeapon.transform.GetChild(i).gameObject.layer = 0;
        }
    }

    public override void Interact()
    {
        if(price == buyPrice)
        {
            player.GainWeapon(weapon);
        }
        else if(price == refillAmmoPrice)
        {
            player.RefillWeaponAmmo(slotPlayerHasWeapon);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        UpdatePrice(other.GetComponent<PlayerWeaponManager>());
    }

    void UpdatePrice(PlayerWeaponManager playerWeaponManager)
    {
        if(playerWeaponManager)
        {
            player = playerWeaponManager;
            slotPlayerHasWeapon = player.CheckIfPlayerHasGun(weaponName);

            if(slotPlayerHasWeapon != -1)
            {
                price = refillAmmoPrice;
                interactMessage = refillWeaponMessage;
            }
            else
            {
                price = buyPrice;
                interactMessage = buyWeaponMessage;
            }
        }
    }
}
