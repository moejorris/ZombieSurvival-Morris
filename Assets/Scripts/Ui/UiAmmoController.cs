//////////////////////////////////////////////
//Assignment/Lab/Project: Zombie Survival
//Name: Joe Morris
//Section: SGD.213.0021
//Instructor: Professor Locklear
//Date: 05/02/2024
/////////////////////////////////////////////

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiAmmoController : MonoBehaviour
{
    public static UiAmmoController instance;

    [Header("References")]
    [SerializeField] TextMeshProUGUI weaponText;
    [SerializeField] TextMeshProUGUI ammoCountText;
    [SerializeField] Transform layoutGroupTransform;
    [SerializeField] GameObject ammoImagePrefab;
    Sprite activeAmmoSprite;
    Sprite inactiveAmmoSprite;

    //Ammo fields used 
    int _maxAmmo;

    void Awake()
    {
        instance = this;
    }

    public void InitUi(string weaponName, int currentAmmo, int clipSize, int currentReserveAmmo, Sprite activeSprite, Sprite inactiveSprite, bool infiniteAmmo = false)
    {
        weaponText.text = weaponName;
        _maxAmmo = clipSize;

        if(infiniteAmmo)
        {
            ammoCountText.gameObject.SetActive(false);
            layoutGroupTransform.gameObject.SetActive(false);
            return;
        }
        else
        {
            ammoCountText.gameObject.SetActive(true);
            layoutGroupTransform.gameObject.SetActive(true);
        }

        activeAmmoSprite = activeSprite;
        inactiveAmmoSprite = inactiveSprite;

        for(int i = 0; i < layoutGroupTransform.childCount; i++)
        {
            Destroy(layoutGroupTransform.GetChild(i).gameObject);
        }

        int dischargedShells = clipSize - currentAmmo;

        for(int i = 0; i < clipSize; i++)
        {
            Image ammoImage = Instantiate(ammoImagePrefab, layoutGroupTransform).GetComponent<Image>();
            ammoImage.sprite = i <= dischargedShells -1 ? inactiveAmmoSprite : activeAmmoSprite;
        }

        ammoCountText.text = "Ammo: " + currentAmmo + "/" + currentReserveAmmo;


    }

    public void UpdateAmmo(int currentAmmo, int currentReserveAmmo) //Updates the Ammo display.
    {
        int dischargedShells = _maxAmmo - currentAmmo;

        ammoCountText.text = "Ammo: " + currentAmmo + "/" + currentReserveAmmo; //Updates ammo text display correct amount of shells left.

        for(int i = 0; i < layoutGroupTransform.childCount; i++) //Loops through shells that have been used and not used and applies the corresponding sprite.
        {
            if(i <= dischargedShells -1)
            {
                layoutGroupTransform.GetChild(i).GetComponent<Image>().sprite = inactiveAmmoSprite;
                // Debug.Log("sprite discharged");
            }
            else
            {
                layoutGroupTransform.GetChild(i).GetComponent<Image>().sprite = activeAmmoSprite;
                // Debug.Log("sprite active");
            }
        }
    }
}
