using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI textBuah;
    public TextMeshProUGUI textUang;
    public TextMeshProUGUI textScore;
    public TextMeshProUGUI textNyawa; // Tambahkan referensi untuk menampilkan nyawa
    public GameObject gameOverPanel;
    public TextMeshProUGUI textHighScores; // Add a reference to display high scores

    void Update()
    {
        textBuah.text = "Buah: " + GameManager.instance.GetDestroyedFruitsCount();
        textUang.text = "Uang: $" + GameManager.instance.GetMoney();
        textScore.text = "Score: " + GameManager.instance.GetScore();
        textNyawa.text = "Nyawa: " + GameManager.instance.GetLives(); // Menampilkan nyawa

        // Display top 5 high scores
        DisplayHighScores();

        if (GameManager.instance.IsGameOver)
        {
            gameOverPanel.SetActive(true);
        }
    }

    void DisplayHighScores()
    {
        var highScores = GameManager.instance.GetHighScores();  // Get top 5 high scores

        string highScoresText = "Top 5 High Scores:\n";
        for (int i = 0; i < highScores.Count; i++)
        {
            highScoresText += (i + 1) + ". " + highScores[i] + "\n";  // Display each score
        }

        textHighScores.text = highScoresText;  // Set the text to the UI TextMeshPro element
    }
}
