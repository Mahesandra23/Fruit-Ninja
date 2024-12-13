    using UnityEngine;
    using System.Collections;

    public class CubeClicker : MonoBehaviour
    {
        private Renderer fruitRenderer;
        public Camera mainCamera;
        public float jumpHeight = 30f;
        public float jumpSpeed = 2f;
        public float moveSpeed = 1f;
        public float horizontalSpeed = 0.5f; // Kecepatan pergerakan horizontal
        public float distanceFromCamera = 6f;
        public float disappearThreshold = -8f;
        public float spawnHeight = -30f;

        public GameObject fruitExplosionPrefab;
        public bool isBomb = false; // Menentukan apakah objek ini adalah bom

        public int maxClicks = 2; // Menentukan jumlah klik untuk meledakkan buah
        public float colorChangeDuration = 0.2f; // Durasi warna berubah menjadi merah
        public float smallJumpHeight = 5f; // Tinggi lompatan kecil saat diklik

        private Vector3 startPosition;
        private bool isJumping = false;
        private float jumpTimer = 0f;
        private float moveTimer = 0f;
        private float fruitWidth = 10f;
        private int jumpDirection;
        private float minX, maxX;

        private int clickCount = 0; // Menyimpan jumlah klik saat ini
        private Color originalColor; // Menyimpan warna asli buah
        private int horizontalDirection; // Menentukan arah horizontal pergerakan

        void Start()
        {
            fruitRenderer = GetComponent<Renderer>();

            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            minX = -9;
            maxX = 9;

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
            if (isBomb) // Jika objek ini adalah bom
            {
                Vector3 centerPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, distanceFromCamera));
                // Mengatur posisi Z untuk menjaga jarak dari kamera
                centerPosition.z = distanceFromCamera - 5f;

                // Panggil fungsi BombClicked dengan posisi tengah
                GameManager.instance.BombClicked(centerPosition);
                GameManager.instance.FruitDestroyed(this);
                return; // Keluar dari fungsi
            }

            clickCount++;

            ApplySmallJump();

            if (clickCount >= maxClicks)
            {
                GameObject explosion = Instantiate(fruitExplosionPrefab, transform.position, Quaternion.identity);
                Destroy(explosion, 2f);

                GameManager.instance.IncreaseDestroyedFruitCount();

                // Beritahu GameManager bahwa buah ini sudah dihancurkan
                GameManager.instance.FruitDestroyed(this);

                clickCount = 0;
                gameObject.SetActive(false); // Nonaktifkan buah
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

        public void StartJumping()
        {
            isJumping = true;
            jumpDirection = startPosition.x < 0 ? 1 : -1;

            // Tentukan arah horizontal berdasarkan posisi awal
            horizontalDirection = startPosition.x < 0 ? 1 : -1; // Jika muncul dari kiri, bergerak ke kanan dan sebaliknya
        }

        void AnimateJump()
        {
            if (GameManager.instance != null && GameManager.instance.IsGameOver)
            {
                return; // Jangan lakukan apapun jika game sudah selesai
            }

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
                gameObject.SetActive(false); // Nonaktifkan buah setelah gagal
                clickCount = 0;
            }
            else
            {
                fruitRenderer.enabled = true;
            }
        }


        public void RepositionFruit()
        {
            if (GameManager.instance != null && GameManager.instance.IsGameOver)
            {
                return; // Jangan lakukan apapun jika game sudah selesai
            }

            float randomX = Random.Range(minX, maxX);
            startPosition = new Vector3(randomX, spawnHeight, distanceFromCamera);
            transform.position = startPosition;

            jumpTimer = 0f;
            moveTimer = 0f;

            horizontalDirection = startPosition.x < 0 ? 1 : -1;
        }


        public void HideFruit()
        {
            fruitRenderer.enabled = false; // Sembunyikan buah
            isJumping = false; // Matikan loncatan
            clickCount = 0; // Reset jumlah klik
        }
    }