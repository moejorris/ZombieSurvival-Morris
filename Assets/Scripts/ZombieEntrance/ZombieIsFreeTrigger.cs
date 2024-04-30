
using UnityEngine;

public class ZombieIsFreeTrigger : MonoBehaviour
{
    //once a zombie has exited the barrier (and entered the playable area) the zombies can no longer take down barriers.
    //this works by once the zombie exits (comes out the other side of) the attached trigger collider, it tells the zombie that they can no longer break barriers.
    void OnTriggerExit(Collider other)
    {
        ZombieAttack zombie = other.gameObject.GetComponent<ZombieAttack>();
        if(zombie && zombie.canRemoveBarrier)
        {
            zombie.canRemoveBarrier = false;
        }
    }
}
