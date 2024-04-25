//////////////////////////////////////////////
//Assignment/Lab/Project: Collision Resolution
//Name: Joe Morris
//Section: SGD.213.0021
//Instructor: Professor Locklear
//Date: 03/18/2024
/////////////////////////////////////////////

using UnityEngine;

public class Health : MonoBehaviour //Parent to PlayerInteractions and Trap, as both objects have health that can be decremented.
{
    [SerializeField] protected float maxHealth, currentHealth; //So child classes can access it.

    void Start() //sets current health to max health at start.
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float damageInflicted) //Called by other scripts, inflicts damage and checks if the object needs to die.
    {
        currentHealth -= damageInflicted;
        
        if(currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }
}
