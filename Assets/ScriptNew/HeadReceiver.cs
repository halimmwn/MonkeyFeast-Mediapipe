using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Globalization;

public class HeadReceiver : MonoBehaviour
{
    [Header("Network Settings")]
    [Tooltip("UDP port to listen on (must match Python sender)")]
    public int port = 5005;
    public bool showDebugLogs = true;

    [Header("Head Movement Settings")]
    public float horizontalRange = 5f;
    public float smoothTime = 0.06f;
    public float predictionFactor = 0.08f;
    public float maxVelocity = 5f;

    // --- VARIABEL PUBLIK UNTUK TANGAN (Dibaca HandCursorController) ---
    [HideInInspector] public bool isFistDetected = false;
    [HideInInspector] public Vector2 handPositionNorm = new Vector2(0.5f, 0.5f);
    [HideInInspector] public bool isHandDetected = false;
    // ------------------------------------------------------------------

    // --- BUFFER THREAD (Penyimpanan Sementara) ---
    private float _rawHeadX = 0.5f;
    private Vector2 _rawHandPos = new Vector2(0.5f, 0.5f);
    private bool _rawIsFist = false;
    private bool _rawHandDetected = false;
    private float _rawTimestamp = 0f;
    private bool _hasNewData = false;
    private readonly object lockObj = new object();

    // --- BUFFER MAIN THREAD ---
    private float receivedX = 0.5f;
    private float lastReceivedTime = 0f;
    private float lastSampleTimestamp = 0f;
    private float prevSampleX = 0.5f;
    private float prevSampleTimestamp = 0f;

    private Thread receiveThread;
    private bool running = false;
    private Vector3 smoothVel = Vector3.zero;

    // Timer log untuk Thread (Pakai DateTime biar aman)
    private DateTime lastLogTime = DateTime.MinValue;

    void Start()
    {
        running = true;
        receiveThread = new Thread(ReceiveLoop);
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    void OnDestroy()
    {
        running = false;
        try
        {
            if (receiveThread != null && receiveThread.IsAlive)
                receiveThread.Join(200);
        }
        catch { }
    }

    void Update()
    {
        // 1. PINDAHKAN DATA DARI THREAD KE MAIN THREAD (UNITY)
        bool newDataArrived = false;
        float currentRawTimestamp = 0f;

        lock (lockObj)
        {
            if (_hasNewData)
            {
                newDataArrived = true;
                _hasNewData = false;

                // Simpan posisi lama untuk hitung kecepatan
                prevSampleX = receivedX;
                prevSampleTimestamp = lastSampleTimestamp;

                // Ambil data baru
                receivedX = _rawHeadX;
                currentRawTimestamp = _rawTimestamp;

                // Update Data Tangan untuk Cursor
                isHandDetected = _rawHandDetected;
                handPositionNorm = _rawHandPos;
                isFistDetected = _rawIsFist;
            }
        }

        // 2. JIKA ADA DATA BARU, UPDATE WAKTU UNITY DI SINI (AMAN)
        if (newDataArrived)
        {
            lastSampleTimestamp = currentRawTimestamp;
            lastReceivedTime = Time.realtimeSinceStartup;
        }

        // 3. LOGIKA GERAK KERANJANG / KEPALA
        float velocity = 0f;
        float dtSamples = lastSampleTimestamp - prevSampleTimestamp;

        if (dtSamples > 1e-4f)
            velocity = (receivedX - prevSampleX) / dtSamples;

        velocity = Mathf.Clamp(velocity, -maxVelocity, maxVelocity);

        float now = Time.realtimeSinceStartup;
        float networkDelay = Mathf.Max(0f, now - lastReceivedTime);

        // Prediksi posisi
        float predictedNormX = receivedX + velocity * networkDelay * predictionFactor;
        predictedNormX = Mathf.Clamp01(predictedNormX);

        // Gerakkan object (InputManager/Basket)
        float targetX = (predictedNormX - 0.5f) * 2f * horizontalRange;
        Vector3 currentPos = transform.position;
        Vector3 targetPos = new Vector3(targetX, currentPos.y, currentPos.z);
        transform.position = Vector3.SmoothDamp(currentPos, targetPos, ref smoothVel, smoothTime);
    }

    private void ReceiveLoop()
    {
        UdpClient client = null;
        try
        {
            client = new UdpClient(port);
            IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, port);

            while (running)
            {
                try
                {
                    byte[] data = client.Receive(ref anyIP); // Menunggu data masuk
                    string text = Encoding.UTF8.GetString(data).Trim();
                    string[] parts = text.Split(',');

                    // LOGGING (Thread Safe)
                    bool doLog = showDebugLogs && (DateTime.Now - lastLogTime).TotalSeconds > 1.0;
                    if (doLog)
                    {
                        Debug.Log($"[UDP] Data Masuk: {text}");
                        lastLogTime = DateTime.Now;
                    }

                    // --- PARSING DATA (6 KOLOM) ---
                    if (parts.Length >= 6)
                    {
                        if (float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float x) &&
                            float.TryParse(parts[5], NumberStyles.Float, CultureInfo.InvariantCulture, out float timestamp))
                        {
                            x = Mathf.Clamp01(x);
                            float hx = float.Parse(parts[2], CultureInfo.InvariantCulture);
                            float hy = float.Parse(parts[3], CultureInfo.InvariantCulture);
                            bool fist = (parts[4].Trim() == "1");

                            lock (lockObj)
                            {
                                // Simpan ke buffer (JANGAN panggil Time.* di sini)
                                _rawHeadX = x;
                                _rawTimestamp = timestamp;

                                if (hx <= -0.5f)
                                {
                                    _rawHandDetected = false;
                                }
                                else
                                {
                                    _rawHandDetected = true;
                                    _rawHandPos = new Vector2(1f - hx, 1f - hy);
                                }
                                _rawIsFist = fist;
                                _hasNewData = true; // Tandai ada data baru
                            }
                        }
                    }
                    else if (parts.Length >= 3 && doLog)
                    {
                        Debug.LogWarning("⚠️ Format Data Salah! Unity butuh 6 angka, menerima " + parts.Length);
                    }
                }
                catch (SocketException) { }
                catch (Exception e)
                {
                    Debug.LogWarning("UDP Thread Error: " + e.Message);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Gagal start UDP: " + e.Message);
        }
        finally
        {
            client?.Close();
        }
    }
}