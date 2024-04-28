using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    [SerializeField] Transform weaponHolderTransform;
    [SerializeField] List<GameObject> weaponInventory = new List<GameObject>();
    [SerializeField] int currentWeaponIndex;
    [SerializeField] int maxWeapons = 2;

    [SerializeField] GameObject defaultWeapon;

    void Awake()
    {
        for(int i = 0; i < maxWeapons; i++)
        {
            weaponInventory.Add(null);
        }
    }

    void Start()
    {
        Debug.Log(weaponInventory[0] == null);
        GainWeapon(defaultWeapon);
    }

    int GetEmptySlot()
    {
        if(weaponInventory[0] == null)
        {
            Debug.Log("player has no weapons: " + 0);
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

    public void RefillWeaponAmmo(int slotWeaponIsIn)
    {

    }

    public int CheckIfPlayerHasGun(string weaponName) //checks if player has the gun being inputted and returns the slot it is in. returns -1 if player does not have it.
    {
        for(int i = 0; i < weaponInventory.Count; i++)
        {
            if(weaponInventory[i] != null && weaponInventory[i].GetComponent<PlayerGun>().GetWeaponName() == weaponName)
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
