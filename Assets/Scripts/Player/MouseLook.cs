//////////////////////////////////////////////
//Assignment/Lab/Project: Zombie Survival
//Name: Joe Morris
//Section: SGD.213.0021
//Instructor: Professor Locklear
//Date: 05/02/2024
/////////////////////////////////////////////

using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float sensitivityX = 1, sensitivityY = 1;
    [SerializeField] float maxLookAngle = 90;
    float xRotation = 0;
    Camera viewCam, weaponCam;

    // Start is called before the first frame update
    void Awake()
    {
        viewCam = Camera.main;
        weaponCam = Camera.main.transform.GetComponentInChildren<Camera>();        
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        RotateCam();
    }

    void GetInput()
    {
        xRotation = Mathf.Clamp(xRotation - (Input.GetAxis("Mouse Y") * sensitivityY), -maxLookAngle, maxLookAngle);
    }

    void RotateCam()
    {
        //rotate on y axis
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * sensitivityX);
        viewCam.transform.parent.localEulerAngles = new Vector3(xRotation, 0, 0);
    }

    public void SwitchFOV(bool sprint)
    {
        //targetFOV = sprint? sprintFOV : FOV;
    }


}
