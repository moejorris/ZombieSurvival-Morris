//////////////////////////////////////////////
//Assignment/Lab/Project: Zombie Survival
//Name: Joe Morris
//Section: SGD.213.0021
//Instructor: Professor Locklear
//Date: 05/02/2024
/////////////////////////////////////////////

using UnityEngine;

public class PowerUp : MonoBehaviour
{
    //powerup. holds different powerups, and chooses a random one to be when the player collides with it. then tells the game manager that it has been picked up, specifically which powerup.
    [SerializeField] float timeBeforeDestroying = 15;
    string[] powerUps = new string[] { "instaKill", "doublePoints", "maxAmmo"};

    void Start()
    {
        Destroy(gameObject, timeBeforeDestroying);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerMove>())
        {
            GameManager.instance.PickUpPowerUp(powerUps[Random.Range(0, powerUps.Length)]);
            Destroy(gameObject);
        }
    }
}
