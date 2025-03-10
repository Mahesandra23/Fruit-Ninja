using System.Collections; // Required for IEnumerator and coroutines
using UnityEngine;

public class shooting_hold : MonoBehaviour
{
    public Transform rectangle;
    public Camera mainCamera;
    public Vector3 offset = new Vector3(-1, 1, 5); // Offset for the bottom-left corner in world space

    public GameObject tinyCubePrefab; // Reference to the tiny cube prefab
    public GameObject trailPrefab; // Reference to the "Trail" GameObject
    public Transform firePoint; // The edge of the rectangle where the tiny cube will be fired from
    public float cubeSpeed = 10f; // Speed of the tiny cube
    public float trailLifetime = 0.5f; // Lifetime of the trail in seconds (short trail)
    public float trailWidth = 0.1f; // Width of the trail
    public float recoilAmount = 0.1f; // Amount of recoil (how far the gun moves back)
    public float recoilTime = 0.1f; // How long the recoil lasts before resetting the position
    public float fireRate = 0.2f; // Time in seconds between consecutive shots (fire rate)
    public int maxAmmo = 6; // Max ammo per reload
    public float reloadTime = 2f; // Time it takes to reload

    private Vector3 originalPosition; // To store the original position of the rectangle
    private bool isRecoiling = false; // Flag to track if the gun is recoiling
    private bool isReloading = false; // Flag to track if the gun is reloading
    private float lastFireTime = 0f; // Time of the last fire to control the rate of fire
    private int currentAmmo; // Current ammo count
    private Animator animator;
    void Start()
    {
        originalPosition = rectangle.position; // Store the original position of the rectangle (gun)
        currentAmmo = maxAmmo; // Initialize ammo
    }

    void Update()
    {
        // Make the rectangle follow the mouse and point toward it
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = mainCamera.nearClipPlane + offset.z;
        Vector3 worldMousePosition = mainCamera.ScreenToWorldPoint(mousePosition);

        Vector3 directionToMouse = (worldMousePosition - rectangle.position).normalized;
        rectangle.rotation = Quaternion.LookRotation(directionToMouse);

        // Reload when pressing the R key
        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            StartCoroutine(Reload());
        }

        // Only allow firing if the gun is not recoiling, not reloading, and if the fire rate delay has passed
        if (Input.GetMouseButton(0) && !isRecoiling && !isReloading && Time.time >= lastFireTime + fireRate && currentAmmo > 0)
        {
            FireCubeWithTrail();
            ApplyRecoil();
            lastFireTime = Time.time; // Update the last fire time to limit the fire rate
            currentAmmo--; // Decrease ammo with each shot
        }
    }

    void FireCubeWithTrail()
    {
        // Instantiate the tiny cube at the fire point's position and rotation
        GameObject tinyCube = Instantiate(tinyCubePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = tinyCube.AddComponent<Rigidbody>(); // Add a Rigidbody to make it move
        rb.useGravity = false; 
        rb.velocity = firePoint.forward * cubeSpeed; 

        // Attach the trail to the tiny cube
        GameObject firedTrail = Instantiate(trailPrefab, tinyCube.transform);
        firedTrail.transform.localPosition = Vector3.zero; // Ensure the trail starts at the cube's position

        // Set the trail's lifetime and width to make it short
        TrailRenderer trailRenderer = firedTrail.GetComponent<TrailRenderer>();
        if (trailRenderer != null)
        {
            trailRenderer.time = trailLifetime; // Set the time the trail stays visible (short trail)
            trailRenderer.startWidth = trailWidth; // Set the start width of the trail
            trailRenderer.endWidth = trailWidth; // Set the end width of the trail
            trailRenderer.minVertexDistance = 0.05f; // Minimum distance between trail points (smaller values create a smoother trail)
        }

        // Destroy the tiny cube after the specified lifetime (bullet-like behavior)
        Destroy(tinyCube, trailLifetime);
    }

    void ApplyRecoil()
    {
        // Set the recoiling flag to true, preventing further firing
        isRecoiling = true;

        // Apply recoil by moving the rectangle (gun) slightly back along the fire direction
        rectangle.position -= rectangle.forward * recoilAmount; // Move it back along its own forward vector

        // Reset the position of the rectangle (gun) after recoil time
        StartCoroutine(ResetRecoil());
    }

    IEnumerator ResetRecoil()
    {
        // Wait for the recoil to finish
        yield return new WaitForSeconds(recoilTime);

        // Reset the rectangle position back to its original position
        rectangle.position = originalPosition;

        // Allow firing again after recoil is finished
        isRecoiling = false;
    }

    // Coroutine to handle reloading
    IEnumerator Reload()
    {
        isReloading = true;

        // Optionally trigger a reload animation (if you have one)

        Debug.Log("Reloading...");
        yield return new WaitForSeconds(reloadTime); // Wait for reload animation to finish

        currentAmmo = maxAmmo; // Refill ammo
        isReloading = false; // End the reloading process

        Debug.Log("Reload complete!");
    }
    void Fire()
    {
        currentAmmo--;

        // Trigger the recoil animation

        // Fire the tiny cube
        FireCubeWithTrail();

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
        }
    }
}
