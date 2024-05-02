using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{
    //Script that controls the Ui.
    public static UiController instance; //public static instance for other scripts to access


    [Header("Pause Screen References")]
    public GameObject pauseScreen; // panel/screen GO used for the pause screen.
    [SerializeField] Slider Xslider; //Xslider and Yslider control the X and Y sensitivity of the Mouse Look (Camera Control)
    [SerializeField] Slider Yslider;
    [SerializeField] Slider FOVslider; //Slider that adjusts the Field of View of the camera.
    // public Toggle DynamicFovToggle; //Dynamic FOV was a scrapped feature from UP_JM_FPSController. All references to it in this project are commented out.
    [SerializeField] TextMeshProUGUI sensitivityXText; //sensitivityXText and sensitivityYText display the value of the X and Y mouse sensitivity.
    [SerializeField] TextMeshProUGUI sensitivityYText;
    [SerializeField] TextMeshProUGUI fovText; //displays the current Field of View of the camera.
    [SerializeField] GameObject mainMenuButtonGameObject;

    [Header("Scene References")]
    [SerializeField] string mainMenuSceneName; //name of main menu scene (if present)

    [Header("InGame HUD References")]
    [SerializeField] Image crosshairImage;
    [SerializeField] Gradient healthColor; //gradient used for the healthbar's color
    [SerializeField] Image healthBarFill; //Fill image used for the healthbar slider
    [SerializeField] Slider healthBar; //slider used to display the value of player's health
    [SerializeField] TextMeshProUGUI healthText; //text that displays what the slider is used for and the value of the player's health
    [SerializeField] GameObject hitmarkerPrefab; //object that spawns when the player hits an enemy/object with a healthbar. Feedback that the player hit something.
    [SerializeField] RectTransform[] crosshairLines; //stores each crosshair line's transform
    [SerializeField] TextMeshProUGUI interactText;
    [SerializeField] TextMeshProUGUI reloadText;
    [SerializeField] TextMeshProUGUI roundText;
    [SerializeField] TextMeshProUGUI zombiesLeftText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI powerUpText;
    [SerializeField] GameObject instaKillGraphic;
    [SerializeField] GameObject doublePointsGraphic;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if(mainMenuSceneName == "") //Disables the Main Menu button if there is no reference to the scene name.
        {
            mainMenuButtonGameObject.SetActive(false);
        }

        InitHealth();
    }

    //pause menu functions were already apart of UP_JM_FPSController and can be ignored if you like.
    #region pause menu functions
    public void UpdateInputs()
    {
        // DynamicFovToggle.isOn = PlayerManager.instance.dynamicFOV; //Dynamic FOV is disabled because it was scrapped.
        Xslider.value = PlayerManager.instance.sensX;
        Yslider.value = PlayerManager.instance.sensY;
        FOVslider.value = PlayerManager.instance.FOV /10;
    }

    // public void ChangeDynamicFOV(bool active)
    // {
    //     PlayerManager.instance.dynamicFOV = active;
    // }

    //"Change" functions below are called by sliders.
    public void ChangeFOV(float FOV)
    {
        PlayerManager.instance.FOV = FOV *10;
        fovText.text = "FOV: " + (FOV * 10);
        PlayerManager.instance.UpdateFOV(FOV);
    }

    public void ChangeSensX(float sens)
    {
        PlayerManager.instance.sensX = sens;
        sensitivityXText.text = "Mouse Sens X: " + sens;
    }

    public void ChangeSensY(float sens)
    {
        PlayerManager.instance.sensY = sens;
        sensitivityYText.text = "Mouse Sens Y: " + sens;
    }

    public void OnClickResume()
    {
        PlayerManager.instance.PauseGame();
    }

    #endregion

    #region game functions

    public void ChangeCrosshairVisibility(bool visible)
    {
        crosshairImage.gameObject.SetActive(visible);
    }
    public void ChangeCrosshairSprite(Sprite crosshair, float spread)
    {
        // crosshairImage.sprite = crosshair;
        //spread is used to figure out how big the crosshair needs to be
        // spread += 1;
        // spread /= 3f;
        // crosshairImage.rectTransform.sizeDelta = new Vector2(50 * spread * 2, 50 * spread * 2);

    }

    public void ChangeCrosshairSize(float spread)
    {
        spread *= 60f/Camera.main.fieldOfView;
        for(int i = 0; i < crosshairLines.Length; i++)
        {
            float width = crosshairLines[i].sizeDelta.x /2f;
            float height = crosshairLines[i].sizeDelta.y /2f;

            if((i+1) % 2 == 0) spread = -spread;

            bool isHorizontal = width > height;

            crosshairLines[i].localPosition = isHorizontal ? new Vector2(width* spread, 0) : new Vector2(0, height * spread);
        }
    }

    public void InitHealth()
    {
        healthBar.maxValue = PlayerHealth.instance.MaxHealth;
        healthBar.value = healthBar.maxValue;
        healthBarFill.color = healthColor.Evaluate(healthBar.normalizedValue);
        healthText.text = "Health: " + healthBar.value + "%";

    }
    public void UpdateHealth() //Gets the health from the PlayerHealth script, and displays it in the respective Text and Slider elements.
    {
        float playerHealth = PlayerHealth.instance.Health;
        healthText.text = "Health: " + playerHealth + "%";
        healthBar.value = playerHealth;
        healthBarFill.color = healthColor.Evaluate(healthBar.normalizedValue);
    }

    public void SpawnHitmarker()
    {
        Instantiate(hitmarkerPrefab, transform);
    }

    public void UpdateInteractText(string text)
    {
        interactText.text = text;
    }

    public void DisplayReloadText(bool visible)
    {
        reloadText.text = visible ? "Press R to Reload" : "";
    }

    public void UpdateRoundText(int round, Color color)
    {
        if(color == null) color = Color.red; //cannot input a default value of Color.red in the parameters because it must be a compile time constant, which Color.red is not.

        roundText.color = color;
        roundText.text = "Round " + round;
    }

    public void UpdateZombiesLeftText(int zombiesLeft)
    {
        zombiesLeftText.text = "Zombies Left: " + zombiesLeft;
    }

    public void UpdateScoreText(int score)
    {
        scoreText.text = "Score: " + score;
    }

    public void ShowPowerUpText(string powerUpName)
    {
        CancelInvoke("HidePowerUpText");

        string message = powerUpName + " Picked Up!";

        powerUpText.text = message;

        Invoke("HidePowerUpText", 2);
    }

    void HidePowerUpText()
    {
        powerUpText.text = "";
    }

    public void ShowInstaKillGraphic(float powerUpDuration)
    {
        CancelInvoke("HideInstaKillGraphic");

        instaKillGraphic.SetActive(true);

        Invoke("HideInstaKillGraphic", powerUpDuration);
    }
    
    void HideInstaKillGraphic()
    {
        instaKillGraphic.SetActive(false);
    }

    public void ShowDoublePointsGraphic(float powerUpDuration)
    {
        CancelInvoke("HideDoublePointsGraphic");

        doublePointsGraphic.SetActive(true);

        Invoke("HideDoublePointsGraphic", powerUpDuration);
    }

    void HideDoublePointsGraphic()
    {
        doublePointsGraphic.SetActive(false);
    }

    #endregion
}
