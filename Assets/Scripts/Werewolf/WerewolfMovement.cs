using System;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using Random = UnityEngine.Random;

public class WerewolfMovement : MonoBehaviour
{
    #region Variables & Properties

    [Header("References")]
    public WerewolfData data;
    public GameObject target;
    public WerewolfSliderBar slider;
    
    [Header("sensors")]
    public Sensor_Prototype _groundSensor;
    private Rigidbody2D _rigidbody;
    
    [Header("movement")]
    public bool isFalling{ get; private set; }
    public bool isGrounding { get; private set; }
    public bool isWalking;
    public bool isRunning;
    public bool isJumping;
    public bool isHurtHeavyAttack;
    public bool isHurting;
    public bool isDodging;
    public bool isDeath;

    [Header("Action")] 
    public bool isAttack1;
    public bool isAttack2;
    public bool isAttack3;
    public bool isAttack4;
    public bool isAttack5;

    [Header("event")] 
    public bool isVer2;
    public bool wasTransitionedToV2;
    [Header("mm_effect")] 
    public MMF_Player transver2Feedback;
    public MMF_Player deadFeedback;

    [Header("Debug")]
     public float timeRecover;
    public int consecutiveHurtCount;


    public int facingDirection;
    private float _dogdeTimeElapsed ;
    private float _timeHurtRecover;


    [Header("Attack Cooldowns")]
    public float attack2CooldownTimer { get; private set; }
    public float attack3CooldownTimer { get; private set; }
    public int parryThreshold = 3;

