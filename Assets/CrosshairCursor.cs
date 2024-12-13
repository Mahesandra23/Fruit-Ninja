using UnityEngine;

public class CrosshairCursor : MonoBehaviour
{
    public Texture2D crosshairTexture; // Drag & drop gambar crosshair ke sini
    public Vector2 hotSpot = Vector2.zero; // Posisi titik aktif pada crosshair
    public CursorMode cursorMode = CursorMode.Auto;

    void Start()
    {
        // Ubah kursor menjadi crosshair
        Cursor.SetCursor(crosshairTexture, hotSpot, cursorMode);
    }
}