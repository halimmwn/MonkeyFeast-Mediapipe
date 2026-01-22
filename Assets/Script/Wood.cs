using UnityEngine;

public class Wood : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Basket"))
        {
            GameManager.instance.AddScore(-3);
            Destroy(gameObject);
        }
    }
}
