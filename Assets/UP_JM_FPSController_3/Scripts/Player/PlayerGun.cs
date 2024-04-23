//////////////////////////////////////////////
//Assignment/Lab/Project: Collision Resolution
//Name: Joe Morris
//Section: SGD.213.0021
//Instructor: Professor Locklear
//Date: 03/18/2024
/////////////////////////////////////////////

using UnityEngine;
public class PlayerGun : MonoBehaviour //This script is setup to work without Sound effects (audioClip and audioSource), Ui, and Particle Effects if they are not needed. 
{
    [Header("Input")]
    [SerializeField] KeyCode reloadButton = KeyCode.R;
    
    [Header("References")]
    [SerializeField] Animator weaponAnimator;
    [SerializeField] AudioSource audioSource; //Optional

    [Header("Ammo Definitions")]
    [SerializeField] int currentAmmo; //How much ammo is currently available
    [SerializeField] int maxAmmo; //how much ammo will be available at the start and after reloading

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
    [SerializeField] Sprite crosshair;
    // [SerializeField] UiAmmoController uiAmmoController; //Reference to the UiAmmoController to tell it when to update.

    bool canShoot;
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

        currentAmmo = maxAmmo;
        canShoot = true;
        UpdateUi();
    }
    void OnEnable()
    {
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

        UpdateWeaponRotation();
    }

    void UpdateWeaponRotation()
    {
        Vector3 lookAtTarget;
        Vector3 lerpRotation;

        RaycastHit hit;

        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxDistance, shootableObjects))
        {
            lookAtTarget = hit.point;
        }
        else
        {
            lookAtTarget = transform.forward;
        }
        Vector3 currentRotation = transform.eulerAngles;
        transform.LookAt(lookAtTarget);
        Vector3 lookAtAngles = transform.eulerAngles;
        transform.eulerAngles = currentRotation;
        lerpRotation = Vector3.Lerp(transform.eulerAngles, lookAtAngles, Time.deltaTime);
        transform.eulerAngles = lerpRotation;
    }

    void UpdateUi()
    {
        if(UiAmmoController.instance != null)
        {
            UiAmmoController.instance.InitUi(weaponName, currentAmmo, maxAmmo, activeAmmoSprite, inactiveAmmoSprite);
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
        currentAmmo--;
        
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
            UiAmmoController.instance.UpdateAmmo(currentAmmo);
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

        if(Physics.Raycast(ray, out hit, maxDistance, shootableObjects))
        {
            if(hit.transform.GetComponent<Health>() != null) //If object hit has a health component inflict damage
            {
                hit.transform.GetComponent<Health>().TakeDamage(damageToInflict);
                

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
        if(currentAmmo == maxAmmo || !canShoot)
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
        currentAmmo = maxAmmo;

        if(UiAmmoController.instance != null)
        {
            UiAmmoController.instance.UpdateAmmo(currentAmmo);
        }
        // weaponAnimator.SetTrigger("Up");
    }
}
