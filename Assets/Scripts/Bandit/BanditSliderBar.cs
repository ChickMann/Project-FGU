using UnityEngine;

public class BanditSliderBar : MonoBehaviour
{
    public SliderBar healthBar;

    [Header("Debug")]
    public float currentHealth { get; private set; }

    [Header("Status")]
    public float maxHealth = 60f;
    public float minHealth = 0f;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.Initialize(maxHealth, minHealth, currentHealth);
        }
    }

    public void DecreaseHealth(float value)
    {
        if (currentHealth <= minHealth) return;
        currentHealth -= value;
        if (healthBar != null) healthBar.UpdateValue(currentHealth);
    }

    public void IncreaseHealth(float value)
    {
        if (currentHealth >= maxHealth) return;
        currentHealth = Mathf.Min(currentHealth + value, maxHealth);
        if (healthBar != null) healthBar.UpdateValue(currentHealth);
    }

    [ContextMenu("testdame")]
    public void TakeDamage()
    {
        DecreaseHealth(20f);
    }

    public void TakeDamage(float damage)
    {
        DecreaseHealth(damage);
    }

    public bool IsDeath()
    {
        return currentHealth <= minHealth;
    }
}
