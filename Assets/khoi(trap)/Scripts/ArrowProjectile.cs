using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    public float speed = 8f;
    public float lifeTime = 3f;
    public Vector2 direction = Vector2.right;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(direction.normalized * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Đụng player thì mũi tên biến mất
        // Player của bạn sẽ tự respawn nếu đã xử lý Tag Obstacle
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
            return;
        }

        // Đụng tường / platform / ground thì biến mất
        if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}