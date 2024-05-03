//////////////////////////////////////////////
//Assignment/Lab/Project: Zombie Survival
//Name: Joe Morris
//Section: SGD.213.0021
//Instructor: Professor Locklear
//Date: 05/02/2024
/////////////////////////////////////////////

using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance; //Public static instance for other scripts to access this script
    [SerializeField] KeyCode pauseButton = KeyCode.Escape;
    public float sensX, sensY, FOV; //the sensitivity and FOV options stored here when they are changed in the pause menu.

    // public bool dynamicFOV; //Dynamic FOV control has been removed; It is a scrapped feature and was unfinished.

    FOVController fOVController; //Refers to the FOV controller.
    MouseLook mouseLook;

    // Start is called before the first frame update
    void Awake() //Sets instance to this, locks cursor and hides it, get references, and initializes the options.
    {
        instance = this;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        fOVController = GetComponent<FOVController>();
        mouseLook = GetComponent<MouseLook>();

        InitOptions();
    }

    #region pause menu

    // Update is called once per frame
    void Update()
    {
        GetInput();
    }

    void GetInput()
    {
        if(Input.GetKeyDown(pauseButton) && UiController.instance != null) //Pauses/Resumes game if the player presses Esc
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if(UiController.instance.pauseScreen.activeSelf) //If pause screen was already active, game is being resumed. Otherwise it is being paused.
        {
            UpdateOptions(); //on close, saves what player changed inputs to
        }
        else
        {
            UiController.instance.UpdateInputs(); //on open, updates sliders and toggles to be what they were set to before
        }

        Time.timeScale = !UiController.instance.pauseScreen.activeSelf ? 0 : 1; //Freezes game if the pause screen is active
        UiController.instance.pauseScreen.SetActive(!UiController.instance.pauseScreen.activeSelf); //sets the pause screens activity to be opposite of what it already is (if active then disable, if not active, disable)
        PausePlayer(!UiController.instance.pauseScreen.activeSelf); //pauses/unpauses player based on if the pause screen is displayed or hidden.
    }

    public void PausePlayer(bool active)
    {
        Cursor.lockState = active ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !active;
        GetComponent<PlayerMove>().enabled = active;
        GetComponent<MouseLook>().enabled = active;
        if(GetComponentInChildren<PlayerGun>()) GetComponentInChildren<PlayerGun>().Pause(!active);
        if(GetComponentInChildren<Animator>()) GetComponentInChildren<Animator>().enabled = active;
        GetComponent<PlayerHealth>().enabled = active;
        GetComponent<PlayerWeaponManager>().enabled = active;
        UpdateAudioSourceSpeed();
    }

    void UpdateAudioSourceSpeed()
    {
        AudioSource[] allAudio = FindObjectsOfType<AudioSource>();

        for(int i = 0; i < allAudio.Length; i++)
        {
            allAudio[i].pitch = Time.timeScale;
        }
    }

    void UpdateOptions()
    {
        mouseLook.sensitivityX = sensX;
        mouseLook.sensitivityY = sensY;
        fOVController.FOV = FOV;
        // fOVController.dynamicFOV = dynamicFOV;
    }

    void InitOptions()
    {
        sensX = mouseLook.sensitivityX;
        sensY = mouseLook.sensitivityY;
        FOV = fOVController.FOV;
        // dynamicFOV = fOVController.dynamicFOV;
    }

    public void UpdateFOV(float FOV)
    {
        fOVController.FOV = FOV * 10f;
    }

    #endregion
}
