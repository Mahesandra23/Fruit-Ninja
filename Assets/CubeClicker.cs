using UnityEngine;

public class CubeClicker : MonoBehaviour
{
    private Renderer fruitRenderer;
    public Camera mainCamera;  // Reference to the main camera
    public float jumpHeight = 12f;   // How high the fruit jumps above the camera POV
    public float jumpSpeed = 2f;    // Speed of the jump
    public float moveSpeed = 1f;    // Speed of left-right movement
    public float distanceFromCamera = 6f;  // Distance the fruit stays from the camera
    public float disappearThreshold = -2f;  // When to make the fruit disappear (below POV)

    public GameObject fruitExplosionPrefab;  // Prefab buah yang meledak


    private Vector3 startPosition;
    private bool isJumping = false;
    private float jumpTimer = 0f;  // Timer for controlling vertical movement
    private float moveTimer = 0f;  // Timer for controlling horizontal movement
    private float fruitWidth;       // Fruit width
    private float fruitHeight;      // Fruit height
    private int jumpDirection;     // 1 for right, -1 for left

    // Batas layar untuk pergerakan buah
    private float minX, maxX;

    void Start()
    {
        fruitRenderer = GetComponent<Renderer>();

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }


        minX = -4;
        maxX = 5;

        // Mulai di bawah kamera
        RepositionFruit();
        transform.position = startPosition;

        StartJumping();  // Mulai animasi melayang
    }

    void Update()
    {
        if (isJumping)
        {
            AnimateJump();  // Mengontrol animasi melayang
        }
    }

    void OnMouseDown()
{
    // Instantiate efek ledakan dan simpan instance-nya
    GameObject explosion = Instantiate(fruitExplosionPrefab, transform.position, transform.rotation);

    Destroy(explosion, 1f);

}


    void StartJumping()
    {
        jumpTimer = Random.Range(0f, 2f);  // Atur waktu acak untuk melayang
        moveTimer = Random.Range(0f, 2f);  // Atur waktu acak untuk gerakan horizontal
        isJumping = true;

        // Tentukan arah melayang berdasarkan posisi awal
        if (startPosition.x < 0)  // Mulai di kiri
        {
            jumpDirection = 1;  // Gerak ke kanan
        }
        else  // Mulai di kanan
        {
            jumpDirection = -1;  // Gerak ke kiri
        }
    }

    void AnimateJump()
    {
        // Timer untuk melayang
        jumpTimer += Time.deltaTime * jumpSpeed;
        // Gerakan naik-turun dengan sine wave
        float jumpOffset = Mathf.Sin(jumpTimer) * jumpHeight;

        // Timer untuk gerakan horizontal
        moveTimer += Time.deltaTime * moveSpeed;
        // Gerakan kiri-kanan
        float moveOffset = Mathf.Sin(moveTimer) * (fruitWidth * 0.5f) * jumpDirection;

        // Update posisi buah
        float newX = Mathf.Clamp(startPosition.x + moveOffset, minX, maxX);  // Pastikan tidak keluar dari batas layar
        transform.position = new Vector3(newX, startPosition.y + jumpOffset, distanceFromCamera);

        // Jika buah jatuh di bawah threshold, atur ulang posisi
        if (jumpOffset < disappearThreshold)
        {
            fruitRenderer.enabled = false;  // Sembunyikan buah
            RepositionFruit();  // Posisikan ulang di bawah layar
            jumpTimer = 0f;  // Reset timer
        }
        else
        {
            fruitRenderer.enabled = true;  // Tampilkan buah saat naik
        }
    }

    void RepositionFruit()
    {
        // Pilih posisi X acak di dalam batas layar
        float randomX = Random.Range(minX, maxX);

        // Mulai di bawah layar
        Vector3 minScreenBounds = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, distanceFromCamera));
        float belowScreenY = minScreenBounds.y - fruitHeight;

        // Tentukan posisi awal buah
        startPosition = new Vector3(randomX, belowScreenY, distanceFromCamera);
    }
}
