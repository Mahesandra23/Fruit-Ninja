using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RectangleFollowCursor : MonoBehaviour
{
    [Header("Gun Settings")]
    [SerializeField] private Transform rectangle;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Vector3 offset = new Vector3(-1, 1, 5);

    [Header("Firing Settings")]
    [SerializeField] private GameObject tinyCubePrefab;
    [SerializeField] private GameObject trailPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float cubeSpeed = 10f;
    [SerializeField] private float trailLifetime = 0.5f;
    [SerializeField] private float trailWidth = 0.1f;
    [SerializeField] private float fireCooldown = 0.5f; // Time between shots

    [Header("Ammo Settings")]
    [SerializeField] private int maxAmmo = 6;
    [SerializeField] private float reloadTime = 2f;

    [Header("UI Settings")]
    [SerializeField] private Text ammoDisplay;

    [Header("Recoil Settings")]
    [SerializeField] private Animator animator; // Animator reference

    [Header("Audio Settings")]
    [SerializeField] private AudioClip fireSound;
    [SerializeField] private AudioClip reloadSound;
    private AudioSource audioSource;

    private bool isReloading = false;
    private bool canFire = true; // Control firing cooldown
    private int currentAmmo;

    void Start()
    {
        if (animator == null)
        {
            animator = rectangle.GetComponent<Animator>(); // Get the Animator component if not set in Inspector
        }
        currentAmmo = maxAmmo; // Initialize ammo
        UpdateAmmoDisplay();

        // Inisialisasi AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        // Always calculate and apply rotation
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = mainCamera.nearClipPlane + offset.z;
        Vector3 worldMousePosition = mainCamera.ScreenToWorldPoint(mousePosition);

        Vector3 directionToMouse = (worldMousePosition - rectangle.position).normalized;
        rectangle.rotation = Quaternion.LookRotation(directionToMouse);

        // Handle firing and reloading logic
        if (Input.GetMouseButtonDown(0) && !isReloading && currentAmmo > 0 && canFire)
        {
            StartCoroutine(Fire());
            UpdateAmmoDisplay();
        }

        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            StartCoroutine(Reload());
            UpdateAmmoDisplay();
        }
    }

    IEnumerator Fire()
    {
        canFire = false; // Prevent firing until cooldown is complete
        currentAmmo--;

        // Trigger the recoil animation
        animator.SetBool("fire", true);
        animator.SetBool("idle", false);

        // Memutar suara tembakan
        if (fireSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(fireSound);
        }

        // Fire the tiny cube
        FireCubeWithTrail();

        yield return new WaitForSeconds(fireCooldown); // Wait for the cooldown duration

        animator.SetBool("fire", false); // Transition back to idle
        animator.SetBool("idle", true);

        canFire = true; // Allow firing again

        if (currentAmmo == 0)
        {
            canFire = false;
            StartCoroutine(Reload());
            canFire = true;
        }
    }

    void FireCubeWithTrail()
    {
        GameObject tinyCube = Instantiate(tinyCubePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = tinyCube.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.velocity = firePoint.forward * cubeSpeed;

        GameObject firedTrail = Instantiate(trailPrefab, tinyCube.transform);
        firedTrail.transform.localPosition = Vector3.zero;

        TrailRenderer trailRenderer = firedTrail.GetComponent<TrailRenderer>();
        if (trailRenderer != null)
        {
            trailRenderer.time = trailLifetime;
            trailRenderer.startWidth = trailWidth;
            trailRenderer.endWidth = trailWidth;
            trailRenderer.minVertexDistance = 0.05f;
        }

        Destroy(tinyCube, trailLifetime);
    }

    IEnumerator Reload()
    {
        isReloading = true; // Prevent movement and firing during reload

        // Trigger the reload animation
        animator.SetBool("reload", true);
        animator.SetBool("idle", false);

        // Memutar suara reload
        if (reloadSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }

        Debug.Log("Reloading...");
        yield return new WaitForSeconds(reloadTime);

        // Reload logic
        currentAmmo = maxAmmo;

        // Reset animation parameters
        animator.SetBool("reload", false);
        animator.SetBool("idle", true);

        isReloading = false; // Re-enable movement and firing
        Debug.Log("Reload complete!");
        UpdateAmmoDisplay();
    }

    void UpdateAmmoDisplay()
    {
        if (ammoDisplay != null)
        {
            ammoDisplay.text = $"Ammo: {currentAmmo}/{maxAmmo}";
        }
    }

    public void SetReloadRotation()
    {
        rectangle.rotation = Quaternion.Euler(90f, rectangle.rotation.eulerAngles.y, rectangle.rotation.eulerAngles.z);
    }
}