using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private int destroyedFruitsCount = 0;
    private int missedFruitsCount = 0;
    private int totalMoney = 0;  // Total money
    private int score = 0;       // Current score
    private List<int> highScores = new List<int>();  // List to hold the top 5 high scores

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
        // Load total money, current score, and high scores from PlayerPrefs
        totalMoney = PlayerPrefs.GetInt("TotalMoney", 0);  // Load the total money (default to 0)
        LoadHighScores();  // Load top 5 high scores from PlayerPrefs

        // Reset the current score for the new game
        score = 0;

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

            // Check if current score is higher than any of the top 5 high scores
            if (score > highScores[4])  // Only update if score is higher than the lowest in the top 5
            {
                highScores.Add(score);
                highScores.Sort((a, b) => b.CompareTo(a));  // Sort in descending order
                if (highScores.Count > 5) highScores.RemoveAt(highScores.Count - 1);  // Keep only top 5 scores
                SaveHighScores();  // Save the updated high scores to PlayerPrefs
            }

            // Save the current total money
            PlayerPrefs.SetInt("TotalMoney", totalMoney);

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
        AddMoney(10);  // Add 10 to total money when a fruit is destroyed
        score += 10;   // Add 10 to the score
        Debug.Log("Buah yang hancur: " + destroyedFruitsCount + ", Uang: " + totalMoney + " dollar, Score: " + score);

        // Save the updated score to PlayerPrefs (Optional if you want to persist current score)
        PlayerPrefs.SetInt("Score", score);  // Save the current score
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
        totalMoney += amount;  // Add the new money to the total
        PlayerPrefs.SetInt("TotalMoney", totalMoney);  // Save the updated total money
    }

    public int GetDestroyedFruitsCount()
    {
        return destroyedFruitsCount;
    }

    public int GetMoney()
    {
        return totalMoney;
    }

    public int GetScore() // Fungsi untuk mendapatkan skor
    {
        return score;
    }

    public List<int> GetHighScores() // Fungsi untuk mendapatkan daftar high scores
    {
        return highScores;
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

    private void SaveHighScores()
    {
        for (int i = 0; i < highScores.Count; i++)
        {
            PlayerPrefs.SetInt("HighScore_" + i, highScores[i]);  // Save top 5 scores
        }
    }

    private void LoadHighScores()
    {
        highScores.Clear();
        for (int i = 0; i < 5; i++)
        {
            int score = PlayerPrefs.GetInt("HighScore_" + i, 0);  // Load top 5 scores
            highScores.Add(score);
        }
    }

    // Optional: Reset game data
    public void ResetGameData()
    {
        PlayerPrefs.DeleteKey("TotalMoney");  // Reset the total money
        PlayerPrefs.DeleteKey("HighScore");   // Reset the high score
        totalMoney = 0;
        highScores.Clear();
    }
}
