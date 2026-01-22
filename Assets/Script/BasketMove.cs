using UnityEngine;

public class BasketMove : MonoBehaviour
{
    public float speed = 7f;

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");   // A D / Arrow Left Right
        float moveY = Input.GetAxis("Vertical");     // W S / Arrow Up Down

        Vector3 movement = new Vector3(moveX, moveY, 0) * speed * Time.deltaTime;

        transform.Translate(movement);
    }
}
