
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    string[] powerUps = new string[] { "instaKill", "doublePoints", "maxAmmo"};

    void OnTriggerEnter()
    {
        GameManager.instance.PickUpPowerUp(powerUps[Random.Range(0, powerUps.Length)]);
        Destroy(gameObject);
    }
}
