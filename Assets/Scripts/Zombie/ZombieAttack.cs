using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAttack : MonoBehaviour
{
    public bool canRemoveBarrier = true; //zombie can only remove barrier before entering the playable area.
    public float barrierRemovalRate = 2;
    [SerializeField] float damageToPlayer = 10;
    [SerializeField] float attackPlayerRate;
    public ZombieEntrance currentEntrance;
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            InvokeRepeating("Attack", 0, 1);
        }

        if(other.GetComponent<ZombieEntrance>())
        {
            if(other.GetComponent<ZombieEntrance>().BeginRemovingBarrier(this))
            {
                currentEntrance = other.GetComponent<ZombieEntrance>();
            }
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

    public void OnDeath()
    {
        if(currentEntrance != null) //tells the entrance it is attacking (if any) to stop removing planks because it is dead.
        {
            currentEntrance.StopRemovingBarrier();
            currentEntrance = null;
        }

        CancelInvoke();
    }
}
