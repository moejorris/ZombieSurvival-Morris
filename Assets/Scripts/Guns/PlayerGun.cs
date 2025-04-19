//////////////////////////////////////////////
//Assignment/Lab/Project: Zombie Survival
//Name: Joe Morris
//Section: SGD.213.0021
//Instructor: Professor Locklear
//Date: 05/02/2024
/////////////////////////////////////////////

using System;
using UnityEngine;
public class PlayerGun : MonoBehaviour //This script is setup to work without Sound effects (audioClip and audioSource), Ui, and Particle Effects if they are not needed. 
{
    public static PlayerGun currentGun;

    //To do: make shooting run a coroutine/loop that shoots multiple raycasts when using weapons that fire multiple projectiles (like shotgun). *Cancelled*
    [Header("Input")]
    [SerializeField] KeyCode reloadButton = KeyCode.R;
    
    [Header("References")]
    [SerializeField] protected Animator weaponAnimator;
    [SerializeField] protected AudioSource audioSource; //Optional

    [Header("Ammo Definitions")]
    [SerializeField] bool unlimitedAmmo; //does not use bullets when shooting/never has to reload
    [SerializeField] int clipSize; //how much ammo can be held in each magazine
    [SerializeField] int startingReserveMagazines = 3; //how many reserve magazines of ammo the player has at the start. Replenished on max ammo.
    [SerializeField] int currentAmmo; //How much ammo is currently available
    [SerializeField] int currentReserveAmmo; //how much ammo there is left

    [Header("Shooting Definitions")]
    [SerializeField] bool fullAuto;
    [SerializeField] float maxSpread = 2;
    [SerializeField] float aimSpreadMultiplier = 0.1f;
    [SerializeField] protected int damageToInflict; //how much damage the weapon will do
    [SerializeField] protected float maxDistance; //the maximum amount of distance the weapon can shoot
    [SerializeField] protected LayerMask shootableObjects; //layermask of objects that can be shot

    [Header("Upgraded Modifiers")] //Modifiers used when the player upgrades the weapon.
    [SerializeField] protected float upgradedClipSizeMultiplier = 1.2f;
    [SerializeField] protected float upgradedDamageMultiplier = 2f;
    [SerializeField] protected float upgradedDistanceMultiplier = 3f;
    [SerializeField] protected float upgradedSpreadMultiplier = 0.5f;

    [Header("Graphics")] //Optional
    [SerializeField] protected Transform muzzleFlashTransform; //The transform of the empty located at the end of the weapons barrel
    [SerializeField] protected GameObject muzzleFlashPrefab; //The prefab of the muzzle flash particle effect
    [SerializeField] protected GameObject upgradedMuzzleFlashPrefab; //The prefab of the muzzle flash particle effect used when the gun is upgraded
    [SerializeField] protected GameObject impactEffectPrefab; //The prefab of the bullet impact particle effect
    [SerializeField] protected GameObject bloodEffectPrefab; //The prefab of the blood particle effect instantiated when hitting a zombie

    [Header("Audio")] //Optional sound effects for shooting and reloading.
    [SerializeField] protected AudioClip shootSound;
    [SerializeField] AudioClip reloadSound;

    [Header("Ui")] //Optional
    [SerializeField] string weaponName;
    [SerializeField] Sprite activeAmmoSprite;
    [SerializeField] Sprite inactiveAmmoSprite;
    [Header("Wall Buy References")]
    [SerializeField] public GameObject displayModel;
    [SerializeField] public Vector3 displaySpawnPoint;
    [SerializeField] public Vector3 displayEulerAngles;
    // [SerializeField] Sprite crosshair; //crosshair is no longer controlled by weapon.
    // [SerializeField] UiAmmoController uiAmmoController; //Reference to the UiAmmoController to tell it when to update.

    [SerializeField] protected bool canShoot;
    public bool isUpgraded; //accessed by weapon upgrader script
    bool reloading;
    public bool isAiming; //accessed by FOV controller
    protected bool paused;

