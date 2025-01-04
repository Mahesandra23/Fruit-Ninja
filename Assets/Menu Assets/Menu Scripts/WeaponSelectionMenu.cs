using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class WeaponSelectionMenu : MonoBehaviour
{
    public GameObject startButton; // Assign the Start button in the Inspector
    public List<GameObject> weapons; // List of weapon GameObjects
    private string selectedWeapon;

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

        DeactivateAllWeapons(); // Pastikan semua senjata dinonaktifkan saat awal
    }

    public void SelectWeapon(string weaponName)
    {
        // Simpan senjata yang dipilih
        PlayerPrefs.SetString("SelectedWeapon", weaponName);
        Debug.Log("Weapon Selected: " + weaponName);

        // Aktifkan senjata yang sesuai
        ActivateWeapon(weaponName);

        // Tampilkan tombol Start
        ShowStartButton();
    }

    private void ActivateWeapon(string weaponName)
    {
        Debug.Log("Attempting to activate weapon: " + weaponName);
        // Nonaktifkan semua senjata
        foreach (var weapon in weapons)
        {
            Debug.Log("Disabling weapon: " + weapon.name);
            weapon.SetActive(false);
        }

        // Aktifkan senjata yang dipilih
        GameObject selectedWeapon = weapons.FirstOrDefault(w => w.name == weaponName);
        if (selectedWeapon != null)
        {
            Debug.Log("Activating weapon: " + selectedWeapon.name);
            selectedWeapon.SetActive(true);  // Aktifkan senjata yang dipilih
        }
        else
        {
            Debug.LogWarning("Weapon not found: " + weaponName);
        }
    }



    private void DeactivateAllWeapons()
    {
        foreach (GameObject weapon in weapons)
        {
            weapon.SetActive(false); // Nonaktifkan semua senjata
        }
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
