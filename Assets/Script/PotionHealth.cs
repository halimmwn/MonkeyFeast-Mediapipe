using UnityEngine;

public class PotionHealth : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Basket"))
        {
            GameManager.instance.RestoreLife();
            Destroy(gameObject);
        }
    }
}
