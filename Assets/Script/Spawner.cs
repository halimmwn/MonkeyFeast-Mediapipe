using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] fruits;     // prefab buah
    public GameObject[] dangers;    // prefab bahaya

    public float spawnInterval = 1f;   // jeda spawn
    private float timer = 0f;

    void Update()
    {
        // kalau game over, stop spawn
        if (GameManager.instance.isGameOver) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnRandom();
            timer = 0f;
        }
    }

    void SpawnRandom()
    {
        // posisi X random
        Vector3 spawnPos = new Vector3(
            Random.Range(-7f, 7f),     // kiri-kanan
            transform.position.y,
            0
        );

        float chance = Random.value;  // 0.0 - 1.0

        if (chance < 0.8f)
        {
            // 80% spawn buah
            int index = Random.Range(0, fruits.Length);
            Instantiate(fruits[index], spawnPos, Quaternion.identity);
        }
        else
        {
            // 20% spawn bahaya
            int index = Random.Range(0, dangers.Length);
            Instantiate(dangers[index], spawnPos, Quaternion.identity);
        }
    }
}
