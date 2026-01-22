using UnityEngine;

public class PendulumAnimation : MonoBehaviour
{
    [Header("Pengaturan Ayunan")]
    public float speed = 2.0f;      // Kecepatan ayunan

    // UBAH DISINI: Semakin KECIL angkanya, semakin PENDEK/SEMPIT ayunannya.
    // Jika 10 masih terlalu jauh, coba ganti jadi 5 di Inspector.
    public float angleLimit = 10.0f;

    private float randomStart;

    void Start()
    {
        // Biar kalau ada banyak monyet, gerakannya nggak barengan persis
        randomStart = Random.Range(0f, 100f);
    }

    void Update()
    {
        // Rumus Matematika Sinus untuk gerakan bolak-balik halus
        float angle = angleLimit * Mathf.Sin((Time.time + randomStart) * speed);

        // Terapkan rotasi pada sumbu Z
        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}