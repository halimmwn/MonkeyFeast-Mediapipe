using UnityEngine;

public class CatchObject : MonoBehaviour
{
    public string tagName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Basket"))
        {
            ScoreManager.instance.OnCatch(tagName);
            Destroy(gameObject);
        }
    }
}
