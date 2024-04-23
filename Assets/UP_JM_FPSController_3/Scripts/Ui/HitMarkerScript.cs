using System.Collections;
using System.Collections.Generic;
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
