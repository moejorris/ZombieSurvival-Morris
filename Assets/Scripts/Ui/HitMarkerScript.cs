//////////////////////////////////////////////
//Assignment/Lab/Project: Zombie Survival
//Name: Joe Morris
//Section: SGD.213.0021
//Instructor: Professor Locklear
//Date: 05/02/2024
/////////////////////////////////////////////


using UnityEngine;

public class HitMarkerScript : MonoBehaviour
{
    [SerializeField] float speed = 2;
    [SerializeField] CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();    
    }

    // Update is called once per frame
    void Update()
    {
        if(canvasGroup.alpha != 0)
        {
            canvasGroup.alpha -= Time.deltaTime * speed;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
