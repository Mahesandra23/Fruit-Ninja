using UnityEngine;
using System.Collections;

public class FruitSpawner : MonoBehaviour
{
    public GameObject[] fruits; // Array untuk berbagai jenis buah (Prefab)
    public float minSpawnInterval = 1f; // Waktu spawn minimum
    public float maxSpawnInterval = 3f; // Waktu spawn maksimum
    public float spawnXRange = 9f; // Jangkauan X tempat spawn buah
    public float spawnYPosition = -30f; // Posisi Y tempat buah muncul

    void Start()
    {
        StartCoroutine(SpawnFruits());
    }

    IEnumerator SpawnFruits()
    {
        while (true)
        {
            // Waktu random untuk spawn berikutnya
            float spawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(spawnInterval);

            // Posisi X random untuk spawn buah
            float spawnX = Random.Range(-spawnXRange, spawnXRange);
            Vector3 spawnPosition = new Vector3(spawnX, spawnYPosition, 6f);

            // Pilih buah random dari array fruits
            int fruitIndex = Random.Range(0, fruits.Length);
            Instantiate(fruits[fruitIndex], spawnPosition, Quaternion.identity);
        }
    }
}
