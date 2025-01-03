using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponSelectionMenu2 : MonoBehaviour
{
    public TMP_Text currencyText; // Text to display the current currency
    public Button[] weaponButtons; // Assign buttons for each weapon in the Inspector
    public int[] weaponCosts; // Costs of each weapon (must match weaponButtons length)
    public int money;

    void Start()
    {
        LoadMoney(); // Load the saved money value
        UpdateUI();
    }

    private void LoadMoney()
    {
        money = PlayerPrefs.GetInt("TotalMoney", 0); // Load money or default to 0
        Debug.Log("Money loaded: " + money); // Check if money is loaded correctly
    }

    private void SaveMoney()
    {
        PlayerPrefs.SetInt("TotalMoney", money); // Save the money to PlayerPrefs
        PlayerPrefs.Save(); // Make sure to save changes to PlayerPrefs
        Debug.Log("Money saved: " + money); // Check if money is saved correctly
    }

    public void AddMoney(int amount)
    {
        money += amount;
        SaveMoney(); // Save money whenever it changes
        UpdateUI();
    }

    public void SelectWeapon(int weaponIndex)
    {
        if (IsWeaponUnlocked(weaponIndex))
        {
            PlayerPrefs.SetString("SelectedWeapon", "Weapon" + weaponIndex);
            Debug.Log("Weapon Selected: Weapon" + weaponIndex);
        }
        else if (money >= weaponCosts[weaponIndex])
        {
            UnlockWeapon(weaponIndex);
            PlayerPrefs.SetInt("TotalMoney",money - weaponCosts[weaponIndex]);
            money -= weaponCosts[weaponIndex];
            PlayerPrefs.Save();
        }
        else
        {
            Debug.Log("Not enough currency to unlock this weapon.");
        }
    }

    void UnlockWeapon(int weaponIndex)
    {
        money -= weaponCosts[weaponIndex]; // Deduct money
        SaveMoney(); // Save updated money
        PlayerPrefs.SetInt("WeaponUnlocked" + weaponIndex, 1); // Mark weapon as unlocked
        Debug.Log("Weapon unlocked: Weapon" + weaponIndex);
        UpdateUI();
    }

    bool IsWeaponUnlocked(int weaponIndex)
    {
        return PlayerPrefs.GetInt("WeaponUnlocked" + weaponIndex, 0) == 1; // Default is locked
    }

    void UpdateUI()
    {
        currencyText.text = money + "$"; // Update the displayed amount of money

        for (int i = 0; i < weaponButtons.Length; i++)
        {
            bool unlocked = IsWeaponUnlocked(i);
            weaponButtons[i].interactable = unlocked || money >= weaponCosts[i];
            // Optional: Update button visuals (e.g., disable or add lock icon)
        }
    }
}
