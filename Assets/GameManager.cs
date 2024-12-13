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
    public List<CubeClicker> bombs;
    public float spawnDelay = 2f; // Jeda antar buah
    public int maxActiveFruits = 5; // Jumlah maksimum buah yang aktif sekaligus

    private List<CubeClicker> activeObjects = new List<CubeClicker>();
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
        foreach (CubeClicker bomb in bombs)
        {
            bomb.gameObject.SetActive(false);
        }

        // Mulai coroutine untuk memunculkan buah/bom
        StartCoroutine(SpawnObjectsSequentially());
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
            
            // Nonaktifkan semua objek yang aktif
            foreach (CubeClicker obj in activeObjects)
            {
                obj.gameObject.SetActive(false);
            }
            activeObjects.Clear();

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
        return score;
    }

    public int GetLives()
    {
        return maxMissedFruits - missedFruitsCount;
    }

    // Coroutine untuk spawn buah dengan logika batas maksimum
private IEnumerator SpawnObjectsSequentially()
    {
        while (!isGameOver)  // Jangan spawn jika game sudah selesai
        {
            // Hanya spawn jika jumlah objek aktif lebih sedikit dari batas maksimum
            if (activeObjects.Count < maxActiveFruits)
            {
                CubeClicker objectToSpawn = GetRandomObject(); // Dapatkan objek (buah/bom) untuk di-spawn

                if (objectToSpawn != null)
                {
                    objectToSpawn.gameObject.SetActive(true); // Aktifkan objek
                    objectToSpawn.RepositionFruit(); // Posisi ulang objek
                    objectToSpawn.StartJumping(); // Mulai animasi loncatan
                    activeObjects.Add(objectToSpawn); // Tambahkan ke daftar objek aktif
                }
            }

            yield return new WaitForSeconds(spawnDelay); // Tunggu sebelum mencoba spawn lagi
        }
    }

    // Fungsi untuk mendapatkan objek acak (buah atau bom)
    private CubeClicker GetRandomObject()
    {
        List<CubeClicker> availableObjects = new List<CubeClicker>();

        foreach (CubeClicker fruit in fruits)
        {
            if (!fruit.gameObject.activeSelf) // Jika buah tidak aktif
            {
                availableObjects.Add(fruit);
            }
        }

        foreach (CubeClicker bomb in bombs)
        {
            if (!bomb.gameObject.activeSelf) // Jika bom tidak aktif
            {
                availableObjects.Add(bomb);
            }
        }

        if (availableObjects.Count > 0)
        {
            int randomIndex = Random.Range(0, availableObjects.Count);
            return availableObjects[randomIndex];
        }

        return null; // Tidak ada objek yang tersedia
    }

    // Panggil fungsi ini saat objek dihancurkan
    // Saat objek dihancurkan (bom)
    public void ObjectDestroyed(CubeClicker obj)
    {
        if (activeObjects.Contains(obj))
        {
            activeObjects.Remove(obj); // Hapus dari daftar objek aktif
        }

        if (fruits.Contains(obj))
        {
            IncreaseDestroyedFruitCount();
        }
        else if (bombs.Contains(obj))
        {
            BombClicked(obj.transform.position); // Berikan posisi bom
        }
    }

    // Fungsi untuk menangani bom yang diklik
    public void BombClicked(Vector3 position)
    {
        bombClickedCount++;
        Debug.Log("Bom diklik! Total bom: " + bombClickedCount);

        // Tampilkan efek ledakan di posisi bom
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, position, Quaternion.identity);
        }

        if (bombClickedCount >= maxBombClicks)
        {
            Debug.Log("Game over karena bom diklik!");
            StopGame();
        }
    }


    // Panggil fungsi ini saat objek tidak dihancurkan
    public void ObjectMissed(CubeClicker obj)
    {
        if (activeObjects.Contains(obj))
        {
            activeObjects.Remove(obj); // Hapus dari daftar objek aktif
        }

        if (fruits.Contains(obj))
        {
            IncreaseMissedFruitCount();
        }
    }

}
