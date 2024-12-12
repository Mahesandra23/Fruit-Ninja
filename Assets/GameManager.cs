using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private int destroyedFruitsCount = 0;
    private int missedFruitsCount = 0;
    private int uang = 0;
    private int score = 0;

    public List<CubeClicker> fruits; // Daftar semua buah
    public float spawnDelay = 2f; // Jeda antar buah
    public int maxActiveFruits = 5; // Jumlah maksimum buah yang aktif sekaligus

    private List<CubeClicker> activeFruits = new List<CubeClicker>(); // Buah yang sedang aktif
    private bool isGameOver = false; // Flag untuk memeriksa status game
    public int maxDestroyedFruits = 5; // Batas jumlah buah yang dihancurkan
    public int maxMissedFruits = 3; // Batas jumlah buah yang tidak dihancurkan

    private int bombClickedCount = 0; // Jumlah bom yang telah diklik
    public int maxBombClicks = 1; // Jumlah maksimum bom yang boleh diklik sebelum game over
    public GameObject explosionEffect;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Nonaktifkan semua buah di awal
        foreach (CubeClicker fruit in fruits)
        {
            fruit.gameObject.SetActive(false);
        }

        // Mulai coroutine untuk memunculkan buah
        StartCoroutine(SpawnFruitsSequentially());
    }

    void Update()
    {
        // Periksa apakah permainan sudah selesai
        CheckStopCriteria();
    }

    // Fungsi untuk mengecek apakah permainan harus berhenti
    private void CheckStopCriteria()
    {
        if (destroyedFruitsCount >= maxDestroyedFruits || 
            missedFruitsCount >= maxMissedFruits || 
            bombClickedCount >= maxBombClicks)
        {
            StopGame();
        }
    }


    public bool IsGameOver
    {
        get { return isGameOver; }
    }

    private void StopGame()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            Debug.Log("Permainan selesai!");

            StopAllCoroutines(); // Hentikan semua coroutine
            
            // Nonaktifkan semua buah yang aktif
            foreach (CubeClicker fruit in activeFruits)
            {
                fruit.gameObject.SetActive(false);
            }
            activeFruits.Clear();

            // Lakukan aksi lainnya, misalnya tampilkan UI Game Over
        }
    }


    public void IncreaseDestroyedFruitCount()
    {
        destroyedFruitsCount++;
        AddMoney(10);
        score += 10; // Menambahkan skor saat buah dihancurkan
        Debug.Log("Buah yang hancur: " + destroyedFruitsCount + ", Uang: " + uang + " dollar, Score: " + score);
    }

    public void IncreaseMissedFruitCount()
    {
        missedFruitsCount++;
        if (missedFruitsCount > maxMissedFruits) // Pastikan tidak lebih dari maxMissedFruits
        {
            missedFruitsCount = maxMissedFruits;
        }
    }

    private void AddMoney(int amount)
    {
        uang += amount;
    }

    public int GetDestroyedFruitsCount()
    {
        return destroyedFruitsCount;
    }

    public int GetMoney()
    {
        return uang;
    }

    public int GetScore() // Fungsi untuk mendapatkan skor
    {
        return missedFruitsCount;
    }

    public int GetLives()
    {
        return maxMissedFruits - missedFruitsCount;
    }

    // Coroutine untuk spawn buah dengan logika batas maksimum
    private IEnumerator SpawnFruitsSequentially()
    {
        while (!isGameOver)  // Jangan spawn buah jika game sudah selesai
        {
            // Hanya spawn jika jumlah buah aktif lebih sedikit dari batas maksimum
            if (activeFruits.Count < maxActiveFruits)
            {
                CubeClicker fruitToSpawn = GetInactiveFruit();

                if (fruitToSpawn != null)
                {
                    fruitToSpawn.gameObject.SetActive(true); // Aktifkan buah
                    fruitToSpawn.RepositionFruit(); // Posisi ulang buah
                    fruitToSpawn.StartJumping(); // Mulai animasi loncatan
                    activeFruits.Add(fruitToSpawn); // Tambahkan ke daftar buah aktif
                }
            }

            yield return new WaitForSeconds(spawnDelay); // Tunggu sebelum mencoba spawn lagi
        }
    }

    // Fungsi untuk mendapatkan buah yang tidak aktif
    private CubeClicker GetInactiveFruit()
    {
        List<CubeClicker> inactiveFruits = new List<CubeClicker>();

        foreach (CubeClicker fruit in fruits)
        {
            if (!fruit.gameObject.activeSelf) // Jika buah tidak aktif
            {
                inactiveFruits.Add(fruit);
            }
        }

        if (inactiveFruits.Count > 0)
        {
            int randomIndex = Random.Range(0, inactiveFruits.Count);
            return inactiveFruits[randomIndex];
        }

        return null; // Tidak ada buah yang tersedia
    }

    // Panggil fungsi ini saat buah dihancurkan
    public void FruitDestroyed(CubeClicker fruit)
    {
        if (activeFruits.Contains(fruit))
        {
            activeFruits.Remove(fruit); // Hapus dari daftar buah aktif
        }
    }

    // Panggil fungsi ini saat buah tidak dihancurkan
    public void FruitMissed(CubeClicker fruit)
    {
        if (fruit.isBomb) return;

        if (activeFruits.Contains(fruit))
        {
            activeFruits.Remove(fruit); // Hapus dari daftar buah aktif
        }

        IncreaseMissedFruitCount();
    }

    public void BombClicked(Vector3 position)
    {
        bombClickedCount++;
        Debug.Log("Bom diklik! Total bom: " + bombClickedCount);

        // Tampilkan efek ledakan di posisi tengah layar
        Instantiate(explosionEffect, position, Quaternion.identity);

        // Guncangkan kamera
        Camera.main.GetComponent<CameraShake>().StartCoroutine(Camera.main.GetComponent<CameraShake>().Shake(0.5f, 0.3f));

        if (bombClickedCount >= maxBombClicks)
        {
            Debug.Log("Game over karena bom diklik!");
            StopGame();
        }
    }


}
