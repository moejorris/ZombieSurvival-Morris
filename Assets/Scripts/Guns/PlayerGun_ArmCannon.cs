using UnityEngine;

public class PlayerGun_ArmCannon : PlayerGun
{
    bool shootButtonHeld;
    float shootButtonHoldTime = 0.5f;
    [SerializeField] float curChargeTime;

    [Header("Upgraded Arm Cannon Settings")]
    //?

    [Header("Power Beam Settings")]
    [SerializeField] float beamSpeed = 12f;
    [SerializeField] GameObject beamProjectilePrefab;
    
    [Header("Charge Beam Settings")]
    [SerializeField] GameObject chargeBeamProjectilePrefab;
    [SerializeField] GameObject chargeStandInPrefab;
    ChargeBeamStandIn spawnedChargeStandIn;
    [SerializeField] GameObject chargeParticle;
    [SerializeField] float chargeFullDamageMult = 2.25f;
    [SerializeField] float fullChargeTime = 1.5f;
    [SerializeField] float chargeBeamSpeed = 25f;

    [Header("Missile Settings")]
    [SerializeField] GameObject missileProjectilePrefab;
    [SerializeField] GameObject missileAOE;
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
        if(shootButtonHeld && curChargeTime > 0)
        {
            curChargeTime += Time.deltaTime * Time.timeScale;
            if(curChargeTime > fullChargeTime) curChargeTime = fullChargeTime;
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

        UpdateChargeStandIn();
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

    void UpdateChargeStandIn()
    {
        if(!spawnedChargeStandIn) return;
        else if(curChargeTime == 0) Destroy(spawnedChargeStandIn.gameObject);

        float charged = curChargeTime / fullChargeTime;
        charged = charged > 1 ? 1 : charged;

        spawnedChargeStandIn.UpdateChargeAmount(charged);
    }

    void StartChargeBeam()
    {
        if(spawnedChargeStandIn) Destroy(spawnedChargeStandIn.gameObject);

        spawnedChargeStandIn = Instantiate(chargeStandInPrefab, muzzleFlashTransform).GetComponent<ChargeBeamStandIn>();
        // spawnedChargeStandIn.transform.localPosition = 

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

        float chargeAmount = curChargeTime/fullChargeTime;
        chargeAmount = chargeAmount > 1 ? chargeAmount = 1 : chargeAmount;

        float damage = (damageToInflict * chargeFullDamageMult) * chargeAmount;
        damage = Mathf.Clamp(damage, damageToInflict, damageToInflict * chargeFullDamageMult);

        float speed = chargeBeamSpeed * chargeAmount;
        speed = Mathf.Max(speed, beamSpeed);

        Transform chargeTransform = ShootProjectile(chargeBeamProjectilePrefab, speed, damage, impactEffectPrefab).transform;
        float scale = chargeAmount;
        scale = Mathf.Max(scale, beamProjectilePrefab.transform.localScale.x * 2f);

        chargeTransform.localScale = Vector3.one * scale * 0.5f;

        curChargeTime = 0;

        Destroy(spawnedChargeStandIn.gameObject);
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
                Transform laserTransform = Instantiate(upgradedMuzzleFlashPrefab, muzzleFlashTransform.position, muzzleFlashTransform.rotation).transform;
                laserTransform.forward = GetProjectileDirection();
            }
            else
            {
                Instantiate(muzzleFlashPrefab, muzzleFlashTransform.position, muzzleFlashTransform.rotation);
            }
        }

        weaponAnimator.SetTrigger("Shoot");

        if(isUpgraded)
        {
            ShootRaycastBulletPenetration();
        }
        else
        {
            //Shoot Projectile
            ShootProjectile(beamProjectilePrefab, beamSpeed, damageToInflict, impactEffectPrefab);
        }
    }

    GameObject ShootProjectile(GameObject prefab, float speed, float damage, GameObject impactParticle = null, GameObject areaOfEffect = null)
    {
        if(isUpgraded) damage *= upgradedDamageMultiplier;

        PlayerProjectile projectile = Instantiate(prefab, muzzleFlashTransform.position, Quaternion.identity).GetComponent<PlayerProjectile>();
        projectile.InitProjectile(GetProjectileDirection(), speed, damage, impactParticle, bloodEffectPrefab, areaOfEffect);

        return projectile.gameObject;
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

        ShootProjectile(missileProjectilePrefab, missileSpeed, missileDamage, impactEffectPrefab, missileAOE);
    }
}
