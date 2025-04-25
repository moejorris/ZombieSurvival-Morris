//////////////////////////////////////////////
//Assignment/Lab/Project: Zombie Survival
//Name: Joe Morris
//Section: SGD.213.0021
//Instructor: Professor Locklear
//Date: 05/02/2024
/////////////////////////////////////////////

using UnityEngine;

public class WallBuyWeapon : InteractableObject
{
    //holds a weapon for the player to buy off the wall. If the player has already bought the weapon, it allows them to buy ammo for it.
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
        if(refillAmmoPrice == 0) refillWeaponMessage = "";


        GameObject displayWeapon = Instantiate(weapon.GetComponent<PlayerGun>().displayModel, transform.position, transform.rotation, transform);
        displayWeapon.transform.localPosition = weapon.GetComponent<PlayerGun>().displaySpawnPoint;
        displayWeapon.transform.localEulerAngles = weapon.GetComponent<PlayerGun>().displayEulerAngles;
        
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
        else if(price == refillAmmoPrice && refillAmmoPrice > 0)
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
