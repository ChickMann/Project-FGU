using UnityEngine;

public class ArrowTrapShooter : MonoBehaviour
{
    public GameObject arrowPrefab;
    public Transform firePoint;
    public float shootInterval = 2f;
    public Vector2 shootDirection = Vector2.right;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= shootInterval)
        {
            Shoot();
            timer = 0f;
        }
    }

    void Shoot()
    {
        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);

        ArrowProjectile projectile = arrow.GetComponent<ArrowProjectile>();

        if (projectile != null)
        {
            projectile.direction = shootDirection.normalized;
        }
    }
}