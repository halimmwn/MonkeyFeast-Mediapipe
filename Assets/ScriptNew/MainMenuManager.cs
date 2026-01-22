using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Config")]
    public string gameSceneName = "MainScene";

    [Header("UI Panels")]
    public GameObject mainButtonsPanel;   // Panel Menu Utama
    public GameObject optionsPanel;
    public GameObject levelSelectionPanel; // Panel Leveling (Easy/Medium/Hard)

    [Header("Level Buttons")]
    public Button easyBtn;
    public Button mediumBtn;
    public Button hardBtn;

    [Header("Button Colors")]
    public Color selectedColor = Color.green;
    public Color normalColor = Color.white;

    [Header("Audio Settings")]
    public AudioSource backgroundMusic;
    public Text musicButtonText;

    private bool isMusicOn = true;

    void Start()
    {
        // 1. Setup Audio & Warna Level
        isMusicOn = true;
        UpdateMusicUI();
        int savedLevel = PlayerPrefs.GetInt("SelectedLevel", 1);
        UpdateLevelButtonsUI(savedLevel);

        // 2. CEK TITIPAN PESAN DARI GAME OVER (LOGIKA BARU)
        // Apakah kita diminta langsung buka Level Select?
        if (PlayerPrefs.GetInt("AutoOpenLevelSelect", 0) == 1)
        {
            // Ya -> Langsung buka panel level
            OpenLevelSelection();

            // Reset sinyalnya biar kalau restart game ngga nyangkut
            PlayerPrefs.SetInt("AutoOpenLevelSelect", 0);
            PlayerPrefs.Save();
        }
        else
        {
            // Tidak -> Buka menu utama seperti biasa
            BackToMenu();
        }
    }

    // --- FUNGSI PANEL ---

    public void OpenLevelSelection()
    {
        mainButtonsPanel.SetActive(false);
        if (optionsPanel) optionsPanel.SetActive(false);

        levelSelectionPanel.SetActive(true);
        levelSelectionPanel.transform.SetAsLastSibling();
    }

    public void BackToMenu()
    {
        if (optionsPanel) optionsPanel.SetActive(false);
        if (levelSelectionPanel) levelSelectionPanel.SetActive(false);

        mainButtonsPanel.SetActive(true);
        mainButtonsPanel.transform.SetAsLastSibling();
    }

    public void OpenOptions()
    {
        mainButtonsPanel.SetActive(false);
        optionsPanel.SetActive(true);
        optionsPanel.transform.SetAsLastSibling();
    }

    // --- FUNGSI GAMEPLAY ---

    public void SelectLevelAndPlay(int levelIndex)
    {
        PlayerPrefs.SetInt("SelectedLevel", levelIndex);
        PlayerPrefs.Save();
        SceneManager.LoadScene(gameSceneName);
    }

    public void ChooseEasyAndPlay() { SelectLevelAndPlay(0); }
    public void ChooseMediumAndPlay() { SelectLevelAndPlay(1); }
    public void ChooseHardAndPlay() { SelectLevelAndPlay(2); }

    void UpdateLevelButtonsUI(int levelIndex)
    {
        if (easyBtn) easyBtn.image.color = (levelIndex == 0) ? selectedColor : normalColor;
        if (mediumBtn) mediumBtn.image.color = (levelIndex == 1) ? selectedColor : normalColor;
        if (hardBtn) hardBtn.image.color = (levelIndex == 2) ? selectedColor : normalColor;
    }

    public void QuitGame() { Application.Quit(); }

    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        if (backgroundMusic != null) backgroundMusic.mute = !isMusicOn;
        UpdateMusicUI();
    }

    void UpdateMusicUI()
    {
        if (musicButtonText != null) musicButtonText.text = isMusicOn ? "MUSIC: ON" : "MUSIC: OFF";
    }
}