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

    void Start()
    {
        UpdateCurrencyFromGameManager(); // Sync the store's currency with GameManager
        UpdateUI();
    }

    public void SelectWeapon(int weaponIndex)
    {
        if (IsWeaponUnlocked(weaponIndex))
        {
            PlayerPrefs.SetString("SelectedWeapon", "Weapon" + weaponIndex);
            Debug.Log("Weapon Selected: Weapon" + weaponIndex);
        }
        else if (GameManager.instance.GetMoney() >= weaponCosts[weaponIndex])
        {
            UnlockWeapon(weaponIndex);
        }
        else
        {
            Debug.Log("Not enough currency to unlock this weapon.");
        }
    }

    void UnlockWeapon(int weaponIndex)
    {
        GameManager.instance.AddMoney(-weaponCosts[weaponIndex]); // Deduct money directly from GameManager
        PlayerPrefs.SetInt("WeaponUnlocked" + weaponIndex, 1); // Mark weapon as unlocked
        Debug.Log("Weapon unlocked: Weapon" + weaponIndex);
        UpdateCurrencyFromGameManager();
        UpdateUI();
    }

    bool IsWeaponUnlocked(int weaponIndex)
    {
        return PlayerPrefs.GetInt("WeaponUnlocked" + weaponIndex, 0) == 1; // Default is locked
    }

    void UpdateUI()
    {
        int currentMoney = GameManager.instance.GetMoney(); // Get current money from GameManager
        currencyText.text = currentMoney + "$"; // Update the displayed amount of money

        for (int i = 0; i < weaponButtons.Length; i++)
        {
            bool unlocked = IsWeaponUnlocked(i);
            weaponButtons[i].interactable = unlocked || currentMoney >= weaponCosts[i];
            // Optional: Update button visuals (e.g., disable or add lock icon)
        }
    }

    void UpdateCurrencyFromGameManager()
    {
        // Sync currency with the GameManager's totalMoney
        if (GameManager.instance != null)
        {
            currencyText.text = GameManager.instance.GetMoney() + "$";
            Debug.Log("Current currency: " + GameManager.instance.GetMoney());
        }
    }
}