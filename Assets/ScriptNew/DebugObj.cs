using UnityEngine;

public class DataDebugger : MonoBehaviour
{
    public HeadReceiver receiver; // Drag script HeadReceiver ke sini

    void OnGUI()
    {
        // Bikin style tulisan biar gede dan kuning
        GUIStyle style = new GUIStyle();
        style.fontSize = 40;
        style.normal.textColor = Color.yellow;
        style.fontStyle = FontStyle.Bold;

        // Siapkan pesan debug
        string info = "--- DATA DEBUGGER ---\n";

        if (receiver == null)
        {
            info += "ERROR: HeadReceiver belum dipasang di Inspector!";
            style.normal.textColor = Color.red;
        }
        else
        {
            info += $"Hand Detected: {receiver.isHandDetected}\n";
            info += $"Hand Position: {receiver.handPositionNorm.ToString("F2")}\n";
            info += $"Fist (Kepal): {receiver.isFistDetected}\n";
            info += $"Scene Saat Ini: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}\n";
        }

        // Tampilkan di pojok kiri atas layar
        GUI.Label(new Rect(50, 50, 1000, 500), info, style);
    }
}