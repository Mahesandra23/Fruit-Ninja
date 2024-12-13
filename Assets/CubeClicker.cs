using UnityEngine;
using System.Collections;

public class CubeClicker : MonoBehaviour
{
    private Renderer fruitRenderer;
    public Camera mainCamera;
    public float jumpHeight = 30f;
    public float jumpSpeed = 2f;
    public float moveSpeed = 1f;
    public float horizontalSpeed = 0.5f;
    public float distanceFromCamera = 6f;
    public float disappearThreshold = -8f;
    public float spawnHeight = -30f;

    public GameObject fruitExplosionPrefab;
    public int maxClicks = 2;
    public float colorChangeDuration = 0.2f;
    public float smallJumpHeight = 5f;

    private Vector3 startPosition;
    private bool isJumping = false;
    private float jumpTimer = 0f;
    private float moveTimer = 0f;
    private float fruitWidth = 10f;
    private int jumpDirection;
    private float minX, maxX;

    private int clickCount = 0;
    private Color originalColor;
    private int horizontalDirection;

    void Start()
    {
        fruitRenderer = GetComponent<Renderer>();

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        minX = -9;
        maxX = 9;

        originalColor = fruitRenderer.material.color;

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
        HandleClick();
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButton(0)) // Left mouse button held down
        {
            HandleClick();
        }
    }

    void HandleClick()
    {
        clickCount++;

        ApplySmallJump();

        if (clickCount >= maxClicks)
        {
            GameObject explosion = Instantiate(fruitExplosionPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 2f);

            GameManager.instance.IncreaseDestroyedFruitCount();

            GameManager.instance.FruitDestroyed(this);

            clickCount = 0;
            gameObject.SetActive(false);
        }
    }

    IEnumerator ChangeColorTemporary()
    {
        fruitRenderer.material.color = Color.red;
        yield return new WaitForSeconds(colorChangeDuration);
        fruitRenderer.material.color = originalColor;
    }

    void ApplySmallJump()
    {
        startPosition.y += smallJumpHeight;
    }

    public void StartJumping()
    {
        isJumping = true;
        jumpDirection = startPosition.x < 0 ? 1 : -1;
        horizontalDirection = startPosition.x < 0 ? 1 : -1;
    }

    void AnimateJump()
    {
        jumpTimer += Time.deltaTime * jumpSpeed;
        float jumpOffset = Mathf.Sin(jumpTimer) * jumpHeight;

        moveTimer += Time.deltaTime * moveSpeed;
        float moveOffset = Mathf.Sin(moveTimer * horizontalSpeed) * (fruitWidth * 0.5f) * horizontalDirection;

        float newX = Mathf.Clamp(startPosition.x + moveOffset, minX, maxX);
        transform.position = new Vector3(newX, startPosition.y + jumpOffset, distanceFromCamera);

        if (jumpOffset < disappearThreshold)
        {
            fruitRenderer.enabled = false;

            GameManager.instance.FruitMissed(this);

            RepositionFruit();
            clickCount = 0;
        }
        else
        {
            fruitRenderer.enabled = true;
        }
    }

    public void RepositionFruit()
    {
        float randomX = Random.Range(minX, maxX);
        startPosition = new Vector3(randomX, spawnHeight, distanceFromCamera);
        transform.position = startPosition;

        jumpTimer = 0f;
        moveTimer = 0f;

        horizontalDirection = startPosition.x < 0 ? 1 : -1;
    }

    public void HideFruit()
    {
        fruitRenderer.enabled = false;
        isJumping = false;
        clickCount = 0;
    }
}
