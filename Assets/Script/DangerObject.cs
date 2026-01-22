using UnityEngine;

public class Danger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Basket"))
        {
            GameManager.instance.ReduceLife();
            Destroy(gameObject);
        }
    }
}
