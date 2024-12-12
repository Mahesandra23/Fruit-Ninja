using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI textBuah;
    public TextMeshProUGUI textUang;
    public TextMeshProUGUI textScore;
    public TextMeshProUGUI textNyawa; // Tambahkan referensi untuk menampilkan nyawa
    public GameObject gameOverPanel;

    void Update()
    {
        textBuah.text = "Buah: " + GameManager.instance.GetDestroyedFruitsCount();
        textUang.text = "Uang: $" + GameManager.instance.GetMoney();
        textScore.text = "Score: " + GameManager.instance.GetScore();
        textNyawa.text = "Nyawa: " + GameManager.instance.GetLives(); // Menampilkan nyawa

        if (GameManager.instance.IsGameOver)
        {
            gameOverPanel.SetActive(true);
        }
    }
}
