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
    [SerializeField] float adsFOVMultiplier = 0.75f;
    float currentADSfov;
    Camera viewCam;

    // Start is called before the first frame update
    void Start()
    {
        viewCam = Camera.main;
        targetFOV = FOV;
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
        if(PlayerGun.currentGun.isAiming)
        {
            fov *= adsFOVMultiplier;

            fov = Mathf.Lerp(viewCam.fieldOfView, fov, Time.deltaTime * 7f);
        }
        else
        {
            fov = Mathf.Lerp(viewCam.fieldOfView, fov, Time.deltaTime * 14);
        }

        viewCam.fieldOfView = fov;
        if(additionalCameras.Length > 0)
        {
            for(int i = 0; i < additionalCameras.Length; i++)
            {
                additionalCameras[i].fieldOfView = fov;
            }
        }
    }
}
