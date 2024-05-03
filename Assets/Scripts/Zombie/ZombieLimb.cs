//////////////////////////////////////////////
//Assignment/Lab/Project: Zombie Survival
//Name: Joe Morris
//Section: SGD.213.0021
//Instructor: Professor Locklear
//Date: 05/02/2024
/////////////////////////////////////////////

using UnityEngine;

public class ZombieLimb : MonoBehaviour
{
    //Attached to different colliders on the zombies. Each limb has a different damage multiplier, and if destroyed, can result in the zombie dying. 
    //Each limb multiplies the damage taken, and tells the zombie health script attached to the transform.root how much damage to take from it's health.
    [SerializeField] Vector3 dismemberedScale = Vector3.zero;
    [SerializeField] bool canBeDestroyed = true;
    [SerializeField] bool deathOnDestroyed = false;
    [SerializeField] float maxTimeUntilDeathAfterLimbDestruction = 3;
    [SerializeField] float limbHealth = 50;
    [SerializeField] float damageMultiplier = 1;
    [SerializeField] int scoreOnShoot = 10;

    void Start()
    {
        if(canBeDestroyed)
        {
            limbHealth = GetComponentInParent<ZombieHealth>().health * (limbHealth/100);
        }
    }

    public void TakeDamage(float damage)
    {
        GetComponentInParent<ZombieHealth>().TakeDamage(damage * damageMultiplier);
        GameManager.instance.PlayerScore(scoreOnShoot);
        if(canBeDestroyed)
        {
            limbHealth -= damage;
            if(limbHealth <= 0)
            {
                limbHealth = 0;
                transform.localScale = dismemberedScale;

                if(deathOnDestroyed)
                {
                    if(GetComponentInParent<ZombieHealth>())
                    {
                        bool liveLimbless = Random.Range(0, 100) > 90; //10% chance that the zombie will walk around headless for the specified time
                        GetComponentInParent<ZombieHealth>().Invoke("Die", Random.Range(0, liveLimbless ? maxTimeUntilDeathAfterLimbDestruction : 0));
                    }
                }
                this.enabled = false;
            }
        }

    }
}
