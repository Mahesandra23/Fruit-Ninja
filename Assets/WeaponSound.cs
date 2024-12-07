using UnityEngine;

public class WeaponSound : MonoBehaviour
{
    public AudioSource audioSource; // Referensi ke AudioSource
    public AudioClip shootSound;   // Suara tembakan

    void Start()
    {
        // Pastikan AudioSource sudah diatur
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void Shoot()
    {
        // Mainkan suara tembakan
        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }
}