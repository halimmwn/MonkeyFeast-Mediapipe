using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    private GameManager gm;

    [Header("Audio SFX")] // --- TAMBAHAN BARU ---
    public AudioClip collectSound;  // Drag suara 'Cling' (Buah/Gold)
    public AudioClip damageSound;   // Drag suara 'Uh' (Danger)
    public AudioClip healSound;     // Drag suara 'Sparkle' (Potion)

    [Header("Audio Source")]
    public AudioSource sfxSource;   // Komponen AudioSource

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        gm = GameManager.instance;

        // --- TAMBAHAN BARU: Cari AudioSource otomatis jika kosong ---
        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();
    }

    public void OnCatch(string tagName)
    {
        switch (tagName)
        {
            case "Fruit":
                gm.AddScore(3);
                PlaySFX(collectSound); // Bunyi Cling
                break;

            case "Wood":
                gm.AddScore(-3);
                PlaySFX(damageSound);// Jika mau ada suara kayu, bisa tambah di sini nanti
                break;

            case "Danger":
                gm.ReduceLife();
                PlaySFX(damageSound);  // Bunyi Uh/Ledakan
                break;

            case "Gold":
                gm.AddScore(15);
                PlaySFX(collectSound); // Bunyi Cling
                break;

            case "Potion":
                gm.RestoreLife();
                PlaySFX(healSound);    // Bunyi Sparkle
                break;

            default:
                break;
        }
    }

    // --- FUNGSI TAMBAHAN: Untuk memutar suara ---
    void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}