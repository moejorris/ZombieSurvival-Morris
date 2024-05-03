
using UnityEngine;

public class PowerUp : MonoBehaviour
{
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
