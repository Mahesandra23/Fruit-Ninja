using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private int destroyedFruitsCount = 0;
    private int missedFruitsCount = 0;
    private int uang = 0;

    public List<CubeClicker> fruits; // Daftar semua buah
    public float spawnDelay = 2f; // Jeda antar buah
    public int maxActiveFruits = 5; // Jumlah maksimum buah yang aktif sekaligus

    private List<CubeClicker> activeFruits = new List<CubeClicker>(); // Buah yang sedang aktif
    private bool isGameOver = false; // Flag untuk memeriksa status game
    public int maxDestroyedFruits = 5; // Batas jumlah buah yang dihancurkan
    public int maxMissedFruits = 3; // Batas jumlah buah yang tidak dihancurkan

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
        if (destroyedFruitsCount >= maxDestroyedFruits || missedFruitsCount >= maxMissedFruits)
        {
            StopGame();
        }
    }

    private void StopGame()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            Debug.Log("Permainan selesai!");

            // Hentikan semua aktivitas permainan, misalnya menghentikan spawn buah
            StopAllCoroutines();

            // Tampilkan pesan game over atau lakukan aksi lainnya
            // Misalnya, tampilkan UI "Game Over"
        }
    }

    public void IncreaseDestroyedFruitCount()
    {
        destroyedFruitsCount++;
        AddMoney(10);
        Debug.Log("Buah yang hancur: " + destroyedFruitsCount + ", Uang: " + uang + " dollar");
    }

    public void IncreaseMissedFruitCount()
    {
        missedFruitsCount++;
        Debug.Log("Buah yang tidak dihancurkan: " + missedFruitsCount);
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
        if (activeFruits.Contains(fruit))
        {
            activeFruits.Remove(fruit); // Hapus dari daftar buah aktif
        }

        IncreaseMissedFruitCount();
    }
}
