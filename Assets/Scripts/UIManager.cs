using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject playerWonScreen;
    public GameObject playerLostScreen;
    bool gameOver;
    bool playerLost;
    int totalLevels;
    static int currentLevelIndex;


    void Start()
    {
        Guard.OnPlayerSpotted += OnGameLost;
        FindObjectOfType<Player>().OnPlayerWin += OnGameWon;
        totalLevels = SceneManager.sceneCountInBuildSettings;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (playerLost)
                {
                    SceneManager.LoadScene(currentLevelIndex);
                }
                else
                {
                    currentLevelIndex++;
                    currentLevelIndex = currentLevelIndex % totalLevels;
                    SceneManager.LoadScene(currentLevelIndex);
                }
            }
        }
    }

    void OnGameLost()
    {
        playerLost = true;
        OnGameOver(playerLostScreen);
    }
    void OnGameWon()
    {
        OnGameOver(playerWonScreen);
    }
    void OnGameOver(GameObject uiScreen)
    {
        uiScreen.SetActive(true);
        gameOver = true;
        Guard.OnPlayerSpotted -= OnGameLost;
        FindObjectOfType<Player>().OnPlayerWin -= OnGameWon;

    }
}
