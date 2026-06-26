using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public int coins = 0;

    public bool hasKey = false;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        Debug.Log("Heal: " + amount);
    }

    public void AddCoin(int amount) {
        coins += amount;
        Debug.Log("Coins: " + coins);
    }

    public void GetKey()
    {
        hasKey = true;
        Debug.Log("Key Collected");
    }
}
