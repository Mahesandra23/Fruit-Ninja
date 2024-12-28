using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour {

    public static Action shootInput;
    public static Action reloadInput;

    [SerializeField] private KeyCode reloadKey = KeyCode.R;
    [SerializeField] private AudioClip shootSound; // Tambahkan variabel untuk klip suara.
    [SerializeField] private AudioClip reloadSound; // Tambahkan variabel untuk klip suara reload.

    private AudioSource audioSource; // Komponen AudioSource.

    private void Awake()
    {
        // Pastikan komponen AudioSource tersedia.
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            PlayShootSound();
            shootInput?.Invoke();
        }

        if (Input.GetKeyDown(reloadKey))
        {
            PlayReloadSound();
            reloadInput?.Invoke();
        }
    }

    private void PlayShootSound()
    {
        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound); // Memainkan suara tembakan.
        }
    }

    private void PlayReloadSound()
    {
        if (reloadSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(reloadSound); // Memainkan suara reload.
        }
    }
}