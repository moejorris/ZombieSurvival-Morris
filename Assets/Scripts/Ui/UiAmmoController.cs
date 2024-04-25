//////////////////////////////////////////////
//Assignment/Lab/Project: Collision Resolution
//Name: Joe Morris
//Section: SGD.213.0021
//Instructor: Professor Locklear
//Date: 03/18/2024
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

    public void InitUi(string weaponName, int currentAmmo, int maxAmmo, Sprite activeSprite, Sprite inactiveSprite)
    {
        weaponText.text = weaponName;
        _maxAmmo = maxAmmo;

        activeAmmoSprite = activeSprite;
        inactiveAmmoSprite = inactiveSprite;

        for(int i = 0; i < layoutGroupTransform.childCount; i++)
        {
            Destroy(layoutGroupTransform.GetChild(i).gameObject);
        }

        int dischargedShells = maxAmmo - currentAmmo;

        for(int i = 0; i < maxAmmo; i++)
        {
            Image ammoImage = Instantiate(ammoImagePrefab, layoutGroupTransform).GetComponent<Image>();
            ammoImage.sprite = i <= dischargedShells -1 ? inactiveAmmoSprite : activeAmmoSprite;
        }

        ammoCountText.text = "Ammo: " + currentAmmo + "/" + maxAmmo;
    }

    public void UpdateAmmo(int currentAmmo) //Updates the Ammo display.
    {
        int dischargedShells = _maxAmmo - currentAmmo;

        ammoCountText.text = "Ammo: " + currentAmmo + "/" + _maxAmmo; //Updates ammo text display correct amount of shells left.

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
