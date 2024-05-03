//////////////////////////////////////////////
//Assignment/Lab/Project: Zombie Survival
//Name: Joe Morris
//Section: SGD.213.0021
//Instructor: Professor Locklear
//Date: 05/02/2024
/////////////////////////////////////////////

using UnityEngine;

public class ZombieAttack : MonoBehaviour
{
    //Controls the zombies attacks to the player and barriers.
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
            if(other.GetComponent<ZombieEntrance>().BeginRemovingBarrier(this)) //tells barrier when to start removing planks
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
        //play sound and animation immediately to warn player
        GetComponent<EnemySoundBank>().PlayAttackSound();
        GetComponent<Animator>().SetTrigger("Attack");

        //damage player 0.3f seconds after
        Invoke("DamagePlayer", 0.3f);
    }

    void DamagePlayer()
    {
        PlayerHealth.instance.TakeDamage(damageToPlayer);
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
