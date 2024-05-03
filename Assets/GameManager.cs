//////////////////////////////////////////////
//Assignment/Lab/Project: Zombie Survival
//Name: Joe Morris
//Section: SGD.213.0021
//Instructor: Professor Locklear
//Date: 05/02/2024
/////////////////////////////////////////////

using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Controls when player receives and loses points, as well as the powerups the player can pick up. Also tells the Ui Controller to reflect changes to points and powerups.
    public static GameManager instance;
    [SerializeField] int _playerScore;
    [SerializeField] float powerUpDuration = 30;
    public bool instaKillActive;
    bool doublePointsActive;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UiController.instance.UpdateScoreText(_playerScore);
    }

    public void PlayerScore(int pointsReceived)
    {
        _playerScore += doublePointsActive ? 2 * pointsReceived : pointsReceived;
        UiController.instance.UpdateScoreText(_playerScore);
    }

    public bool SpendPoints(int pointsLost)
    {
        if(_playerScore >= pointsLost)
        {
            _playerScore -= pointsLost;
            UiController.instance.UpdateScoreText(_playerScore);
            return true;
        }
        else
        {
            Debug.LogWarning("Player does not have enough points to spend on this purchase.");
            return false;
        }
    }

    public void PickUpPowerUp(string powerUp)
    {
        string powerUpName = "";

        switch(powerUp)
        {
            case "instaKill":
            powerUpName = "Insta-Kill";
            instaKillActive = true;
            Invoke("DisableInstaKill", powerUpDuration);

            UiController.instance.ShowInstaKillGraphic(powerUpDuration);
            break;

            case "doublePoints":
            powerUpName = "Double Points";
            doublePointsActive = false;
            Invoke("DisableDoublePoints", powerUpDuration);

            UiController.instance.ShowDoublePointsGraphic(powerUpDuration);
            break;

            case "maxAmmo":
            powerUpName = "Max Ammo";
            PlayerWeaponManager.instance.RefillAllAmmo();
            break;
        }

        UiController.instance.ShowPowerUpText(powerUpName);
    }

    void DisableInstaKill()
    {
        instaKillActive = false;
    }

    void DisableDoublePoints()
    {
        doublePointsActive = false;
    }
}
