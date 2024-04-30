//////////////////////////////////////////////
//Assignment/Lab/Project: Zombie Survival / Final Project
//Name: Joe Morris
//Section: SGD.213.0021
//Instructor: Professor Locklear
//Date: 05/02/2024
/////////////////////////////////////////////

using UnityEngine;
public class PlayerGun : MonoBehaviour //This script is setup to work without Sound effects (audioClip and audioSource), Ui, and Particle Effects if they are not needed. 
{
    //To do: make shooting run a coroutine/loop that shoots multiple raycasts when using weapons that fire multiple projectiles (like shotgun)
    [Header("Input")]
    [SerializeField] KeyCode reloadButton = KeyCode.R;
    
    [Header("References")]
    [SerializeField] Animator weaponAnimator;
    [SerializeField] AudioSource audioSource; //Optional

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
    [SerializeField] int damageToInflict; //how much damage the weapon will do
    [SerializeField] float maxDistance; //the maximum amount of distance the weapon can shoot
    [SerializeField] LayerMask shootableObjects; //layermask of objects that can be shot

    [Header("Graphics")] //Optional
    [SerializeField] Transform muzzleFlashTransform; //The transform of the empty located at the end of the weapons barrel
    [SerializeField] GameObject muzzleFlashPrefab; //The prefab of the muzzle flash particle effect
    [SerializeField] GameObject impactEffectPrefab; //The prefab of the bullet impact particle effect

    [Header("Audio")] //Optional sound effects for shooting and reloading.
    [SerializeField] AudioClip shootSound;
    [SerializeField] AudioClip reloadSound;

    [Header("Ui")] //Optional
    [SerializeField] string weaponName;
    [SerializeField] Sprite activeAmmoSprite;
    [SerializeField] Sprite inactiveAmmoSprite;
    [Header("Wall Buy References")]
    [SerializeField] public GameObject displayModel;
    [SerializeField] public Vector3 displaySpawnPoint;
    // [SerializeField] Sprite crosshair; crosshair is no longer controlled by weapon.
    // [SerializeField] UiAmmoController uiAmmoController; //Reference to the UiAmmoController to tell it when to update.

    [SerializeField] bool canShoot;
    bool reloading;
    bool isAiming;
    bool paused;

    public void Pause(bool isPaused)
    {
        paused = isPaused;
        UpdateUi();
    }

    // Start is called before the first frame update
    void Start()
    {

        currentAmmo = clipSize;
        currentReserveAmmo = clipSize * startingReserveMagazines;
        canShoot = false;
        UpdateUi();
    }
    void OnEnable()
    {
        canShoot = false;
        reloading = false;
        UpdateUi();
    }

    // Update is called once per frame
    void Update()
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
        if(UiAmmoController.instance != null)
        {
            UiAmmoController.instance.InitUi(weaponName, currentAmmo, clipSize, currentReserveAmmo, activeAmmoSprite, inactiveAmmoSprite);
        }
        if(UiController.instance != null)
        {
            // UiController.instance.ChangeCrosshairSprite(crosshair);
            UiController.instance.ChangeCrosshairSize(maxSpread);
        }
    }

    void UpdateBounceAnimationSpeed()
    {
        Vector3 charControllerVelocity = GetComponentInParent<CharacterController>().velocity;
        Vector2 v2Speed = new Vector2(charControllerVelocity.x, charControllerVelocity.z);
        float bounceSpeed = Mathf.Clamp(v2Speed.magnitude, 1, 100f);
        weaponAnimator.SetFloat("Speed", bounceSpeed);
    }

    void ShootGun()
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
            Instantiate(muzzleFlashPrefab, muzzleFlashTransform.position, muzzleFlashTransform.rotation);
        }

        weaponAnimator.SetTrigger("Shoot"); //play shoot animation

        if(UiAmmoController.instance != null) //update ammo in Ui
        {
            UiAmmoController.instance.UpdateAmmo(currentAmmo, currentReserveAmmo);
        }

        ShootRaycast();
    }

    void ShootRaycast() //Shoots raycast from the center of the screen. If it hits anything, inflicts damage (if object has Health) and spawns the bullet impact effect on it.
    {
        float curSpread = maxSpread / 100f;
        float curAimSpread = isAiming ? aimSpreadMultiplier : 1;

        Vector3 bulletTrajectory = Camera.main.transform.forward;

        Vector3 spreadOffset =  
        (Camera.main.transform.right * Random.Range(-curSpread, curSpread) * curAimSpread) + 
        (Camera.main.transform.up * Random.Range(-curSpread, curSpread) * curAimSpread);

        if(Mathf.Abs(spreadOffset.magnitude) > curSpread)
        {
            spreadOffset = spreadOffset.normalized * curSpread;
        }

        bulletTrajectory += spreadOffset;

        RaycastHit hit;
        Ray ray = new Ray(Camera.main.transform.position, bulletTrajectory);

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

    void CanShoot() //Called by animations to signal to this script when the player can shoot or reload again.
    {
        canShoot = true;
        reloading = false;
    }

    void BeginReload() //Determines if it can begin reloading, and if so plays sound effect and plays animation.
    {
        if(currentAmmo == clipSize || !canShoot || currentReserveAmmo <= 0)
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

    void ReloadAmmo() //Called by reload animation, towards the end of the animation. Refills current ammo, and tells Ui to reflect that.
    {
        int ammoDifferential = clipSize - currentAmmo; //how much ammo is required to fill the current magazine;
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
        // weaponAnimator.SetTrigger("Up");
    }

    public bool CheckIfWeaponAtMaxAmmo()
    {
        if(currentReserveAmmo == clipSize*startingReserveMagazines)
        {
            return true;
        }
        return false;
    }

    public void RefillReserveAmmo()
    {
        currentReserveAmmo = clipSize * startingReserveMagazines;
        if(UiAmmoController.instance != null)
        {
            UiAmmoController.instance.UpdateAmmo(currentAmmo, currentReserveAmmo);
        }
    }

    public string GetWeaponName()
    {
        return weaponName;
    }
}
