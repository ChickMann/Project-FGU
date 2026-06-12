using UnityEngine;

public class PlayerHealth: MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int curentHealth;

    void Start()
    {
        curentHealth = maxHealth;
    }

    // Nhận sát thương
    public void TakeDame(int dame)
    {
        curentHealth -= dame;
        if (curentHealth < 0)
        {
            curentHealth = 0;
        }
        Debug.Log("Current HP: " + curentHealth);

        if(curentHealth == 0)
        {
            Die();
        }
    }

    //Hồi máu
    public void Heal(int amount)
    {
        curentHealth += amount;
        if (curentHealth > maxHealth)
        {
            curentHealth = maxHealth;
        }
        Debug.Log("Current HP: " + curentHealth);
    }

    private void Die()
    {
        Debug.Log("Player Died!");
        // Tạm thời chỉ log ra console
        // Sau này có thể thêm hiệu ứng chết, respawn, v.v.
        // Animation chết
        // Game Over
    }
}
