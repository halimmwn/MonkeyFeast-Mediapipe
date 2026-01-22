using UnityEngine;
using System.Collections;

public class PopUpAnimation : MonoBehaviour
{
    [Header("Pengaturan Animasi")]
    public float duration = 0.5f; // Durasi animasi (detik)

    // Kurva ini saya atur lebih soft (hanya membal sedikit 5% biar elegan)
    public AnimationCurve popUpCurve = new AnimationCurve(
        new Keyframe(0f, 0f),         // Mulai dari 0
        new Keyframe(0.8f, 1.05f),    // Membal dikit (105%)
        new Keyframe(1f, 1f)          // Normal kembali
    );

    private Vector3 originalScale; // Variabel untuk menyimpan ukuran asli desainmu

    private void Awake()
    {
        // PENTING: Simpan ukuran asli object saat pertama kali jalan
        // Ini mencegah UI jadi "penyet" atau berubah proporsi
        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        // Reset ke 0 dulu baru animasi
        transform.localScale = Vector3.zero;
        StartCoroutine(AnimatePopUp());
    }

    IEnumerator AnimatePopUp()
    {
        float timer = 0f;

        while (timer < duration)
        {
            // Gunakan unscaledDeltaTime agar tetap jalan saat Game Over (TimeScale = 0)
            timer += Time.unscaledDeltaTime;

            float progress = timer / duration;
            float curveValue = popUpCurve.Evaluate(progress);

            // PENTING: Kalikan nilai animasi dengan ukuran asli (originalScale)
            // Jadi kalau desainmu X=2 Y=1, dia akan tetap proporsional X=2 Y=1
            transform.localScale = originalScale * curveValue;

            yield return null;
        }

        // Pastikan di akhir animasi ukurannya kembali ke ukuran asli yang presisi
        transform.localScale = originalScale;
    }
}