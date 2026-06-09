using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class PlayerStatsManager : MonoBehaviour
{

    [Header("Setting")] [SerializeField] private float lerpSpeed;
    
    
    [Header("UI")]
    public Slider healthSlider;
    public Slider staminaSlider;
    public Slider easeStaminaSlider;
    public Slider easeHealthSlider;
    
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
        
        if (healthSlider != null)
        {
            healthSlider.maxValue = StatsData.maxHealth;
            healthSlider.minValue = StatsData.minHealth;
            currentHealth = StatsData.maxHealth;
        }
        if (staminaSlider != null)
        {
            staminaSlider.maxValue = StatsData.maxStamina;
            staminaSlider.minValue = StatsData.minStamina;
            easeStaminaSlider.maxValue = StatsData.maxStamina;
            easeStaminaSlider.value = StatsData.maxStamina;
            currentStamina = StatsData.maxStamina;
        }
        UpdateHealthSlider();
        UpdateStaminaSlider();
    }

    private void Update()
    {
       UpdateHealthSlider();
       UpdateStaminaSlider();
       CheckOutStamina();
    }


    void UpdateHealthSlider()
    {
        if (healthSlider.value != currentHealth)
        {
            healthSlider.value = currentHealth;
        }

        if (easeHealthSlider.value >= healthSlider.value)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, currentHealth, lerpSpeed * Time.deltaTime);
        }
        else if (easeHealthSlider.value <= healthSlider.value)
        {
            easeHealthSlider.value = currentHealth;
        }
    }
   
    void UpdateStaminaSlider()
    {
        if (staminaSlider.value != currentStamina)
        {
            staminaSlider.value = currentStamina;
        }

        if (easeStaminaSlider.value >= staminaSlider.value)
        {
            easeStaminaSlider.value = Mathf.Lerp(easeStaminaSlider.value, currentStamina, lerpSpeed * Time.deltaTime);;
        } 
        else if (easeStaminaSlider.value <= staminaSlider.value)
        {
            easeStaminaSlider.value = currentStamina;
        }
     
    }
    
    public void DecreaseHealth(float value)
    {
        if(currentHealth <=  StatsData.minHealth) return;
       currentHealth -= value;
    }
    public void DecreaseStamina(float value)
    {
        if(currentStamina <=  StatsData.minStamina) return ;
        currentStamina -= value;
    }
    public void IncreaseHealth(float value)
    {
        if(currentStamina >=  StatsData.maxHealth) return;
        currentHealth += value;
        UpdateHealthSlider();
    }
    public void IncreaseStamina(float value)
    {
       
        if(currentStamina >=  StatsData.maxStamina) return ;
        currentStamina += value;
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
