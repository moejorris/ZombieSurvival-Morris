using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAttack : MonoBehaviour
{
    public bool canRemoveBarrier = true; //zombie can only remove barrier before entering the playable area.
    public float barrierRemovalRate = 2;
    [SerializeField] float damageToPlayer = 10;
    [SerializeField] float attackPlayerRate;

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            InvokeRepeating("Attack", 0, 1);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            CancelInvoke("Attack");
        }
    }

    void Attack()
    {
        //play sound immediately
        Invoke("DamagePlayer", 0.3f);
    }

    void DamagePlayer()
    {
        PlayerHealth.instance.TakeDamage(damageToPlayer);
        GetComponent<Animator>().SetTrigger("Attack");
    }
}
