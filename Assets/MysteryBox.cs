using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteryBox : InteractableObject
{
    [SerializeField] GameObject[] wonderWeaponPool;
    [SerializeField] GameObject[] weaponPool;
    [SerializeField] int weaponsToDisplay = 2;
    List<GameObject> displayWeapons = new List<GameObject>();
    public override void Interact()
    {
        GetComponent<Animator>().SetTrigger("Activate");
        currentlyInteractable = false;
    }

    public void Done()
    {
        currentlyInteractable = true;
    }


    void PickDisplayWeapons()
    {
        displayWeapons.Clear();

        if(wonderWeaponPool.Length > 0)
        {
            displayWeapons.Add(wonderWeaponPool[Random.Range(0, wonderWeaponPool.Length)].GetComponent<PlayerGun>().displayModel);
        }

        for(int i = 0; i < weaponPool.Length; i++)
        {
            //pick random weapon in weapon pool
            //check if weapon already in display weapons
            //if not, add to display weapons
            //if display weapons.Length >= weaponsToDisplay, return
        }
    }
}
