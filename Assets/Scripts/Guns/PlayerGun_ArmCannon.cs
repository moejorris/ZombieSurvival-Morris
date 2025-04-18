using UnityEngine;

public class PlayerGun_ArmCannon : PlayerGun
{
    bool shootButtonHeld;
    float shootButtonHoldTime = 0.5f;
    float curChargeTime;

    [Header("Power Beam Settings")]
    [SerializeField] GameObject beamProjectilePrefab;
    
    [Header("Charge Beam Settings")]
    [SerializeField] GameObject chargeParticle;
    [SerializeField] float chargeFullDamageMult = 2.25f;
    [SerializeField] float fullChargeTime = 3f;

    [Header("Missile Settings")]
    [SerializeField] GameObject missileProjectilePrefab;
    [SerializeField] int missileDamage;
    [SerializeField] float missileSpeed = 7f;

    [Header("Arm Cannon Sounds")]
    [SerializeField] AudioClip pickUpSound;
    [SerializeField] AudioClip equipSound;
    [SerializeField] AudioClip beamChargeStartSound;
    [SerializeField] AudioClip beamChargeLoopSound;
    [SerializeField] AudioSource beamChargeSource;
    [SerializeField] AudioClip beamChargeShootSound;
    [SerializeField] AudioClip missileShootSound;


    GameObject chargeParticleSpawned;

    // Start is called before the first frame update
    protected override void Start()
    {
        //play metroid sound
        audioSource.PlayOneShot(pickUpSound);
        base.Start();
    }

    protected override void OnEnable()
    {
        audioSource.PlayOneShot(equipSound);
        base.OnEnable();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if(paused)
        {
            return;
        }

        if(weaponAnimator.GetCurrentAnimatorStateInfo(0).ToString().Contains("Idle") && !canShoot)
        {
            Debug.Log("canShoot true fail. Enabling");
            canShoot = true;
        }

        if(Input.GetButtonDown("Fire1") && canShoot && curChargeTime == 0)
        {
            ShootGun();
        }
        else if(Input.GetButton("Fire1") && canShoot && !shootButtonHeld)
        {
            Invoke(nameof(ButtonIsHeld), shootButtonHoldTime);
        }
        else if(Input.GetButtonDown("Fire2") && canShoot && curChargeTime == 0)
        {
            ShootMissile();
        }
        else if(shootButtonHeld && canShoot)
        {
            if(curChargeTime == 0)
            {
                StartChargeBeam();
            }

        }
        else if(shootButtonHeld && curChargeTime > 0)
        {
            curChargeTime = Mathf.Clamp(curChargeTime + Time.deltaTime * Time.timeScale, curChargeTime, fullChargeTime);
        }
        
        if(!Input.GetButton("Fire1"))
        {
            if(curChargeTime > 0)
            {
                ShootChargeBeam();
            }
            
            CancelInvoke(nameof(ButtonIsHeld));
            shootButtonHeld = false;
        }

        UpdateBounceAnimationSpeed();
        UpdateChargeBlendTree();
    }

    void ButtonIsHeld()
    {
        if(Input.GetButton("Fire1"))
        {
            shootButtonHeld = true;
        }
    }

    void UpdateChargeBlendTree()
    {
        weaponAnimator.SetFloat("Charged", curChargeTime/fullChargeTime);
    }

    void StartChargeBeam()
    {
        beamChargeSource.enabled = true;
        beamChargeSource.clip = beamChargeStartSound;
        beamChargeSource.loop = false;
        beamChargeSource.Play();
        Invoke(nameof(PlayChargeLoop), beamChargeStartSound.length);

        curChargeTime = Time.deltaTime * Time.timeScale;

        chargeParticleSpawned = Instantiate(chargeParticle, muzzleFlashTransform);
        canShoot = false;
    }

    void PlayChargeLoop() //Plays looping sound after charge startup sound
    {
        canShoot = false;
        beamChargeSource.Stop();
        beamChargeSource.enabled = true;
        beamChargeSource.clip = beamChargeLoopSound;
        beamChargeSource.loop = true;
        beamChargeSource.Play();
    }

    void ShootChargeBeam()
    {
        weaponAnimator.SetTrigger("Shoot_Charge");
        CancelInvoke(nameof(PlayChargeLoop));
        audioSource.PlayOneShot(beamChargeShootSound);
        beamChargeSource.loop = false;
        beamChargeSource.enabled = false;
        canShoot = false;
        Destroy(chargeParticleSpawned);

        curChargeTime = 0;
    }

    protected override void ShootGun()
    {
        canShoot = false;

        if(shootSound)
        {
            audioSource?.PlayOneShot(shootSound);
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

        weaponAnimator.SetTrigger("Shoot");

        //Shoot Projectile
    }

    Vector3 GetProjectileDirection()
    {
        RaycastHit hit;

        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxDistance, shootableObjects, QueryTriggerInteraction.Ignore))
        {
            Debug.DrawRay(muzzleFlashTransform.position, hit.point - muzzleFlashTransform.position, Color.blue, 3f);
            return (hit.point - muzzleFlashTransform.position).normalized;
        }

        return muzzleFlashTransform.forward;
    }

    void ShootMissile()
    {
        canShoot = false;
        audioSource.PlayOneShot(missileShootSound);
        weaponAnimator.SetTrigger("Shoot_Missile");
        PlayerProjectile missile = Instantiate(missileProjectilePrefab, muzzleFlashTransform.position, Quaternion.identity).GetComponent<PlayerProjectile>();
        missile.InitProjectile(GetProjectileDirection(), missileSpeed, missileDamage);
    }
}
