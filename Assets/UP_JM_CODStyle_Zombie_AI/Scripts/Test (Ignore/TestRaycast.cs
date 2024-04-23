using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRaycast : MonoBehaviour
{
    [SerializeField] float damage = 50;
    // Update is called once per frame
    void Update()
    {
        Vector3 position = Camera.main.transform.position;
        if(Input.GetButtonDown("Fire1"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.Log("ray fired.");
            if(Physics.Raycast(ray, out RaycastHit hit))
            {
                if(hit.transform.GetComponent<ZombieLimb>())
                {
                    Debug.Log("GetComponent (Not Parent) hit: " + hit.collider.name);
                    hit.transform.GetComponent<ZombieLimb>().Hit(damage);
                }
            }
        }        
    }
}
