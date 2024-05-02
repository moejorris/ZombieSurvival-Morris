
using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    [SerializeField] public float health = 100;
    [SerializeField] int pointsOnDeath = 50;
    public void TakeDamage(float damage)
    {
        if(GameManager.instance.instaKillActive)
        {
            damage = health;
        }

        health -= damage;
        if(health <= 0)
        {
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
        
        //then it gets destroyed
        Destroy(gameObject);
    }
}