    private PlayerController _playerController;
    private HitBox[] _hitboxes;
    private int _lastAttackType = 0;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        slider = GetComponent<WerewolfSliderBar>();
    }

    void Start()
    {
        SetGravityScale(data.gravityScale);
        facingDirection = 1;
        target = GameObject.FindGameObjectWithTag("Player");
        if (target != null)
        {
            _playerController = target.GetComponent<PlayerController>();
        }
        _hitboxes = GetComponentsInChildren<HitBox>(true);
        SetHeavyAttack(false);
        WalkInput();
    }

    void Update()
    {
        isGrounding = _groundSensor.State();
        RecoverHurt();
        UpdateCooldowns();
        if(slider.IsDeath()) {isDeath = true;}
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Hit Box") && !isHurting)
        {
            if(slider.currentHealth <=0)
            {
                isDeath = true;
            }
            else if (other.GetComponent<HitBox>().isHeavyAttack)
            {
                isHurtHeavyAttack = true;
                isHurting = true;
            }
            else
            {
                isHurting = true;
            }
        }
    }

    #endregion

    #region Movement & Physics

    public bool MovingToTarget( float stopDistance = 0.1f)
    {
        float targetSpeed = 0f;
        bool hasMoveInput = false;
        if (target != null)
        {
            float distanceToTarget = target.transform.position.x - transform.position.x;
            int moveDirX = facingDirection * -1;
            if (Mathf.Abs(distanceToTarget) > stopDistance)
            {
                hasMoveInput = true;
                if (isWalking) targetSpeed = moveDirX * data.walkAcceleration;
                else if(isRunning) targetSpeed = moveDirX * data.runAcceleration;
            }
            else
            {
              return false;
            }
        }

        float accelRate = (isGrounded: isGrounding, hasMoveInput) switch
        {
            (true, true)   => isRunning ? data.runAccelAmount : data.walkAccelAmount,
            (true, false)  => data.runDeccelAmount,
            (false, true)  => data.runAccelAmount * data.accelInAir,
            (false, false) => data.runDeccelAmount * data.deccelInAir
        };

        bool isOverspeeding = Mathf.Abs(_rigidbody.linearVelocity.x) > Mathf.Abs(targetSpeed) 
                              && (_rigidbody.linearVelocity.x * targetSpeed) > 0f;

        if (data.doConserveMomentum && isOverspeeding && hasMoveInput && !isGrounding )
        {
            accelRate = 0f; 
        }
        if ((isJumping || isFalling) && Mathf.Abs(_rigidbody.linearVelocity.y) < data.jumpHangTimeThreshold)
        {
            accelRate *= data.jumpHangAccelerationMult;
            targetSpeed *= data.jumpHangMaxSpeedMult;
        }

        float speedDif = targetSpeed - _rigidbody.linearVelocity.x;
        float movement = speedDif * accelRate;
        _rigidbody.AddForce(movement * Vector2.right, ForceMode2D.Force);
        return true;
    }

    public void Jumping()
    {
        if (isGrounding )
        {
            DisableSensor();
            float force = data.jumpForce;
            if (_rigidbody.linearVelocity.y < 0)
                force -= _rigidbody.linearVelocity.y;
        
            _rigidbody.AddForce(Vector2.up * force, ForceMode2D.Impulse);
            isFalling = false;
            _groundSensor.Disable(0.2f);
        }
    }

    public void Dogding()
    {
        if (isGrounding && isDodging)
        {
            if (_dogdeTimeElapsed < data.dogdeTime)
            {
                float startVelocity = (2f * data.dogdeDistance) / data.dogdeTime;
                float normalizedTime = _dogdeTimeElapsed / data.dogdeTime;
                float currentSpeed = Mathf.Lerp(startVelocity, 0f, normalizedTime);

                _rigidbody.AddForce(data.dogdeForce * new Vector2(-facingDirection  * currentSpeed, _rigidbody.linearVelocity.y), ForceMode2D.Impulse);
                _dogdeTimeElapsed += Time.fixedDeltaTime;
            }
            else
            {
                _dogdeTimeElapsed = 0f;
                isDodging = false;
            }
        }
    }

    public void SetGravityScale(float scale)
    {
        _rigidbody.gravityScale = scale;
    }

    public void CheckDirectionToFace()
    {
        if (target == null) return;
        Vector2 directionToTarget = (target.transform.position - transform.position).normalized;
        int targetDirection = directionToTarget.x >= 0 ? -1 : 1;

        if (targetDirection != facingDirection)
        {
            Turn(targetDirection);
        }
    }

    public void Turn(int direction)
    {
        Vector3 scale = transform.localScale; 
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;
        facingDirection = direction;
    }

    #endregion

    #region Attack & Combat Logic

    public void ChooseNextAttack()
    {
        isAttack1 = false;
        isAttack2 = false;
        isAttack3 = false;
        isAttack4 = false;
        isAttack5 = false;
        isJumping = false;
        isDodging = false;

        if (wasTransitionedToV2)
        {
            if (!isVer2)
            {
                if (Random.value < 0.3f)
                {
                    isVer2 = true;
                    RunInput();
                }
            }
        }

        if (isVer2)
        {
            int randomVal = Random.Range(0, 2);
            if (randomVal == 0)
            {
                isAttack4 = true;
                isJumping = true;
            }
            else
            {
                isAttack5 = true;
                isDodging = true;
            }
        }
        else
        {
            if (_playerController != null && _playerController.successfulParryCount >= parryThreshold && isAttack2Ready())
            {
                Atack2Input();
                _playerController.ResetSuccessfulParryCount();
                return;
            }

            List<int> candidates = new List<int>();
            
            if (_lastAttackType != 1)
            {
                candidates.Add(1);
            }
            if (isAttack2Ready() && _lastAttackType != 2)
            {
                candidates.Add(2);
            }
            if (isAttack3Ready() && _lastAttackType != 3)
            {
                candidates.Add(3);
            }

            if (candidates.Count == 0)
            {
                candidates.Add(1);
            }

            int chosenAttack = candidates[Random.Range(0, candidates.Count)];
            
            if (chosenAttack == 1)
            {
                Attack1Input();
            }
            else if (chosenAttack == 2)
            {
                Atack2Input();
            }
            else if (chosenAttack == 3)
            {
                Atack3Input();
            }
        }
    }

    public void RandomAttackV1()
    {
        ChooseNextAttack();
    }

    public void RandomAttackV2()
    {
        ChooseNextAttack();
    }

    public void AttackEffect()
    {
        if (!isPlayerNear(1f))
        {
            _rigidbody.linearVelocity = new Vector2(facingDirection * -1 * data.attackForce, 0);
        }
    }

    public bool isPlayerNear(float minDistance = 1.5f)
    {
        float distanceToTarget = target.transform.position.x - transform.position.x;
        if(minDistance >= Mathf.Abs(distanceToTarget)) return true;
        return false;
    }

    public void SetHeavyAttack(bool isHeavy)
    {
        if (_hitboxes != null)
        {
            foreach (var hb in _hitboxes)
            {
                if (hb != null)
                {
                    hb.isHeavyAttack = isHeavy;
                }
            }
        }
    }

    #endregion

    #region Attack Cooldowns

    private void UpdateCooldowns()
    {
        if (attack2CooldownTimer > 0)
        {
            attack2CooldownTimer -= Time.deltaTime;
        }
        if (attack3CooldownTimer > 0)
        {
            attack3CooldownTimer -= Time.deltaTime;
        }
    }

    public bool isAttack2Ready()
    {
        return attack2CooldownTimer <= 0;
    }

    public bool isAttack3Ready()
    {
        return attack3CooldownTimer <= 0;
    }

    public void StartAttack2Cooldown()
    {
        attack2CooldownTimer = data.attack2Coldown;
    }

    public void StartAttack3Cooldown()
    {
        attack3CooldownTimer = data.attack3Coldown;
    }

    #endregion

    #region Input Handlers

    public void Attack1Input()
    {
        isAttack1 = true;
        isAttack2 = false;
        isAttack3 = false;
        isAttack4 = false;
        isAttack5 = false;
        _lastAttackType = 1;
    }

    public void Atack2Input()
    {
        isAttack1 = false;
        isAttack2 = true;
        isAttack3 = false;
        isAttack4 = false;
        isAttack5 = false;
        _lastAttackType = 2;
    }

    public void Atack3Input()
    {
        isAttack1 = false;
        isAttack2 = false;
        isAttack3 = true;
        isAttack4 = false;
        isAttack5 = false;
        _lastAttackType = 3;
    }

    public void WalkInput()
    {
        isWalking = true;
        isRunning = false;
    }

    public void RunInput()
    {
        isRunning = true;
        isWalking = false;
    }
    

    #endregion

    #region Sensors

    public void DisableSensor()
    {
        _groundSensor.gameObject.SetActive(false);
    }

    public void EnableSensor()
    {
        _groundSensor.gameObject.SetActive(true);
    }

    #endregion

    #region Hurt & Death

    private void RecoverHurt()
    {
        if(!isHurting && !isHurtHeavyAttack) return;
        _timeHurtRecover -= Time.deltaTime;
        if (_timeHurtRecover <= 0)
        {
            _timeHurtRecover = timeRecover;
            isHurting = false;
            isHurtHeavyAttack = false;
        }
    }

    public void Hurting()
    {
        float force = data.hurtForce;
        if(isHurtHeavyAttack) force *=2;
        _rigidbody.linearVelocity = new Vector2(facingDirection * force, _rigidbody.linearVelocity.y);
       
    }


    public void ClearHurt()
    {
        isHurting = false;
        isHurtHeavyAttack = false;
        _timeHurtRecover = timeRecover;
    }

    public void ResetConsecutiveHurt()
    {
        consecutiveHurtCount = 0;
    }

    #endregion
}
