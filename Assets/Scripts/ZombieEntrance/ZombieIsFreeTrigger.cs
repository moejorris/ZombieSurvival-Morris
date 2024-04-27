using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieIsFreeTrigger : MonoBehaviour
{
    void OnTriggerExit(Collider other)
    {
        ZombieAttack zombie = other.gameObject.GetComponent<ZombieAttack>();
        if(zombie && zombie.canRemoveBarrier)
        {
            zombie.canRemoveBarrier = false;
        }
    }
}
