using UnityEngine;

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

    private Vector3 startPosition;
    private bool isJumping = false;
    private float jumpTimer = 0f;
    private float moveTimer = 0f;
    private float fruitWidth;
    private int jumpDirection;
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
        // Instantiate explosion effect
        GameObject explosion = Instantiate(fruitExplosionPrefab, transform.position, transform.rotation);

        // Destroy explosion effect after 2 seconds
        Destroy(explosion, 2f);

        // Reposition the fruit at spawnHeight
        RepositionFruit();
        
        // Restart jumping animation
        StartJumping();
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

        // Reset the timers to start jump from the beginning
        jumpTimer = 0f;
        moveTimer = 0f;
    }
}
