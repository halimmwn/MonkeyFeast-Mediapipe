using UnityEngine;
using UnityEngine.UI;

public class HeartUI : MonoBehaviour
{
    [Header("UI References")]
    public Image[] hearts;       // Pastikan Drag: Heart1, Heart2, Heart3 di Inspector

    [Header("Settings")]
    [Range(0f, 1f)]
    public float fadedAlpha = 0.2f;  // SAYA UBAH JADI 0.2 BIAR KELIHATAN MATINYA

    private void Start()
    {
        // Reset: Pastikan semua hati menyala (100%) di awal game
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null)
                SetHeartAlpha(i, 1f);
        }
    }

    // Dipanggil saat kena BOM
    public void LoseHeart(int index)
    {
        // Validasi agar tidak error
        if (index < 0 || index >= hearts.Length) return;

        Debug.Log("Hati berkurang di index: " + index); // Cek Console kalau ini muncul

        // Ubah jadi transparan (mati)
        SetHeartAlpha(index, fadedAlpha);
    }

    // Dipanggil saat dapat POTION
    public void RestoreHeart(int index)
    {
        if (index < 0 || index >= hearts.Length) return;

        // Ubah jadi terang lagi (hidup)
        SetHeartAlpha(index, 1f);
    }

    // Fungsi pembantu mengubah warna alpha
    private void SetHeartAlpha(int index, float alpha)
    {
        if (hearts[index] == null) return;

        Color tempColor = hearts[index].color;
        tempColor.a = alpha;
        hearts[index].color = tempColor;
    }
}