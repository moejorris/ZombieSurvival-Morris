using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    public static PlayerWeaponManager instance;
    [SerializeField] Transform weaponHolderTransform;
    [SerializeField] List<GameObject> weaponInventory = new List<GameObject>();
    [SerializeField] int currentWeaponIndex;
    [SerializeField] int maxWeapons = 2;

    [SerializeField] GameObject defaultWeapon;

    void Awake()
    {
        instance = this;

        for(int i = 0; i < maxWeapons; i++)
        {
            weaponInventory.Add(null);
        }
    }

    void Start()
    {
        GainWeapon(defaultWeapon);
    }

    int GetEmptySlot()
    {
        if(weaponInventory[0] == null)
        {
            return 0;
        }

        for(int i = 0; i < weaponInventory.Count; i++)
        {
            if(weaponInventory[i] == null)
            {
                return i;
            }
        }

        return -1; //returns -1 if there are no empty slots
    }

    public void GainWeapon(GameObject gunPrefab)
    {
        int emptySlot = GetEmptySlot();
        
        GameObject gunGameObject = Instantiate(gunPrefab, weaponHolderTransform);
        gunGameObject.transform.localPosition = Vector3.zero;
        gunGameObject.transform.localEulerAngles = Vector3.zero;

        if(emptySlot != -1) //if an empty slot is available
        {
            if(weaponInventory[currentWeaponIndex] != null)
            {
                weaponInventory[currentWeaponIndex].SetActive(false);
            }
            weaponInventory[emptySlot] = gunGameObject;
            currentWeaponIndex = emptySlot;
        }
        else //replace current weapon
        {
            GameObject gunToBeReplaced = weaponInventory[currentWeaponIndex];
            weaponInventory[currentWeaponIndex] = gunGameObject;
            Destroy(gunToBeReplaced);
        }
    }

    public bool IsWeaponAtMaxAmmo(int slotWeaponIsIn)
    {
        return weaponInventory[slotWeaponIsIn].GetComponent<PlayerGun>().CheckIfWeaponAtMaxAmmo();
    }

    public void RefillWeaponAmmo(int slotWeaponIsIn = -1)
    {
        if(slotWeaponIsIn == -1)
        {
            slotWeaponIsIn = currentWeaponIndex;
        }

        weaponInventory[slotWeaponIsIn].GetComponent<PlayerGun>().RefillReserveAmmo();
    }

    public void RefillAllAmmo()
    {
        for(int i = 0; i < weaponInventory.Count; i++)
        {
            if(weaponInventory[i] != null)
            {
                weaponInventory[i].GetComponent<PlayerGun>().RefillReserveAmmo();
            }
        }
    }

    public int CheckIfPlayerHasGun(string weaponName) //checks if player has the gun being inputted and returns the slot it is in. returns -1 if player does not have it.
    {
        for(int i = 0; i < weaponInventory.Count; i++)
        {
            if(weaponInventory[i] != null && weaponInventory[i].GetComponent<PlayerGun>().GetWeaponName().Contains(weaponName)) //uses contains() rather than == so it checks if the weapon the player has is upgraded (weapon name adds a suffix when it is upgraded)
            {
                return i;
            }
        }

        return -1;
    }


    void SwitchWeapon(float changeValue)
    {
        changeValue = Mathf.Sign(changeValue);

        int nextWeaponIndex = currentWeaponIndex + (int) changeValue;

        if(nextWeaponIndex >= weaponInventory.Count)
        {
            nextWeaponIndex = 0;
        }
        else if(nextWeaponIndex < 0)
        {
            nextWeaponIndex = weaponInventory.Count - 1;
        }

        if(weaponInventory[nextWeaponIndex] != null)
        {
            weaponInventory[currentWeaponIndex].SetActive(false);
            weaponInventory[nextWeaponIndex].SetActive(true);
            currentWeaponIndex = nextWeaponIndex;
        }
    }

    void Update()
    {
        if(Input.mouseScrollDelta.y != 0)
        {
            SwitchWeapon(Input.mouseScrollDelta.y);
        }
    }
}