    public void Pause(bool isPaused)
    {
        paused = isPaused;
        UpdateUi();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        currentAmmo = clipSize;
        currentReserveAmmo = clipSize * startingReserveMagazines;
        canShoot = false;
        UpdateUi();
    }
    protected virtual void OnEnable()
    {
        currentGun = this;
        canShoot = false;
        reloading = false;
        UpdateUi();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(paused)
        {
            return;
        }

        if(fullAuto)
        {
            if(Input.GetButton("Fire1") && canShoot)
            {
                ShootGun();
            }
        }
        else
        {
            if(Input.GetButtonDown("Fire1") && canShoot)
            {
                ShootGun();
            }
        }

        if(!reloading && Input.GetButton("Fire2"))
        {
            isAiming = true;
            weaponAnimator.SetBool("ADS", true);
            if(UiController.instance != null)
            {
                UiController.instance.ChangeCrosshairVisibility(false);
            }            
        }
        else
        {
            isAiming = false;
            weaponAnimator.SetBool("ADS", false);
            if(UiController.instance != null)
            {
                UiController.instance.ChangeCrosshairVisibility(true);
            } 
        }

        if(Input.GetKeyDown(reloadButton))
        {
            BeginReload();
        }

        UpdateBounceAnimationSpeed();
    }

    void UpdateUi()
    {
        int currentClipSize = isUpgraded ? Mathf.FloorToInt(clipSize * upgradedClipSizeMultiplier) : clipSize; //determines whether or not should use base clip size or the upgraded clip size

        if(UiAmmoController.instance != null)
        {
            UiAmmoController.instance.InitUi(weaponName, currentAmmo, currentClipSize, currentReserveAmmo, activeAmmoSprite, inactiveAmmoSprite, unlimitedAmmo);
        }
        if(UiController.instance != null)
        {
            // UiController.instance.ChangeCrosshairSprite(crosshair);
            UiController.instance.ChangeCrosshairSize(isUpgraded ? upgradedSpreadMultiplier * maxSpread : maxSpread);
        }
    }

    protected void UpdateBounceAnimationSpeed()
    {
        Vector3 charControllerVelocity = GetComponentInParent<CharacterController>().velocity;
        Vector2 v2Speed = new Vector2(charControllerVelocity.x, charControllerVelocity.z);
        float bounceSpeed = Mathf.Clamp(v2Speed.magnitude, 1, 100f);
        weaponAnimator.SetFloat("Speed", bounceSpeed);
    }

    protected virtual void ShootGun()
    {
        if(currentAmmo <= 0) //Trying to shoot with no ammo will cause reload
        {
            BeginReload();
            return;
        }

        canShoot = false; //can't shoot during shoot animation
        
        if(!unlimitedAmmo) //takes ammo if gun does not have unlimited ammo
        {
            currentAmmo--;
        }
        
        if(audioSource != null && shootSound != null) //play sound effect
        {
            audioSource.PlayOneShot(shootSound);
        }

        if(muzzleFlashPrefab != null && muzzleFlashTransform != null) //instantiate muzzle flash
        {
            if(isUpgraded && upgradedMuzzleFlashPrefab != null)
            {
                Instantiate(upgradedMuzzleFlashPrefab, muzzleFlashTransform.position, muzzleFlashTransform.rotation);
            }
            else
            {
                Instantiate(muzzleFlashPrefab, muzzleFlashTransform.position, muzzleFlashTransform.rotation);
            }
        }

        weaponAnimator.SetTrigger("Shoot"); //play shoot animation

        if(UiAmmoController.instance != null && !unlimitedAmmo) //update ammo in Ui
        {
            UiAmmoController.instance.UpdateAmmo(currentAmmo, currentReserveAmmo);
        }

        ShootRaycastBulletPenetration();
    }

