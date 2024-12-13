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

    private bool isReloading = false;
    private bool canFire = true; // Control firing cooldown
    private int currentAmmo;
    private Coroutine firingCoroutine;

    void Start()
    {
        if (animator == null)
        {
            animator = rectangle.GetComponent<Animator>(); // Get the Animator component if not set in Inspector
        }
        currentAmmo = maxAmmo; // Initialize ammo
        UpdateAmmoDisplay(); 
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
    if (Input.GetMouseButtonDown(0) && !isReloading && currentAmmo > 0)
    {
        animator.SetBool("fire", true);
        animator.SetBool("idle", false);

        // Start firing coroutine
        if (firingCoroutine == null)
        {
            firingCoroutine = StartCoroutine(FireContinuously());
        }
    }

    if (Input.GetMouseButtonUp(0) && firingCoroutine != null)
    {
        // Stop firing coroutine
        StopCoroutine(firingCoroutine);
        firingCoroutine = null;

        // Set animator to idle
        animator.SetBool("fire", false);
        animator.SetBool("idle", true);
    }

    if (Input.GetKeyDown(KeyCode.R) && !isReloading)
    {
        StartCoroutine(Reload());
    }
}
IEnumerator FireContinuously()
{
    while (currentAmmo > 0 && !isReloading)
    {
        if (canFire)
        {
            canFire = false; // Prevent firing until cooldown is complete

            // Decrease ammo and update UI
            currentAmmo--;
            UpdateAmmoDisplay();

            // Fire the bullet and trail
            FireCubeWithTrail();

            yield return new WaitForSeconds(fireCooldown); // Wait for cooldown
            canFire = true; // Allow firing again
        }
        else
        {
            yield return null; // Wait until firing is allowed
        }
    }

    // Stop firing if out of ammo
    if (currentAmmo <= 0)
    {
        StartCoroutine(Reload());
    }
}

void FireCubeWithTrail()
{
    // Instantiate the tiny cube at the fire point
    GameObject tinyCube = Instantiate(tinyCubePrefab, firePoint.position, firePoint.rotation);
    Rigidbody rb = tinyCube.AddComponent<Rigidbody>();
    rb.useGravity = false;
    rb.velocity = firePoint.forward * cubeSpeed;

    // Create and configure the trail
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

    // Destroy the cube and trail after their lifetime
    Destroy(tinyCube, trailLifetime);
}

IEnumerator Reload()
{
    isReloading = true; // Prevent movement and firing during reload

    // Trigger the reload animation
    animator.SetBool("reload", true);
    animator.SetBool("idle", false);

    Debug.Log("Reloading...");
    yield return new WaitForSeconds(reloadTime);

    // Reload logic
    currentAmmo = maxAmmo;

    // Reset animation parameters
    animator.SetBool("reload", false);
    animator.SetBool("idle", true);

    isReloading = false; // Re-enable movement and firing
    Debug.Log("Reload complete!");
    UpdateAmmoDisplay(); // Update UI counter after reload
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
