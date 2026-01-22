using UnityEngine;

public class PatternSpawner : MonoBehaviour
{
    // --- LEVELING SYSTEM ---
    public enum Difficulty { Easy, Medium, Hard }

    [Header("Level Difficulty")]
    public Difficulty currentDifficulty = Difficulty.Medium;

    [Header("Object Categories")]
    public GameObject[] fruits;
    public GameObject[] woods;
    public GameObject[] dangers;

    public GameObject goldPrefab;
    public GameObject potionPrefab;

    [Header("Spawn Settings")]
    public float baseInterval = 0.9f;
    private float timer;        // Timer khusus untuk Buah/Bom
    private float potionTimer;  // Timer KHUSUS untuk Potion (Baru)

    // Variabel probabilitas dinamis
    private float difficultyIntervalMultiplier = 1f;
    private float probFruit = 0.5f;
    private float probWood = 0.25f;

    // Variabel interval potion (Baru)
    private float potionInterval = 20f;

    private int goldSpawnCount = 0;

    // Hapus potionSpawnCount agar unlimited
    // private int potionSpawnCount = 0; 

    private float goldTimeA;
    private float goldTimeB;

    void Start()
    {
        // 1. LOAD LEVEL
        int savedLevel = PlayerPrefs.GetInt("SelectedLevel", 1);

        if (savedLevel == 0) currentDifficulty = Difficulty.Easy;
        else if (savedLevel == 1) currentDifficulty = Difficulty.Medium;
        else if (savedLevel == 2) currentDifficulty = Difficulty.Hard;

        // 2. TERAPKAN SETTING
        ApplyDifficultySettings();

        float total = GameManager.instance.timer;
        goldTimeA = Random.Range(total * 0.3f, total * 0.45f);
        goldTimeB = Random.Range(total * 0.65f, total * 0.85f);
    }

    void ApplyDifficultySettings()
    {
        switch (currentDifficulty)
        {
            case Difficulty.Easy:
                difficultyIntervalMultiplier = 1.3f;
                probFruit = 0.60f;
                probWood = 0.30f;
                // Easy: Potion muncul sering (tiap 15 detik)
                potionInterval = 15f;
                break;

            case Difficulty.Medium:
                difficultyIntervalMultiplier = 1.0f;
                probFruit = 0.50f;
                probWood = 0.25f;
                // Medium: Potion muncul standar (tiap 25 detik)
                potionInterval = 25f;
                break;

            case Difficulty.Hard:
                difficultyIntervalMultiplier = 0.7f;
                probFruit = 0.30f;
                probWood = 0.30f;
                // Hard: Potion sangat jarang (tiap 40 detik)
                potionInterval = 40f;
                break;
        }
    }

    void Update()
    {
        if (GameManager.instance.isGameOver) return;

        // Update kedua timer secara terpisah
        timer += Time.deltaTime;       // Timer untuk buah
        potionTimer += Time.deltaTime; // Timer untuk potion

        float t = GameManager.instance.timer;

        // GOLD EVENT
        if (goldSpawnCount == 0 && t <= goldTimeA) SpawnGold();
        if (goldSpawnCount == 1 && t <= goldTimeB) SpawnGold();

        // POTION (Panggil fungsi baru)
        UpdatePotionSpawn();

        // PATTERN SYSTEM (Buah & Bom)
        if (t > 70) NormalSpawn();
        else if (t > 55) MixedLinePattern();
        else if (t > 40) MixedClusterPattern();
        else if (t > 25) DangerBoostPattern();
        else FastNormalSpawn();
    }

    // ============================
    // SPAWN LOGIC - BUAH/BOM
    // ============================
    void NormalSpawn()
    {
        if (timer < baseInterval * difficultyIntervalMultiplier) return;
        timer = 0; // Reset timer buah saja
        SpawnSingleObject();
    }

    void FastNormalSpawn()
    {
        if (timer < 0.6f * difficultyIntervalMultiplier) return;
        timer = 0;
        SpawnSingleObject();
    }

    void SpawnSingleObject()
    {
        Vector3 pos = new Vector3(Random.Range(-7, 7), transform.position.y, 0);
        float r = Random.value;

        if (r < probFruit) Instantiate(fruits[Random.Range(0, fruits.Length)], pos, Quaternion.identity);
        else if (r < (probFruit + probWood)) Instantiate(woods[Random.Range(0, woods.Length)], pos, Quaternion.identity);
        else Instantiate(dangers[Random.Range(0, dangers.Length)], pos, Quaternion.identity);
    }

    // ... (Fungsi Pattern Lain Tetap Sama - MixedLine, Cluster, DangerBoost) ...
    void MixedLinePattern()
    {
        if (timer < 2f * difficultyIntervalMultiplier) return;
        timer = 0;
        for (int i = -2; i <= 2; i++)
        {
            Vector3 pos = new Vector3(i * 2f, transform.position.y, 0);
            SpawnWithDifficultyLogic(pos);
        }
    }

    void MixedClusterPattern()
    {
        if (timer < 1.8f * difficultyIntervalMultiplier) return;
        timer = 0;
        float cx = Random.Range(-5f, 5f);
        for (int i = 0; i < 5; i++)
        {
            Vector3 pos = new Vector3(cx + Random.Range(-1f, 1f), transform.position.y, 0);
            SpawnWithDifficultyLogic(pos);
        }
    }

    void DangerBoostPattern()
    {
        if (timer < 1f * difficultyIntervalMultiplier) return;
        timer = 0;
        Vector3 pos = new Vector3(Random.Range(-7, 7), transform.position.y, 0);

        float boostedDanger = (currentDifficulty == Difficulty.Hard) ? 0.6f : 0.45f;
        float remaining = 1f - boostedDanger;
        float r = Random.value;

        if (r < remaining * 0.6f) Instantiate(fruits[Random.Range(0, fruits.Length)], pos, Quaternion.identity);
        else if (r < remaining) Instantiate(woods[Random.Range(0, woods.Length)], pos, Quaternion.identity);
        else Instantiate(dangers[Random.Range(0, dangers.Length)], pos, Quaternion.identity);
    }

    void SpawnWithDifficultyLogic(Vector3 pos)
    {
        float r = Random.value;
        if (r < probFruit) Instantiate(fruits[Random.Range(0, fruits.Length)], pos, Quaternion.identity);
        else if (r < (probFruit + probWood)) Instantiate(woods[Random.Range(0, woods.Length)], pos, Quaternion.identity);
        else Instantiate(dangers[Random.Range(0, dangers.Length)], pos, Quaternion.identity);
    }

    void SpawnGold()
    {
        if (goldSpawnCount >= 2) return;
        goldSpawnCount++;
        Vector3 pos = new Vector3(Random.Range(-6, 6), transform.position.y, 0);
        Instantiate(goldPrefab, pos, Quaternion.identity);
    }

    // ============================
    // LOGIKA POTION BARU
    // ============================
    void UpdatePotionSpawn()
    {
        // 1. Cek Timer
        if (potionTimer < potionInterval) return;

        // 2. Cek apakah nyawa kurang dari 3 (Opsional)
        // Kalau user bilang "muncul terus aja", baris di bawah ini bisa dihapus/dikomentari.
        // Tapi biasanya game balance tetap butuh ini. 
        // Jika kamu mau benar-benar muncul walau darah penuh, HAPUS baris if di bawah ini.
        if (GameManager.instance.lives >= 3) return;

        // 3. Reset Timer & Spawn
        potionTimer = 0;

        Vector3 pos = new Vector3(Random.Range(-5, 5), transform.position.y, 0);
        Instantiate(potionPrefab, pos, Quaternion.identity);
    }
}