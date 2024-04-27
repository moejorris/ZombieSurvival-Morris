using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBuyWeapon : InteractableObject
{
    [Header("Wallbuy Weapon")]
    string buyWeaponMessage;
    string refillWeaponMessage;

    int buyPrice;
    [SerializeField] int refillAmmoPrice;
    [SerializeField] GameObject weapon;

    void Start()
    {
        Destroy(transform.GetChild(0).gameObject);

        buyPrice = price;

        buyWeaponMessage = "Press F to buy " + weapon.GetComponent<PlayerGun>().GetWeaponName() + " for " + buyPrice + " points";
        refillWeaponMessage = "Press F to buy " + weapon.GetComponent<PlayerGun>().GetWeaponName() + " ammo for " + refillAmmoPrice + " points";

        GameObject displayWeapon = Instantiate(weapon.GetComponent<PlayerGun>().displayModel, transform.position, transform.rotation, transform);
        
        displayWeapon.layer = 0;
        
        for(int i = 0; i < displayWeapon.transform.childCount; i++)
        {
            displayWeapon.transform.GetChild(i).gameObject.layer = 0;
        }
    }

    bool PlayerHasThisGun()
    {
        //delete the below return when weapon manager is completed. Maybe make this a function of the weapon managers??? depends
        return true;

        /*
            PlayerWeaponManager weaponManager = PlayerWeaponManager.Instance;
            string thisWeaponName = weapon.GetComponenet<PlayerGun>().GetWeaponName();
            
            for(int i = 0; i < weaponManager.weaponInventory.Length)
            {
                PlayerGun weapon = weaponManager.weaponInventory[i].GetComponent<PlayerGun>();
                if(weapon.GetWeaponName() == thisWeaponName)
                {
                    return true;
                }
            }

            return false;
        */
    }

    public override void Interact()
    {
        Debug.Log("Interacted");
        // if()
        // {
        //     ;
        //     /*
        //         PlayerWeaponManager weaponManager = PlayerWeaponManager.instance;
        //         string currentWeapon = weapon.GetComponent<PlayerGun>().GetWeaponName();
        //         bool playerHasWeapon;

        //         for(int i = 0; i < PlayerWeaponManager.instance.weaponInventory.Length)
        //         {
        //             PlayerGun weapon = PlayerWeaponManager.instance.weaponInventory[i].GetComponent<PlayerGun>();
        //             if(weapon != null && weapon.GetWeaponName() == currentWeapon)
        //             {

        //             }
        //         }
        //     */
        // }
    }

    void OnTriggerEnter(Collider other)
    {
        if(PlayerHasThisGun())
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
