//////////////////////////////////////////////
//Assignment/Lab/Project: Zombie Survival
//Name: Joe Morris
//Section: SGD.213.0021
//Instructor: Professor Locklear
//Date: 05/02/2024
/////////////////////////////////////////////

//Originally made for Collision Resolution, not modified since.
using UnityEngine;

public class PlayerHealth : Health //Player derivative of Health
{
    public static PlayerHealth instance; //used so other scripts can acess the readonly properties of Health and Score.
    [SerializeField] float healTime = 5f; //how long after taking damage before the player's health is restored;
    [SerializeField] AudioClip damageSound, deathSound; //sound effects played when taking damage and dying.
    AudioSource audioSource;

    public float MaxHealth
    {
        get { return maxHealth; }
    }
    public float Health
    {
        get{return currentHealth;}
    }

    void Awake()
    {
        instance = this;
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
    }

    void Heal()
    {
        currentHealth = maxHealth;
        UiController.instance.UpdateHealth();
    }

    public override void TakeDamage(float damageInflicted) //Adds playing the sound and updating the Ui to the base function. Also ensures the script is enabled to stop it from being called after the player is dead.
    {
        if(enabled) //this script is disabled when the player is paused, either when the game is over or the game is paused. This ensures nothing happens in either case.
        {
            CancelInvoke("Heal");

            audioSource.PlayOneShot(damageSound);
            base.TakeDamage(damageInflicted);
            UiController.instance.UpdateHealth();

            Invoke("Heal", healTime);
        }
    }

    public override void Die() //Unlike traps, we don't want the player to be destroyed, we just want the game to end, so we override the inherited Die() function.
    {
        audioSource.PlayOneShot(deathSound);
        ZombieSpawnManager.instance.PlayerDied();
        PlayerManager.instance.PausePlayer(false);
        UiController.instance.GameOver(RoundManager.instance.CurrentRound);
    }
}
