using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject playerWonScreen;
    public GameObject playerLostScreen;
    bool gameOver;

    void Start()
    {
        Guard.OnPlayerSpotted += OnGameLost;
        FindObjectOfType<Player>().OnPlayerWin += OnGameWon;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    void OnGameLost()
    {
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
