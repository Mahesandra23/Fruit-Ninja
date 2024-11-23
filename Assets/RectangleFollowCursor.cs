using System.Collections;
using UnityEngine;

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

    [Header("Recoil Settings")]
    [Header("Ammo Settings")]
    [SerializeField] private int maxAmmo = 6;
    [SerializeField] private float reloadTime = 2f;

    private Animator animator; // Animator reference
    private bool isReloading = false;
    private int currentAmmo;

    void Start()
    {
        animator = rectangle.GetComponent<Animator>(); // Get the Animator component
        currentAmmo = maxAmmo; // Initialize ammo
    }

    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = mainCamera.nearClipPlane + offset.z;
        Vector3 worldMousePosition = mainCamera.ScreenToWorldPoint(mousePosition);

        Vector3 directionToMouse = (worldMousePosition - rectangle.position).normalized;
        rectangle.rotation = Quaternion.LookRotation(directionToMouse);

        if (Input.GetMouseButtonDown(0) && !isReloading && currentAmmo > 0)
        {
            Fire();
        }

         if (Input.GetKeyDown(KeyCode.R) && !isReloading)
    {
        StartCoroutine(Reload());
    }
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
    isReloading = true;

    // Trigger the reload animation
    animator.SetTrigger("Reload");

    Debug.Log("Reloading...");
    yield return new WaitForSeconds(reloadTime); // Wait for reload animation to finish

    currentAmmo = maxAmmo;
    isReloading = false;
    Debug.Log("Reload complete!");
}
}
