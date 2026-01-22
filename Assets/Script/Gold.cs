using UnityEngine;

public class Gold : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Basket"))
        {
            GameManager.instance.AddScore(5);
            Destroy(gameObject);
        }
    }
}