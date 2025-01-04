using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private bool isMuted = false;
    public AudioSource[] sfxAudioSources;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ToggleMuteSFX()
    {
        isMuted = !isMuted;

        foreach (AudioSource audioSource in sfxAudioSources)
        {
            audioSource.mute = isMuted;
        }

        Debug.Log("SFX Muted: " + isMuted);
    }

    public bool GetMuteState()
    {
        return isMuted;
    }
}

