using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] int _playerScore;

    void Awake()
    {
        instance = this;
    }

    public void PlayerScore(int pointsReceived)
    {
        _playerScore += pointsReceived;
        UiController.instance.UpdateScoreText(_playerScore);
    }

    public void SpendPoints(int pointsLost)
    {
        if(_playerScore >= pointsLost)
        {
            _playerScore -= pointsLost;
            UiController.instance.UpdateScoreText(_playerScore);
        }
        else
        {
            Debug.LogWarning("Player does not have enough points to spend on this purchase.");
        }
    }
}
