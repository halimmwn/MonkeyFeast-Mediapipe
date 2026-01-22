using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Penting untuk reload scene

public class GameOverUI : MonoBehaviour
{
    public GameObject gameOverPanel; // Panel yang berisi menu Game Over
    public Text finalScoreText;      // Text untuk menampilkan skor akhir

    // Fungsi ini dipanggil oleh GameManager saat game selesai
    public void Setup(int score)
    {
        gameOverPanel.SetActive(true); // Munculkan panel
        finalScoreText.text = "Final Score: " + score.ToString();
    }

    // Fungsi untuk tombol Restart
    public void RestartButton()
    {
        // PENTING: Kembalikan waktu berjalan normal sebelum reload scene
        // Karena di GameManager kamu men-set Time.timeScale = 0 saat GameOver
        Time.timeScale = 1;

        // Reload scene yang sedang aktif
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}