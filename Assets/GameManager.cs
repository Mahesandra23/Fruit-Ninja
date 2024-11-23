using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private int destroyedFruitsCount = 0;
    private int uang = 0; // Variabel untuk menyimpan uang

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

    public void IncreaseDestroyedFruitCount()
    {
        destroyedFruitsCount++;
        AddMoney(10); // Tambahkan 10 dollar setiap buah dihancurkan
        Debug.Log("Buah yang hancur: " + destroyedFruitsCount + ", Uang: " + uang + " dollar");
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
}