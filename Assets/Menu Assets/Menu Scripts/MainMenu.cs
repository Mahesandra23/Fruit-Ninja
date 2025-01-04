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
        PlayerPrefs.SetInt("TotalMoney",100000000);
        PlayerPrefs.Save();
        // Hide the difficulty and weapon selection panels initially
        difficultyPanel.SetActive(false);
        weaponSelectionPanel.SetActive(false);

        // Set listeners for the buttons
        playButton.onClick.AddListener(ShowDifficultyOptions);
        easyButton.onClick.AddListener(() => StartGameWithDifficulty("Easy"));
        hardButton.onClick.AddListener(() => StartGameWithDifficulty("Hard"));
        backButton.onClick.AddListener(BackToMainMenu);

        // Load previously selected difficulty (if any)
        string difficulty = PlayerPrefs.GetString("SelectedDifficulty", "Easy");
        if (difficulty == "Easy")
        {
            PlayerPrefs.SetString("SelectedDifficulty", "Easy");
        }
        else if (difficulty == "Hard")
        {
            PlayerPrefs.SetString("SelectedDifficulty", "Hard");
        }
        PlayerPrefs.Save(); // Save the value immediately
    }

    void ShowDifficultyOptions()
    {
        // Hide main menu, show difficulty selection
        mainPanel.SetActive(false);
        difficultyPanel.SetActive(true);
    }

    void StartGameWithDifficulty(string difficulty)
    {
        // Save the selected difficulty
        PlayerPrefs.SetString("SelectedDifficulty", difficulty);
        PlayerPrefs.Save();  // Save the value immediately
        ShowWeaponSelection();
    }

    public void ShowWeaponSelection()
    {
        // Hide difficulty panel, show weapon selection
        difficultyPanel.SetActive(false);
        weaponSelectionPanel.SetActive(true);
    }

    public void SelectWeapon(string weapon)
    {
        // Save the selected weapon
        PlayerPrefs.SetString("SelectedWeapon", weapon);
        StartGame();
    }

    public void StartGame()
    {
        // Load the game scene
        SceneManager.LoadScene(gameSceneName);
    }

    public void LoadMainMenu()
    {
        // Load the main menu scene
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        // Quit the application
        Application.Quit();
    }

    void BackToMainMenu()
    {
        // Switch back to the main menu panel if needed
        difficultyPanel.SetActive(false);
        weaponSelectionPanel.SetActive(false);
        mainPanel.SetActive(true);
    }
}