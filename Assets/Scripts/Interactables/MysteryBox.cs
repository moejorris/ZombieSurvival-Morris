//////////////////////////////////////////////
//Assignment/Lab/Project: Zombie Survival
//Name: Joe Morris
//Section: SGD.213.0021
//Instructor: Professor Locklear
//Date: 05/02/2024
/////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MysteryBox : InteractableObject
{
    //When interactig, it cycles through various models of weapons to display, and then displays the weapon the player has won. The player can choose not to pick it up.
    //This is also controlled by the mystery box's animator using animation events.
    [SerializeField] string buyMessage = "Press and hold F to buy a random weapon for 950 points";
    [SerializeField] string pickMessage = "Press and hold F to trade weapons";
    [SerializeField] Transform displayWeaponParent;
    [SerializeField] GameObject[] wonderWeaponPool;
    [SerializeField] GameObject[] weaponPool;
    int weaponsToDisplay = 2; //how many weapons to cycle through (graphical) before stopping and showing the weapon the player will receive
    PlayerGun weaponWon; //the weapon the player will actually receive.
    int buyPrice;
    bool cycleWeapons = false;

    void Start()
    {
        interactMessage = buyMessage;
        buyPrice = price;
    }
    public override void Interact()
    {
        if(cycleWeapons) return;

        if(interactMessage == buyMessage)
        {
            GetComponent<AudioSource>().Play();
            GetComponent<Animator>().SetTrigger("Activate");
            currentlyInteractable = false;
            StartCoroutine(CycleDisplayWeapons());
        }
        else if(interactMessage == pickMessage)
        {
            PlayerWeaponManager.instance.GainWeapon(weaponWon.gameObject);
            currentlyInteractable = false;
            DestroyDisplayWeapons();
            GetComponent<Animator>().SetTrigger("Close");
        }
    }

    public void StopCyclingWeaponModels()
    {
        cycleWeapons = false;
        interactMessage = pickMessage;
        price = 0;
        DestroyDisplayWeapons();
        ShowWeaponToBeGiven();
        currentlyInteractable = true;
    }

    public void Done()
    {
        currentlyInteractable = true;
        interactMessage = buyMessage;
        price = buyPrice;
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
        //create duplicate lists and remove weapons player already has
        List<GameObject> currentWonderWeaponPool = wonderWeaponPool.ToList();
        List<GameObject> currentWeaponPool = weaponPool.ToList();

        for(int i = 0; i < currentWonderWeaponPool.Count; i++)
        {
            string weaponName = currentWonderWeaponPool[i].GetComponent<PlayerGun>().GetWeaponName(); //gets the weapon name that is currently being checked against the weapon inventory

            if(PlayerWeaponManager.instance.CheckIfPlayerHasGun(weaponName) != -1)
            {
                Debug.Log("Player has this gun. removing " + currentWonderWeaponPool[i].name + " from pool");
                currentWonderWeaponPool.Remove(currentWonderWeaponPool[i]);
            }
        }

        for(int i = 0; i < currentWeaponPool.Count; i++)
        {
            string weaponName = currentWeaponPool[i].GetComponent<PlayerGun>().GetWeaponName(); //gets the weapon name that is currently being checked against the weapon inventory

            if(PlayerWeaponManager.instance.CheckIfPlayerHasGun(weaponName) != -1)
            {
                Debug.Log("Player has this gun. removing " + currentWeaponPool[i].name + " from pool");
                currentWeaponPool.Remove(currentWeaponPool[i]);
            }
        }

        //decide if player gets a wonder weapon
        bool wonderWeapon = Random.Range(0f, 1f) > 0.95f; //5% chance of getting a wonder weapon (raygun only right now)
        //choose random weapon
        if(wonderWeapon && currentWonderWeaponPool.Count > 0) //checks if the player should receive a wonder weapon AND if they dont have any
        {
            weaponWon = currentWonderWeaponPool[Random.Range(0, currentWonderWeaponPool.Count)].GetComponent<PlayerGun>();
        }
        else
        {
            weaponWon = currentWeaponPool[Random.Range(0, currentWeaponPool.Count)].GetComponent<PlayerGun>();
        }
        //instantiate weapon
        AddDisplayWeapon(Instantiate(weaponWon.displayModel), weaponWon.displaySpawnPoint, true);
        //currently interactable to true
    }


    void PickDisplayWeapons()
    {
        Debug.Log("Pick random weapons to display");

        weaponsToDisplay = weaponPool.Length;

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
    }
}
