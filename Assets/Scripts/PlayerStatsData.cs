using UnityEngine;

[CreateAssetMenu(menuName = "Player Stats Data")]
public class PlayerStatsData: ScriptableObject
{
    [Header("Health")]
    public float maxHealth;
    public float minHealth;
    public float hurtDame;
    
    [Header("Stamina")]
    public float maxStamina;
    public float minStamina;
    public float attackStamina;
    public float heavyAttackStamina;
    public float jumpStamina;
    public float dogdeStamina;
}
