using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public Transform respawnPoint;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            Respawn();
        }
    }

    void Respawn()
    {
        transform.position = respawnPoint.position;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}