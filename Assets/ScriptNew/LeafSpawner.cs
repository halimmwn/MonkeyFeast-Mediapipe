using UnityEngine;
using System.Collections; // Wajib ada untuk fungsi IEnumerator

public class LeafSpawner : MonoBehaviour
{
    [Header("Aset")]
    public GameObject leafPrefab;   // Masukkan Prefab Daun di sini

    [Header("Pengaturan Spawn")]
    public float spawnInterval = 0.5f; // Muncul setiap berapa detik
    public float spawnRangeX = 9f;     // Lebar area spawn (sesuaikan lebar layar)

    void Start()
    {
        // Memulai loop tanpa henti (Unlimited)
        // Coroutine lebih aman dan fleksibel daripada InvokeRepeating untuk loop tanpa batas
        StartCoroutine(SpawnLoop());
    }

    // Fungsi Looping Unlimited
    IEnumerator SpawnLoop()
    {
        // while(true) artinya "lakukan selamanya" selama objek ini aktif & game berjalan
        while (true)
        {
            SpawnLeaf();

            // Tunggu sesuai interval sebelum spawn lagi (misal: 0.5 detik)
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnLeaf()
    {
        if (leafPrefab == null) return;

        // Tentukan posisi X secara acak (kiri sampai kanan)
        float randomX = Random.Range(-spawnRangeX, spawnRangeX);

        // Mengambil posisi Y paling atas dari kamera utama secara otomatis
        // Ini memastikan daun selalu muncul dari luar layar bagian atas
        float screenTop = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;

        // Posisi spawn: X acak, Y di atas layar
        Vector3 spawnPos = new Vector3(transform.position.x + randomX, screenTop + 1.0f, 0);

        // Buat objek daunnya
        Instantiate(leafPrefab, spawnPos, Quaternion.identity);
    }
}