using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Gameplay Status")]
    public int score = 0;
    public int lives = 3;
    public float timer = 90f;
    public bool isGameOver = false;

    [Header("UI References")]
    public GameObject gameOverPanel;
    public Text finalScoreText;

    [Header("Audio Settings")]
    public AudioSource bgmSource;       // Drag Audio Source yang memutar lagu gameplay
    public AudioSource sfxSource;       // Drag Audio Source untuk efek suara

    public AudioClip gameOverMusic;     // Lagu Game Over
    public AudioClip scoreCountSound;   // Bunyi 'tik' saat menghitung
    public AudioClip scoreFinishSound;  // Bunyi 'cling' saat selesai

    private HeartUI heartUI;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        heartUI = FindObjectOfType<HeartUI>();
        Time.timeScale = 1f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (isGameOver) return;

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = 0;
            GameOver();
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        if (score < 0) score = 0;
    }

    public void ReduceLife()
    {
        if (isGameOver) return;
        if (lives <= 0) return;

        lives--;
        if (heartUI != null) heartUI.LoseHeart(lives);
        if (lives <= 0) GameOver();
    }

    public void RestoreLife()
    {
        if (isGameOver) return;
        if (lives >= 3) return;
        if (heartUI != null) heartUI.RestoreHeart(lives);
        lives++;
    }

    // ========================
    // GAME OVER SYSTEM
    // ========================
    void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;
        Debug.Log("GAME OVER");

        // 1. Matikan musik gameplay
        if (bgmSource != null) bgmSource.Stop();

        // 2. Mainkan musik Game Over
        if (sfxSource != null && gameOverMusic != null)
            sfxSource.PlayOneShot(gameOverMusic);

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            if (finalScoreText != null)
            {
                StartCoroutine(AnimateScore());
            }
        }
    }

    IEnumerator AnimateScore()
    {
        int startScore = 0;
        int targetScore = score;
        float duration = 1.5f; // Sedikit diperlama biar kerasa countingnya
        float elapsed = 0f;

        // Timer untuk mengatur agar bunyi tidak terlalu cepat (seperti mesin jahit)
        float soundTimer = 0f;
        float soundInterval = 0.1f; // Bunyi setiap 0.1 detik

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float currentScore = Mathf.Lerp(startScore, targetScore, elapsed / duration);
            finalScoreText.text = Mathf.RoundToInt(currentScore).ToString();

            // --- Logika Bunyi Tik-Tik ---
            soundTimer += Time.unscaledDeltaTime;
            if (soundTimer >= soundInterval)
            {
                if (sfxSource != null && scoreCountSound != null)
                    sfxSource.PlayOneShot(scoreCountSound, 0.5f); // Volume 0.5
                soundTimer = 0f;
            }

            yield return null;
        }

        finalScoreText.text = targetScore.ToString();

        // --- Bunyi Finish (CLING!) ---
        if (sfxSource != null && scoreFinishSound != null)
            sfxSource.PlayOneShot(scoreFinishSound);
    }

    // --- FUNGSI TAMBAHAN UNTUK TOMBOL UI GAME OVER ---

    public void BackToMainMenu()
    {
        Time.timeScale = 1f; // PENTING: Kembalikan waktu agar game tidak pause di menu

        // Reset sinyal (0 = Buka menu biasa)
        PlayerPrefs.SetInt("AutoOpenLevelSelect", 0);
        PlayerPrefs.Save();

        SceneManager.LoadScene("SampleScene"); // Pastikan nama ini SAMA dengan nama scene menu kamu
    }

    // FUNGSI INI YANG DIKEMBALIKAN (Untuk Tombol "Change Level" / "Difficulty")
    public void GoToLevelSelect()
    {
        Time.timeScale = 1f;

        // Set sinyal ke 1 (Minta menu untuk LANGSUNG buka panel level selection)
        PlayerPrefs.SetInt("AutoOpenLevelSelect", 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene("SampleScene");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}