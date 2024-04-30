using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteryBox : InteractableObject
{
    [SerializeField] string buyMessage = "Press and hold F to buy a random weapon for 950 points";
    [SerializeField] string pickMessage = "Press and hold F to trade weapons";
    [SerializeField] Transform displayWeaponParent;
    [SerializeField] GameObject[] wonderWeaponPool;
    [SerializeField] GameObject[] weaponPool;
    [SerializeField] int weaponsToDisplay = 2;
    [SerializeField] bool addAllWeaponsToDisplay;
    PlayerGun weaponWon; //the weapon the player will actually receive. 
    bool cycleWeapons = false;
    public override void Interact()
    {
        GetComponent<AudioSource>().Play();
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
        DestroyDisplayWeapons();
    }

    void DestroyDisplayWeapons()
    {
        for(int i = 0; i < displayWeaponParent.childCount; i++)
        {
            Destroy(displayWeaponParent.GetChild(i).gameObject);
        }
    }

    void ShowWeaponToBeGiven()
    {
        GameObject weapon = null;

        //decide if player gets a wonder weapon
        bool wonderWeapon = Random.Range(0f, 1f) > 0.9f; //90% chance of getting a wonder weapon (raygun only right now)
        //choose random weapon
        if(wonderWeapon)
        {
            weapon = wonderWeaponPool[Random.Range(0, wonderWeaponPool.Length)];
        }
        else
        {
            weapon = weaponPool[Random.Range(0, weaponPool.Length)];
        }
        weaponWon = weapon.GetComponent<PlayerGun>();
        //instantiate weapon
        AddDisplayWeapon(weaponWon.displayModel, weaponWon.displaySpawnPoint, true);
        //currently interactable to true
        currentlyInteractable = true;
    }


    void PickDisplayWeapons()
    {
        Debug.Log("Pick random weapons to display");

        if(addAllWeaponsToDisplay || weaponsToDisplay > weaponPool.Length)
        {
            weaponsToDisplay = weaponPool.Length;
        }

        int weaponsAdded = 0;

        if(wonderWeaponPool.Length > 0)
        {
            //add random wonder weapon to displayweapons
            PlayerGun gunToAdd = wonderWeaponPool[Random.Range(0, wonderWeaponPool.Length)].GetComponent<PlayerGun>();
            GameObject model = Instantiate(gunToAdd.displayModel);
            model.name = gunToAdd.GetWeaponName();
            AddDisplayWeapon(model, gunToAdd.displaySpawnPoint);
            weaponsAdded++;
            weaponsToDisplay++;
        }
        
        
        while(weaponsAdded < weaponsToDisplay)
        {
            //pick random weapon in weapon pool
            PlayerGun gunScriptReference = weaponPool[Random.Range(0, weaponPool.Length)].GetComponent<PlayerGun>();
            GameObject weaponModel = Instantiate(gunScriptReference.displayModel);
            weaponModel.name = gunScriptReference.GetWeaponName();

            //check if weapon already in display weapons
            bool weaponAlreadyExists = false;
            for(int i = 0; i < displayWeaponParent.childCount; i++)
            {
                if(displayWeaponParent.GetChild(i).name == weaponModel.name)
                {
                    Destroy(weaponModel);
                    weaponAlreadyExists = true;
                    //wanted to do a continue here so it would just skip to the next iteration of the while loop, but that doesn't work because this would skip to the next iteration of the for loop inside the while loop. using a bool and if state made more sense.
                }
            }

            if(!weaponAlreadyExists)
            {
                AddDisplayWeapon(weaponModel, gunScriptReference.displaySpawnPoint);
                weaponsAdded++;
            }
        }

    }

    void AddDisplayWeapon(GameObject weaponModel, Vector3 positionOffset, bool enabled = false)
    {
        weaponModel.transform.parent = displayWeaponParent;
        weaponModel.transform.localPosition = positionOffset;
                
        weaponModel.transform.forward = -weaponModel.transform.right;
        weaponModel.layer = 0;

        for(int i = 0; i < weaponModel.transform.childCount; i++) //change all children's layers so player can't see them through walls (weaponCam). Not sure if there is a better way to do this, but yes I wish there was a lest complicated (and probably resource heavy) way to do this
        {
            Transform child = weaponModel.transform.GetChild(i);
            child.gameObject.layer = 0;
            if(child.childCount > 0)
            {
                for(int j = 0; j < child.childCount; j++)
                {
                    child.GetChild(j).gameObject.layer = 0;
                }
            }
        }
        
        weaponModel.gameObject.SetActive(enabled);
    }

    IEnumerator CycleDisplayWeapons()
    {
        PickDisplayWeapons();
        cycleWeapons = true;
        int i = 0;
        while(cycleWeapons == true)
        {
            displayWeaponParent.GetChild(i).gameObject.SetActive(false);

            i++;

            if(i >= displayWeaponParent.childCount)
            {
                i = 0;
            }

            displayWeaponParent.GetChild(i).gameObject.SetActive(true);
            yield return new WaitForSeconds(0.25f);
        }
        DestroyDisplayWeapons();
        ShowWeaponToBeGiven();

    }
}
