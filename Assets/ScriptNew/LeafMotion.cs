using UnityEngine;

public class LeafMotion : MonoBehaviour
{
    [Header("Kecepatan Jatuh")]
    public float fallSpeed = 2f;       // Seberapa cepat jatuh ke bawah

    [Header("Efek Goyang (Angin)")]
    public float swayAmount = 1.5f;    // Seberapa jauh goyang ke kiri-kanan
    public float swaySpeed = 2f;       // Seberapa cepat goyangnya

    [Header("Efek Putar")]
    public float rotateSpeed = 50f;    // Kecepatan berputar

    private float startX;
    private float timeOffset;
    private float randomRotateDir;

    void Start()
    {
        // Simpan posisi X awal saat muncul
        startX = transform.position.x;

        // Acak waktu mulai goyang agar tiap daun beda-beda gerakannya
        timeOffset = Random.Range(0f, 10f);

        // Acak arah putaran (bisa ke kiri atau ke kanan)
        randomRotateDir = Random.Range(0, 2) == 0 ? 1 : -1;

        // Hancurkan otomatis setelah 10 detik agar tidak menuhin memori
        Destroy(gameObject, 10f);
    }

    void Update()
    {
        // 1. Gerakan Jatuh ke Bawah
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        // 2. Gerakan Goyang (Sinusoidal)
        // Rumus matematika: Posisi X = Awal + (Sin(waktu) * Jarak)
        float sway = Mathf.Sin((Time.time + timeOffset) * swaySpeed) * swayAmount;

        Vector3 pos = transform.position;
        pos.x = startX + sway;
        transform.position = pos;

        // 3. Gerakan Berputar
        transform.Rotate(0, 0, rotateSpeed * randomRotateDir * Time.deltaTime);
    }
}