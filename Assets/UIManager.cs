using UnityEngine;
using TMPro; // Tambahkan ini

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI textBuah; // Ubah menjadi TextMeshProUGUI
    public TextMeshProUGUI textUang; // Ubah menjadi TextMeshProUGUI

    void Update()
    {
        // Update jumlah buah dan uang di UI
        textBuah.text = "Buah Dihancurkan: " + GameManager.instance.GetDestroyedFruitsCount();
        textUang.text = "Uang: $" + GameManager.instance.GetMoney();
    }
}
