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
    int slotPlayerHasWeapon;

    PlayerWeaponManager player;

    void Start()
    {
        Destroy(transform.GetChild(0).gameObject); //destroys display model used for reference in editor.

        buyPrice = price;
        weaponName = weapon.GetComponent<PlayerGun>().GetWeaponName();
        buyWeaponMessage = "Press and hold F to buy " + weaponName + " for " + buyPrice + " points";
        refillWeaponMessage = "Press and hold F to buy " + weaponName + " ammo for " + refillAmmoPrice + " points";

        GameObject displayWeapon = Instantiate(weapon.GetComponent<PlayerGun>().displayModel, transform.position, transform.rotation, transform);
        displayWeapon.transform.localPosition = weapon.GetComponent<PlayerGun>().displaySpawnPoint;
        
        displayWeapon.layer = 0; //take it out of WeaponCam layer so player cannot see it through walls.
        
        for(int i = 0; i < displayWeapon.transform.childCount; i++)
        {
            Transform child = displayWeapon.transform.GetChild(i);
            child.gameObject.layer = 0;
            if(child.childCount > 0)
            {
                for(int j = 0; j < child.childCount; j++)
                {
                    child.GetChild(j).gameObject.layer = 0;
                }
            }
        }

        player = PlayerWeaponManager.instance;
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
        UpdatePrice();
    }

    void UpdatePrice()
    {
        slotPlayerHasWeapon = player.CheckIfPlayerHasGun(weaponName);

        if(slotPlayerHasWeapon != -1)
        {
            if(!player.IsWeaponAtMaxAmmo(slotPlayerHasWeapon))
            {
                currentlyInteractable = true;
            }
            else
            {
                currentlyInteractable = false;
            }
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
