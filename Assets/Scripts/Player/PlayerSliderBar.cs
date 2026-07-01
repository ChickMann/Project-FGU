using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class PlayerSliderBar : MonoBehaviour
{
    [Header("UI")]
    public SliderBar healthBar;
    public SliderBar staminaBar;
    
    [Header("Debug")]
    public float currentHealth { get; private set; }
    public float currentStamina {get; private set;}
    
    [Header("refs")]
    PlayerController _playerController;
    public PlayerStatsData StatsData;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    void Start()
    {
        currentHealth = StatsData.maxHealth;
        currentStamina = StatsData.maxStamina;

        if (healthBar != null)
        {
            healthBar.Initialize(StatsData.maxHealth, StatsData.minHealth, currentHealth);
        }
        if (staminaBar != null)
        {
            staminaBar.Initialize(StatsData.maxStamina, StatsData.minStamina, currentStamina);
        }
    }

    private void Update()
    {
       CheckOutStamina();
    }

    public void DecreaseHealth(float value)
    {
        if (currentHealth <= StatsData.minHealth) return;
        currentHealth -= value;
        if (healthBar != null) healthBar.UpdateValue(currentHealth);
    }
    public void DecreaseStamina(float value)
    {
        if (currentStamina <= StatsData.minStamina) return;
        currentStamina -= value;
        if (staminaBar != null) staminaBar.UpdateValue(currentStamina);
    }
    public void IncreaseHealth(float value)
    {
        if (currentHealth >= StatsData.maxHealth) return;
        currentHealth = Mathf.Min(currentHealth + value, StatsData.maxHealth);
        if (healthBar != null) healthBar.UpdateValue(currentHealth);
    }
    public void IncreaseStamina(float value)
    {
        if (currentStamina >= StatsData.maxStamina) return;
        currentStamina = Mathf.Min(currentStamina + value, StatsData.maxStamina);
        if (staminaBar != null) staminaBar.UpdateValue(currentStamina);
    }

    public void JumpStamina()
    {
         DecreaseStamina(StatsData.jumpStamina);
    }
    public void attackStamina()
    {
         DecreaseStamina(StatsData.attackStamina);
    }

    public void heavyattackStamina()
    {
         DecreaseStamina(StatsData.heavyAttackStamina);
    }

    public void dogdeStamina()
    {
        DecreaseStamina(StatsData.dogdeStamina);
    }

    public void hurtDame()
    {
        DecreaseHealth(StatsData.hurtDame);
    }

    public void HealthPotion()
    {
        IncreaseHealth(20);
    }

    public void punchStamina()
    {
        DecreaseStamina(StatsData.punchStamina);
    }
    public void CheckOutStamina()
    {
        if (IsOutStamina(StatsData.jumpStamina)) _playerController._jumpAction.action.Disable();
        else _playerController._jumpAction.action.Enable();
        
        if (IsOutStamina(StatsData.attackStamina)) _playerController._attackAction.action.Disable();
        else _playerController._attackAction.action.Enable();
        
        if (IsOutStamina(StatsData.heavyAttackStamina)) _playerController._heavyAttackAction.action.Disable();
        else _playerController._heavyAttackAction.action.Enable();
        
        if (IsOutStamina(StatsData.dogdeStamina)) _playerController._dogAction.action.Disable();
        else _playerController._dogAction.action.Enable();
        
        if (IsOutStamina(StatsData.punchStamina)) _playerController._punchAction.action.Disable();
        else _playerController._punchAction.action.Enable();
    } 
    public bool IsOutStamina(float value)
    {
        return value > currentStamina;
    }
}

