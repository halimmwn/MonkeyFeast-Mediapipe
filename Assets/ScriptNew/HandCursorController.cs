using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class HandCursorController : MonoBehaviour
{
    [Header("References")]
    public HeadReceiver headReceiver;
    public Image cursorImage;
    public RectTransform cursorRect;

    [Header("Visual Settings")]
    public Color normalColor = new Color(1, 1, 1, 0.8f);
    public Color clickColor = new Color(1, 0, 0, 1f);
    public float smoothing = 10f;

    [Header("Movement Settings")] // --- FITUR BARU: MIRRORING ---
    public bool mirrorHorizontal = true; // Centang ini jika gerak Kiri-Kanan terbalik
    public bool mirrorVertical = false;   // Centang ini jika gerak Atas-Bawah terbalik

    [Header("Game State")]
    public string mainMenuSceneName = "SampleScene";

    private bool isClicking = false;

    void Start()
    {
        if (cursorImage == null) cursorImage = GetComponent<Image>();
        if (cursorRect == null) cursorRect = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (headReceiver == null) return;

        // 1. CEK SCENE & GAME OVER
        bool isMainMenu = SceneManager.GetActiveScene().name == mainMenuSceneName;

        bool isGameOver = false;
        if (GameManager.instance != null)
        {
            isGameOver = GameManager.instance.isGameOver;
        }

        bool shouldShow = (isMainMenu || isGameOver) && headReceiver.isHandDetected;

        if (!shouldShow)
        {
            ShowCursor(false);
            return;
        }

        ShowCursor(true);

        // 2. GERAKAN KURSOR (DENGAN LOGIKA MIRROR)
        // Ambil posisi raw 0..1
        float posX = headReceiver.handPositionNorm.x;
        float posY = headReceiver.handPositionNorm.y;

        // Terapkan Mirror jika dicentang di Inspector
        if (mirrorHorizontal) posX = 1f - posX;
        if (mirrorVertical) posY = 1f - posY;

        Vector2 targetPos = new Vector2(
            posX * Screen.width,
            posY * Screen.height
        );

        cursorRect.position = Vector2.Lerp(cursorRect.position, targetPos, Time.unscaledDeltaTime * smoothing);

        // 3. LOGIKA KLIK
        if (headReceiver.isFistDetected)
        {
            cursorImage.color = clickColor;
            if (!isClicking)
            {
                ClickUI();
                isClicking = true;
            }
        }
        else
        {
            cursorImage.color = normalColor;
            isClicking = false;
        }
    }

    void ShowCursor(bool show)
    {
        if (cursorImage != null)
        {
            cursorImage.enabled = show;
        }
    }

    void ClickUI()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = cursorRect.position;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            Button btn = result.gameObject.GetComponent<Button>();
            if (btn == null) btn = result.gameObject.GetComponentInParent<Button>();

            if (btn != null && btn.interactable)
            {
                btn.onClick.Invoke();
                Debug.Log("Hand Click: " + btn.name);
                return;
            }
        }
    }
}