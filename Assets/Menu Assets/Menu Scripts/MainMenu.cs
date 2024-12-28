using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{
    public Button playButton;
    public Button startButton;
    public Button easyButton;
    public Button hardButton;
    public Button backButton;
    public GameObject difficultyPanel;
    public GameObject mainPanel;
    public GameObject weaponSelectionPanel;
    public string gameSceneName = "Game";

    void Start()
    {
        difficultyPanel.SetActive(false);
        weaponSelectionPanel.SetActive(false);
        playButton.onClick.AddListener(ShowDifficultyOptions);
        string difficulty = PlayerPrefs.GetString("SelectedDifficulty", "Easy");
        if (difficulty == "Easy")
        {
            // Set easy mode parameters
        }
        else if (difficulty == "Hard")
        {
            // Set hard mode parameters
        }
    }

    void ShowDifficultyOptions()
    {
        mainPanel.SetActive(false);
        difficultyPanel.SetActive(true);
    }

    void StartGameWithDifficulty(string difficulty)
    {
        PlayerPrefs.SetString("SelectedDifficulty", difficulty);
        startButton.onClick.AddListener(ShowWeaponSelection);
    }

    public void ShowWeaponSelection()
    {
        difficultyPanel.SetActive(false);
        weaponSelectionPanel.SetActive(true);
    }

    public void SelectWeapon(string weapon)
    {
        PlayerPrefs.SetString("SelectedWeapon", weapon);
        StartGame();
    }

    public void StartGame()
    { 
        string gameSceneName = "Game";
        SceneManager.LoadScene(gameSceneName);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}