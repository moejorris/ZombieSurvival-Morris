using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieLimb : MonoBehaviour
{
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
