using UnityEngine;

public class PlayerGun_ArmCannon : PlayerGun
{
    bool shootButtonHeld;
    float shootButtonHoldTime = 0.5f;
    float curChargeTime;
    
    [Header("Charge Beam Settings")]
    [SerializeField] float fullChargeTime = 3f;

    [Header("Missile Settings")]
    [SerializeField] GameObject missilePrefab;

    [Header("Arm Cannon Sounds")]
    [SerializeField] AudioClip pickUpSound;
    [SerializeField] AudioClip equipSound;
    [SerializeField] AudioClip beamChargeStartSound;
    [SerializeField] AudioClip beamChargeLoopSound;
    [SerializeField] AudioClip beamChargeShootSound;
    [SerializeField] AudioClip missileShootSound;

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

        if(Input.GetButtonDown("Fire1") && canShoot && curChargeTime == 0)
        {
            ShootGun();
            Invoke(nameof(ButtonIsHeld), shootButtonHoldTime);
        }
        else if(shootButtonHeld && canShoot)
        {
            curChargeTime += Time.deltaTime * Time.timeScale;
        }
        
        if(Input.GetButtonUp("Fire1"))
        {
            CancelInvoke(nameof(ButtonIsHeld));
            shootButtonHeld = false;

            if(curChargeTime > 0)
            {
                curChargeTime = 0;
            }
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

    void ShootChargeBeam()
    {
        audioSource.PlayOneShot(beamChargeShootSound);
    }
}