    protected virtual void ShootRaycast() //No longer in use. ShootRaycastBulletPenetration() is now used in its place. Shoots raycast from the center of the screen. If it hits anything, inflicts damage (if object has Health) and spawns the bullet impact effect on it.
    {
        RaycastHit hit;
        Ray ray = new Ray(Camera.main.transform.position, DetermineBulletTrajectory());

        if(Physics.Raycast(ray, out hit, maxDistance, shootableObjects, QueryTriggerInteraction.Ignore))
        {
            if(hit.point != null) Debug.Log(hit.collider.name);
            if(hit.collider.GetComponent<ZombieLimb>() != null) //If object hit has a health component inflict damage
            {
                hit.collider.GetComponent<ZombieLimb>().TakeDamage(damageToInflict);
                Debug.Log("Hit limb");

                if(UiController.instance != null)
                {
                    UiController.instance.Invoke("SpawnHitmarker", 0.05f);
                }
            }
            
            if(impactEffectPrefab != null && hit.point != null) //Instantiate bullet impact particle effect on hit object
            {
                GameObject bulletImpact = Instantiate(impactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal), hit.transform);
                bulletImpact.transform.position = bulletImpact.transform.position + (bulletImpact.transform.forward * 0.1f); //slightly moves the PE outward from the surface so the bullet holes are visible
            }
        }
    }

    Vector3 DetermineBulletTrajectory() //determines the bullet trajectory based on the weapons spread, and if the weapon is being aimed or not.
    {
        float spread = maxSpread / 100f;
        float curSpread = isUpgraded ? upgradedSpreadMultiplier * spread : spread;
        float curAimSpread = isAiming ? aimSpreadMultiplier : 1;

        Vector3 bulletTrajectory = Camera.main.transform.forward;

        Vector3 spreadOffset =  
        (Camera.main.transform.right * UnityEngine.Random.Range(-curSpread, curSpread) * curAimSpread) + 
        (Camera.main.transform.up * UnityEngine.Random.Range(-curSpread, curSpread) * curAimSpread);

        if(Mathf.Abs(spreadOffset.magnitude) > curSpread)
        {
            spreadOffset = spreadOffset.normalized * curSpread;
        }

        bulletTrajectory += spreadOffset;

        return bulletTrajectory;
    }

    protected virtual void ShootRaycastBulletPenetration() //Shoots raycastAll from the center of the screen. If it hits anything, inflicts damage (if object has Health) and spawns the bullet impact effect on it. Uses raycast all so your bullets can go through zombies.
    {
        Ray ray = new Ray(Camera.main.transform.position, DetermineBulletTrajectory());

        float bulletDistance = isUpgraded ? maxDistance * upgradedDistanceMultiplier : maxDistance;

        RaycastHit[] hits = Physics.RaycastAll(ray, bulletDistance, shootableObjects, QueryTriggerInteraction.Ignore); //gets all objects the raycast hit

        Array.Sort(hits, delegate(RaycastHit hit1, RaycastHit hit2) //sorts through the list by distance (raycast all order is random/not sorted by default)
        {
            return hit1.distance.CompareTo(hit2.distance); 
        }
        );
        
        if(hits.Length > 0)
        {
            float currentDamage = isUpgraded ? damageToInflict * upgradedDamageMultiplier : damageToInflict;

            for(int i = 0; i < hits.Length; i++)
            {
                string consoleMessage = "item " + i + ": " + hits[i].transform.name;
                if(hits[i].transform.parent)
                {
                    consoleMessage+= " " + hits[i].transform.parent.name;
                }
                Debug.Log(consoleMessage);
                if(i > 0)
                {
                    currentDamage*= 0.5f;
                }

                if(hits[i].collider.GetComponent<ZombieLimb>())
                {
                    hits[i].collider.GetComponent<ZombieLimb>().TakeDamage(currentDamage);
                    if(UiController.instance != null && i < 1)
                    {
                        UiController.instance.Invoke("SpawnHitmarker", 0.05f);
                    }

                    if(bloodEffectPrefab != null && hits[i].point != null && i < 1) //Instantiate bullet impact particle effect on hit object (only the first hit object)
                    {
                        GameObject bulletImpact = Instantiate(bloodEffectPrefab, hits[i].point, Quaternion.LookRotation(hits[i].normal));
                        bulletImpact.transform.position = bulletImpact.transform.position + (bulletImpact.transform.forward * 0.2f); //slightly moves the PE outward from the surface so the bullet holes are visible
                    }
                }
                else //the first collision with a non-zombie stops the for loop so you can't shoot zombies through walls, only through zombies.
                {
                    if(impactEffectPrefab != null && hits[i].point != null) //Instantiate bullet impact particle effect on hit object
                    {
                        GameObject bulletImpact = Instantiate(impactEffectPrefab, hits[i].point, Quaternion.LookRotation(hits[i].normal), hits[i].transform);
                        bulletImpact.transform.position = bulletImpact.transform.position + (bulletImpact.transform.forward * 0.1f); //slightly moves the PE outward from the surface so the bullet holes are visible
                    }
                    return; //if a wall is hit, then the bullet stops there (the for loop stops and no other objects are hit or damaged)
                }
            }
        }
    }

    void CanShoot() //Called by animations using animation events to signal to this script when the player can shoot or reload again. Called in Shooting and Reload animations toward the end of the animations.
    {
        canShoot = true;
        reloading = false;
    }

    void BeginReload() //Determines if it can begin reloading, and if so plays sound effect and plays animation.
    {
        int currentClipSize = isUpgraded ? Mathf.FloorToInt(clipSize * upgradedClipSizeMultiplier) : clipSize;
        if(currentAmmo == currentClipSize || !canShoot || currentReserveAmmo <= 0)
        {
            return;
        }

        if(audioSource != null && reloadSound != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }

        canShoot = false;
        reloading = true;
        weaponAnimator.SetTrigger("Reload");
    }

    void ReloadAmmo() //Called by reload animation, using an animation event towards the end of the animation. Refills current ammo, and tells Ui to reflect that.
    {
        int currentClipSize = isUpgraded ? Mathf.FloorToInt(clipSize * upgradedClipSizeMultiplier) : clipSize; //determines whether or not should use base clip size or the upgraded clip size

        int ammoDifferential = currentClipSize - currentAmmo; //how much ammo is required to fill the current magazine;
        if(currentReserveAmmo >= ammoDifferential)
        {
            currentAmmo += ammoDifferential;
            currentReserveAmmo -= ammoDifferential;
        }
        else
        {
            currentAmmo += currentReserveAmmo;
            currentReserveAmmo = 0;
        }
        

        if(UiAmmoController.instance != null)
        {
            UiAmmoController.instance.UpdateAmmo(currentAmmo, currentReserveAmmo);
        }
    }

    public bool CheckIfWeaponAtMaxAmmo() //called by interactable objects and weapon manager
    {
        int currentClipSize = isUpgraded ? Mathf.FloorToInt(clipSize * upgradedClipSizeMultiplier) : clipSize; //determines whether or not should use base clip size or the upgraded clip size
        
        if(currentReserveAmmo == currentClipSize * startingReserveMagazines)
        {
            return true;
        }
        return false;
    }

    public void RefillReserveAmmo() //called by interactable objects and weapon manager
    {
        currentReserveAmmo = isUpgraded ? Mathf.FloorToInt(clipSize * upgradedClipSizeMultiplier) * startingReserveMagazines : clipSize * startingReserveMagazines;
        if(UiAmmoController.instance != null)
        {
            UiAmmoController.instance.UpdateAmmo(currentAmmo, currentReserveAmmo);
        }
    }

    public string GetWeaponName()
    {
        return weaponName;
    }

    public virtual void UpgradeWeapon()
    {
        weaponName += " (Upgraded)";
        isUpgraded = true;

        currentAmmo = Mathf.FloorToInt(clipSize * upgradedClipSizeMultiplier);
        RefillReserveAmmo();
        currentReserveAmmo *= 2;

        UpdateUi();
    }
}
