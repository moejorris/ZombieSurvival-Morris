//////////////////////////////////////////////
//Assignment/Lab/Project: Zombie Survival
//Name: Joe Morris
//Section: SGD.213.0021
//Instructor: Professor Locklear
//Date: 05/02/2024
/////////////////////////////////////////////

using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    //Health script for the zombies. 
    [SerializeField] GameObject powerUpPrefab;
    [SerializeField] public float health = 100;
    [SerializeField] int pointsOnDeath = 50;

    bool dead = false;
    public void TakeDamage(float damage) //Called by zombie limbs. 
    {
        if(dead ) return;
        if(GameManager.instance.instaKillActive)
        {
            damage = health;
        }

        health -= damage;
        if(health <= 0)
        {
            dead = true;
            Die();
        }
    }

    public void Die(bool roundOver = false)
    {
        //when the zombie dies, it tells it's other scripts that it has died
        GetComponent<ZombieAttack>().OnDeath();
        GetComponent<EnemySoundBank>().PlayDeathSound();

        //then it lets the spawn manager it has died, and tells it to check if the round has ended. roundOver is set to true by CheckForRoundOver()
        //round over has to be false when KillingAllZombies() runs otherwise this will cause an infinite loop and crash the game.
        if(!roundOver)
        {
            ZombieSpawnManager.instance.CheckForRoundOver();
        }
        
        //then the player receives 50 points for killing it
        GameManager.instance.PlayerScore(pointsOnDeath);

        //decide if powerup spawns
        if(Random.Range(0f, 1f) > 0.98f && !GetComponent<ZombieAttack>().canRemoveBarrier) //zombie must have entered playable area. 2% chance of dropping powerup
        {
            Instantiate(powerUpPrefab, transform.position, Quaternion.identity);
        }
        
        //then it gets destroyed
        Destroy(gameObject);
    }
}
