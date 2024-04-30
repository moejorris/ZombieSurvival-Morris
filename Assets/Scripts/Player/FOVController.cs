using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVController : MonoBehaviour
{
    public bool dynamicFOV;
    public float FOV = 60, maxFOV = 80;
    [SerializeField] float increaseSpeedFOV = 100f, decreaseSpeedFOV = 75f;
    [HideInInspector] public float targetFOV;
    [SerializeField] Camera[] additionalCameras;
    float adsFOV = 0.75f;
    float currentFOV = 0;
    Camera viewCam;

    // Start is called before the first frame update
    void Start()
    {
        viewCam = Camera.main;
        targetFOV = FOV;
        adsFOV = FOV*0.75f;
    }

    void LateUpdate()
    {
        FOVCheck();
    }

    void FOVCheck()
    {
        if(!dynamicFOV)
        {
            ChangeCamFov(FOV);
            return;
        }
        Vector3 controllerV = GetComponent<CharacterController>().velocity;
        targetFOV = FOV + (new Vector3(controllerV.x, 0, controllerV.z).magnitude * (maxFOV/FOV));
        float fovSmoothed = viewCam.fieldOfView = Mathf.MoveTowards(viewCam.fieldOfView, targetFOV, (viewCam.fieldOfView < targetFOV ? increaseSpeedFOV : decreaseSpeedFOV) * Time.unscaledDeltaTime);
        ChangeCamFov(fovSmoothed);
    }

    void ChangeCamFov(float fov)
    {
        // float changeSmooth = Mathf.Lerp(viewCam.fieldOfView, fov, Time.deltaTime);
        float changeSmooth = fov;
        
        viewCam.fieldOfView = changeSmooth;
        if(additionalCameras.Length > 0)
        {
            for(int i = 0; i < additionalCameras.Length; i++)
            {
                additionalCameras[i].fieldOfView = changeSmooth;
            }
        }
    }
}
