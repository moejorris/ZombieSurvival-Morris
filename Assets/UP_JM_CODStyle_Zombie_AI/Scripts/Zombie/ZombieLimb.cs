using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieLimb : MonoBehaviour
{
    [SerializeField] Vector3 dismemberedScale = Vector3.zero;
    [SerializeField] bool canBeDestroyed = true;
    [SerializeField] bool deathOnDestroyed = false;
    [SerializeField] float timeUntilDeathAfterLimbDestruction = 1;
    [SerializeField] float limbHealth = 50;
    [SerializeField] float damageMultiplier = 1;

    public void Hit(float damage)
    {
        GetComponentInParent<ZombieHealth>().TakeDamage(damage * damageMultiplier);
        
        if(canBeDestroyed)
        {
            limbHealth -= damage;
            if(limbHealth <= 0)
            {
                limbHealth = 0;
                transform.localScale = dismemberedScale;

                if(deathOnDestroyed)
                {
                    GetComponentInParent<ZombieHealth>().Invoke("Die", timeUntilDeathAfterLimbDestruction);
                }
                this.enabled = false;
            }
        }

    }
}
