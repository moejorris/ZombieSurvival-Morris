using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBuyWeapon : InteractableObject
{
    [Header("Wallbuy Weapon")]
    [SerializeField] GameObject weapon;

    void Start()
    {
        Destroy(transform.GetChild(0).gameObject);
        interactMessage = "Press F to buy " + weapon.GetComponent<PlayerGun>().GetWeaponName() + " for " + price + "points";
        GameObject displayWeapon = Instantiate(weapon.GetComponent<PlayerGun>().displayModel, transform.position, transform.rotation, transform);
        displayWeapon.layer = 0;
    }
}
