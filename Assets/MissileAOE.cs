using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileAOE : MonoBehaviour
{
    public float damage = 10f;

    void Start()
    {
        Collider[] colliders= Physics.OverlapSphere(transform.position, transform.localScale.x /2f);

        foreach(Collider collider in colliders)
        {
            if(collider.GetComponent<ZombieLimb>())
            {
                collider.GetComponent<ZombieLimb>().TakeDamage(damage);
            }
        }
    }
}
