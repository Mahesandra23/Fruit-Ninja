using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Import TextMeshPro namespace

public class MainMenuManager : MonoBehaviour
{
    public Button muteButton;
    public TMP_Text buttonText;

    void Start()
    {
        UpdateButtonText();
        muteButton.onClick.AddListener(ToggleSFX);
    }

    void ToggleSFX()
    {
        AudioManager.instance.ToggleMuteSFX();
        UpdateButtonText();
    }

    void UpdateButtonText()
    {
        if (AudioManager.instance.GetMuteState())
        {
            buttonText.text = "SFX = off";
        }
        else
        {
            buttonText.text = "SFX = on";
        }
    }
}

