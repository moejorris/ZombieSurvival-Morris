using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteryBox : InteractableObject
{
    [SerializeField] Transform displayWeaponPosition;
    [SerializeField] GameObject[] wonderWeaponPool;
    [SerializeField] GameObject[] weaponPool;
    [SerializeField] int weaponsToDisplay = 2;
    List<GameObject> displayWeapons = new List<GameObject>();

    bool cycleWeapons = false;
    public override void Interact()
    {
        GetComponent<Animator>().SetTrigger("Activate");
        currentlyInteractable = false;
        StartCoroutine(CycleDisplayWeapons());
    }

    public void StopCyclingWeaponModels()
    {
        cycleWeapons = false;
    }

    public void Done()
    {
        currentlyInteractable = true;
    }


    void PickDisplayWeapons()
    {
        displayWeapons.Clear();

        if(weaponPool.Length + wonderWeaponPool.Length < weaponsToDisplay)
        {
            weaponsToDisplay = weaponPool.Length + wonderWeaponPool.Length;
        }

        int weaponsAdded = 0;

        if(wonderWeaponPool.Length > 0)
        {
            //add random wonder weapon to displayweapons
            weaponsToDisplay++;
        }
        
        
        while(weaponsAdded < weaponsToDisplay)
        {
            //pick random weapon in weapon pool
            //check if weapon already in display weapons
            //if not, add to display weapons
            weaponsToDisplay++;
        }

        displayWeapons.Clear();
    }

    IEnumerator CycleDisplayWeapons()
    {
        PickDisplayWeapons();
        cycleWeapons = true;
        int i = 0;
        while(cycleWeapons)
        {
            displayWeaponPosition.GetChild(i).gameObject.SetActive(false);

            i++;

            if(i >= displayWeapons.Count)
            {
                i = 0;
            }

            displayWeaponPosition.GetChild(i).gameObject.SetActive(true);

            yield return new WaitForSeconds(0.1f);
        }
    }
}
