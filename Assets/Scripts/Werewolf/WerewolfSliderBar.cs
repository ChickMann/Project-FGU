using UnityEngine;

public class WerewolfSliderBar : MonoBehaviour
{
    public SliderBar healthBar;
    
    [Header("Debug")]
    public float currentHealth { get; private set; }
    
    [Header("status")]
    public float maxHealth;
    public float minHealth;
    void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.Initialize(maxHealth, minHealth, currentHealth);
        }
    }

    private void DecreaseHealth(float value)
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
        DecreaseHealth(20);
    }

    public bool IsDeath()
    {
        return currentHealth<= 0;
    }
}
