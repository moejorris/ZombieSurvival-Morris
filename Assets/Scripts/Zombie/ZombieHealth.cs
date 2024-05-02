using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    [SerializeField] public float health = 100;
    public void TakeDamage(float damage)
    {
        Debug.Log("Took " + damage);
        health -= damage;
        if(health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Death!");
        GetComponent<ZombieAttack>().OnDeath();
        GetComponent<EnemySoundBank>().PlayDeathSound();
        ZombieSpawnManager.instance.CheckForRoundOver();

        // gameObject.SetActive(false);
        Destroy(gameObject);
        //Let roundManager know zombie has died
    }
}
