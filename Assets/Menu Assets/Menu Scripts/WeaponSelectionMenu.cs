using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponSelectionMenu : MonoBehaviour
{
    public GameObject startButton; // Assign the Start button in the Inspector

    void Start()
    {
        // Menyembunyikan tombol Start saat awal
        if (startButton != null)
        {
            startButton.SetActive(false); // Hide Start button initially
        }
        else
        {
            Debug.LogError("Start button tidak diatur di Inspector!");
        }
    }

    public void SelectWeapon(string weapon)
    {
        // Menyimpan senjata yang dipilih
        PlayerPrefs.SetString("SelectedWeapon", weapon);
        Debug.Log("Weapon Selected: " + weapon);

        // Tampilkan tombol Start
        ShowStartButton();
    }

    public void ShowStartButton()
    {
        // Pastikan tombol Start tidak null sebelum diaktifkan
        if (startButton != null)
        {
            startButton.SetActive(true); // Enable the Start button
        }
        else
        {
            Debug.LogError("Start button tidak ditemukan!");
        }
    }

    public void StartGame()
    {
        if (GameManager.instance != null)  // Ensure GameManager is available
        {
            GameManager.instance.ResetGameState();
            Debug.Log("StartGame() dipanggil, Resetting Game State.");
        }
        else
        {
            Debug.LogError("GameManager.instance adalah null! Pastikan GameManager ada di scene.");
            return; // Exit early if GameManager is not found
        }

        // Mendapatkan senjata yang dipilih
        string selectedWeapon = PlayerPrefs.GetString("SelectedWeapon", "DefaultWeapon");
        Debug.Log("Starting Game with Weapon: " + selectedWeapon);

        // Cek apakah scene Game tersedia sebelum memuatnya
        Debug.Log("Muat scene: Game");
        if (SceneExists("Game"))
        {
            SceneManager.LoadScene("Game");
        }
        else
        {
            Debug.LogError("Scene 'Game' tidak ditemukan dalam Build Settings!");
        }
    }

    private bool SceneExists(string sceneName)
    {
        // Cek apakah scene tersedia di Build Settings
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneFileName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneFileName == sceneName)
            {
                return true;
            }
        }
        return false;
    }
}