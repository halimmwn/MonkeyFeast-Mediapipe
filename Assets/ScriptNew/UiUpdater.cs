using UnityEngine;
using TMPro;

public class UIUpdater : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    void Update()
    {
        scoreText.text = "Score: " + GameManager.instance.score;
        timerText.text = "Time: " + Mathf.CeilToInt(GameManager.instance.timer);
    }
}
