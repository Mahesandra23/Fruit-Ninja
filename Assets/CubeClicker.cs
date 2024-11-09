using UnityEngine;
using System.Collections;

public class CubeClicker : MonoBehaviour
{
    private Renderer fruitRenderer;
    public Camera mainCamera;
    public float jumpHeight = 30f;
    public float jumpSpeed = 2f;
    public float moveSpeed = 1f;
    public float distanceFromCamera = 6f;
    public float disappearThreshold = -8f;
    public float spawnHeight = -30f;

    public GameObject fruitExplosionPrefab;
    public int maxClicks = 2; // Menentukan jumlah klik untuk meledakkan buah
    public float colorChangeDuration = 0.2f; // Durasi warna berubah menjadi merah
    public float smallJumpHeight = 5f; // Tinggi lompatan kecil saat diklik

    private Vector3 startPosition;
    private bool isJumping = false;
    private float jumpTimer = 0f;
    private float moveTimer = 0f;
    private float fruitWidth;
    private int jumpDirection;
    private float minX, maxX;

    private int clickCount = 0; // Menyimpan jumlah klik saat ini
    private Color originalColor; // Menyimpan warna asli buah

    void Start()
    {
        fruitRenderer = GetComponent<Renderer>();

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        minX = -4;
        maxX = 5;

        originalColor = fruitRenderer.material.color; // Simpan warna asli

        RepositionFruit();
        StartJumping();
    }

    void Update()
    {
        if (isJumping)
        {
            AnimateJump();
        }
    }

    void OnMouseDown()
    {
        clickCount++;

        // Mulai coroutine untuk mengganti warna menjadi merah sementara
        StartCoroutine(ChangeColorTemporary());

        // Lompat sedikit ke atas dengan mengubah posisi tinggi asli
        ApplySmallJump();

        if (clickCount >= maxClicks)
        {
            GameObject explosion = Instantiate(fruitExplosionPrefab, transform.position, transform.rotation);
            Destroy(explosion, 2f);

            clickCount = 0;
            RepositionFruit();
            StartJumping();
        }
    }

    // Coroutine untuk mengganti warna menjadi merah sementara
    IEnumerator ChangeColorTemporary()
    {
        fruitRenderer.material.color = Color.red; // Ganti warna menjadi merah
        yield return new WaitForSeconds(colorChangeDuration); // Tunggu sebentar
        fruitRenderer.material.color = originalColor; // Kembalikan warna asli
    }

    // Fungsi untuk mengubah posisi tinggi asli saat klik
    void ApplySmallJump()
    {
        startPosition.y += smallJumpHeight; // Menambah tinggi asli buah
    }

    void StartJumping()
    {
        isJumping = true;
        jumpDirection = startPosition.x < 0 ? 1 : -1;
    }

    void AnimateJump()
    {
        jumpTimer += Time.deltaTime * jumpSpeed;
        float jumpOffset = Mathf.Sin(jumpTimer) * jumpHeight;

        moveTimer += Time.deltaTime * moveSpeed;
        float moveOffset = Mathf.Sin(moveTimer) * (fruitWidth * 0.5f) * jumpDirection;

        float newX = Mathf.Clamp(startPosition.x + moveOffset, minX, maxX);
        transform.position = new Vector3(newX, startPosition.y + jumpOffset, distanceFromCamera);

        if (jumpOffset < disappearThreshold)
        {
            fruitRenderer.enabled = false;
            RepositionFruit();
            clickCount = 0;
        }
        else
        {
            fruitRenderer.enabled = true;
        }
    }

    void RepositionFruit()
    {
        float randomX = Random.Range(minX, maxX);
        startPosition = new Vector3(randomX, spawnHeight, distanceFromCamera);
        transform.position = startPosition;

        jumpTimer = 0f;
        moveTimer = 0f;
    }
}
