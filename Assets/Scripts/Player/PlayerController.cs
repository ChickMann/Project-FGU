using System;
using StateMachinePlayer;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[ RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    #region Variables & Properties

    [Header("Input references")]
    public InputActionReference _moveAction;
    public InputActionReference _runAction;
    public InputActionReference _crouchAction;
    public InputActionReference _jumpAction;
    public InputActionReference _dogAction;
    public InputActionReference _parryAction;
    public InputActionReference _attackAction;
    public InputActionReference _hurtAction;
    public InputActionReference _deathAction;
    public InputActionReference _punchAction;
    public InputActionReference _heavyAttackAction;
    
    [Header("sensors")]
    public Sensor_Prototype _groundSensor;
    public Sensor_Prototype _wallSensorR1;
    public Sensor_Prototype _wallSensorR2;
    public Sensor_Prototype _wallSensorL1;
    public Sensor_Prototype _wallSensorL2;
    
    [Header("Parry Effect")]
    public GameObject parryEffect;
    public GameObject transitionEffect;
    
    [Header("Landing Effect")]
    public GameObject landingEffect;
    public GameObject landingPos;
    
    [Header("Jump Effect")]
    public GameObject jumpEffect;

    [Header("Run stop Effect")]
    public GameObject runStopEffect;
    public GameObject runStopPos;
    
    [Header("Dodge Effect")]
    public GameObject dodgeEffect;
    public GameObject dodgePos;

    [Header("Player Stats")] 
    public PlayerData data;
    
    [Header("Buffer Time")]
    public float LastOnGroundTime { get; private set; }
    public float LastPressedJumpTime { get; private set; }
    public float LastPressedAttackTime { get; private set; }
    public float LastPressedPunchTime { get; private set; }
    public float LastPressedParryTime { get; private set; }
    public float LastPressedDogdeTime { get; private set; }
    public float LastPressedCrounchTime { get; private set; }
    private float _dogdeCooldownTime;
    private float _parryCooldownTime;
    private float _attackCooldownTime;
    private float _punchCooldownTime;
    
    public bool isRunning { get; private set; }
    public bool isCrouching { get; private set; }
    public bool isGrounding { get; private set; }
    public bool isFalling { get; private set; }
    public bool isJumpCut{ get; private set; }
    public bool isDodging { get; private set; }
    public bool isJumping { get; private set; }
    public bool isDeath { get; private set; }
    public bool isWallSliding { get; private set; }
    public bool isReversingDirection { get; private set; }
    public bool isHeavyAttack { get; private set; }
    public bool isFocus { get; private set; }
    
    public bool wasJumpPressed { get; private set; }
    public bool wasHurted { get; private set; }
    public bool wasHurtedHeavyAttack {get; private set;}
    public bool wasDodgePressed { get; private set; }
    public bool wasAttackPressed { get; private set; }
    public bool wasParryPressed { get; private set; }
    public bool wasPunchPresssed { get; private set; }
    public int successfulParryCount { get; private set; }
    
    public float _moveDirectionX { get; private set; }

    public int facingDirection { get; private set; }
    private float _lastNonZeroInputX = 1f;
    private float _dogdeTimeElapsed ;
    private float _timeHurtRecover;
    
    [Header("refs")]
    public Rigidbody2D _rigidbody { get; private set; }
    public PlayerSliderBar playerSliderBar;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        playerSliderBar = FindObjectOfType<PlayerSliderBar>();
    }

    private void Start()
    {
        _timeHurtRecover = data.timeRecover;
        SetGravityScale(data.gravityScale);
        facingDirection = 1;
    }

    private void OnEnable()
    {
        _moveAction.action.Enable();
        _runAction.action.Enable();
        _crouchAction.action.Enable();
        _jumpAction.action.Enable();
        _dogAction.action.Enable();
        _parryAction.action.Enable();
        _attackAction.action.Enable();
        _hurtAction.action.Enable();
    }

    private void OnDisable()
    {
        _moveAction.action.Disable();
        _runAction.action.Disable();
        _crouchAction.action.Disable();
        _jumpAction.action.Disable();
        _dogAction.action.Disable();
        _parryAction.action.Disable();
        _attackAction.action.Disable();
        _hurtAction.action.Disable();
    }

    private void Update()
    {
        LastPressedJumpTime -= Time.deltaTime;
        LastOnGroundTime -= Time.deltaTime;
        LastPressedAttackTime -= Time.deltaTime;
        LastPressedPunchTime -= Time.deltaTime;
        LastPressedDogdeTime -= Time.deltaTime;
        LastPressedParryTime -= Time.deltaTime;

        isFocus = _heavyAttackAction.action.IsPressed();
        isHeavyAttack = _heavyAttackAction.action.WasReleasedThisFrame();
        _moveDirectionX = _moveAction.action.ReadValue<Vector2>().x;
        isRunning =  _runAction.action.IsPressed();
        isCrouching = _crouchAction.action.IsPressed();
        wasDodgePressed = OnDodgeInput();
        isGrounding = IsGrounded();
        wasParryPressed = OnParryInput();
        wasJumpPressed = OnJumpInput();
        wasAttackPressed = OnAttackInput();
        isFalling = IsFalling();
        isWallSliding = IsWallSliding();
        isReversingDirection = _moveDirectionX * _lastNonZeroInputX < 0f;
        if (_moveDirectionX != 0)
        {
            _lastNonZeroInputX = _moveDirectionX;
        }

        wasPunchPresssed = OnPunchInput();

        if (isFalling) isJumping = false;
        JumpCut();
        RecoverHurt();
        if(playerSliderBar.currentHealth <=0)
        {
            isDeath = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Hit Box") && !wasHurted)
        {
             if (other.GetComponent<HitBox>().isHeavyAttack)
            {
                wasHurtedHeavyAttack = true;
                wasHurted = true;
            }
            else
            {
                wasHurted = true;
            }

            // Werewolf V2 life steal logic (recovers 2% max health on hit)
            var werewolf = other.GetComponentInParent<WerewolfMovement>();
            if (werewolf != null && werewolf.wasTransitionedToV2)
            {
                float healAmount = werewolf.slider.maxHealth * 0.02f;
                werewolf.slider.IncreaseHealth(healAmount);
            }
        }
    }

    #endregion

    #region Movement & Physics

    public void Moving(bool isMoving = true)
    {
        float targetSpeed = isMoving ?  (isRunning ? _moveDirectionX * data.runMaxSpeed : _moveDirectionX * data.walkMaxSpeed):0f;
    
        bool hasMoveInput = Mathf.Abs(targetSpeed) > 0.01f;

        float accelRate = (isGrounded: isGrounding, hasMoveInput) switch
        {
            (true, true)   => isRunning ? data.runAccelAmount : data.walkAccelAmount,
            (true, false)  => data.runDeccelAmount,
            (false, true)  => data.runAccelAmount * data.accelInAir,
            (false, false) => data.runDeccelAmount * data.deccelInAir
        };

        bool isOverspeeding = Mathf.Abs(_rigidbody.linearVelocity.x) > Mathf.Abs(targetSpeed) 
                              && (_rigidbody.linearVelocity.x * targetSpeed) > 0f;

        if (data.doConserveMomentum && isOverspeeding && hasMoveInput && !isGrounding && !isMoving)
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
    }

    public bool IsWallSliding()
    {
        return _wallSensorR1.State();
    }

    public void WallSliding()
    {
        float slideSpeed = isCrouching ? data.slideSpeedFaster : _jumpAction.action.IsPressed()? data.slideSpeedLower : data.slideSpeed;
        
        float speedDif = slideSpeed - _rigidbody.linearVelocity.y;	
        Debug.Log("Wall sliding: " + _rigidbody.linearVelocity.y + " " + speedDif );
        float movement = speedDif * data.slideAccel;
        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif)  * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));
        _rigidbody.AddForce(movement * Vector2.up);
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
            
                _rigidbody.linearVelocity = new Vector2(facingDirection * currentSpeed, _rigidbody.linearVelocity.y);
                _dogdeTimeElapsed += Time.fixedDeltaTime;
            }
            else 
            {
                _dogdeTimeElapsed = 0f;
                isDodging = false;
            }
        }
    }

    public void Jumping()
    {
        if (isGrounding && wasJumpPressed )
        {
            LastPressedJumpTime = 0;
            LastOnGroundTime = 0;
            isJumpCut = false;
            Instantiate(jumpEffect, landingPos.transform.position, Quaternion.identity);
            float force = data.jumpForce;
            if (_rigidbody.linearVelocity.y < 0)
                force -= _rigidbody.linearVelocity.y;
        
            _rigidbody.AddForce(Vector2.up * force, ForceMode2D.Impulse);
            isFalling = false;
            _groundSensor.Disable(0.2f);
        }
    }

    public bool IsFalling()
    {
        return _rigidbody.linearVelocity.y<0 ;
    }

    public void Turn(int direction)
    {
        Vector3 scale = transform.localScale; 
        scale.x *= -1;
        transform.localScale = scale;
        facingDirection = direction;
    }

    public void CheckDirectionToFace()
    {
        if (_moveDirectionX != 0)
        {
            int isMovingRight = _moveDirectionX > 0 ? 1 : -1;
            if (isMovingRight != facingDirection)
                Turn(isMovingRight);
        }
    }

    public void SetGravityScale(float scale)
    {
        _rigidbody.gravityScale = scale;
    }

    public void JumpCut()
    {
        if (_rigidbody.linearVelocity.y > 0 && _jumpAction.action.WasReleasedThisFrame())
            isJumpCut = true;
    }

    public void Gravity()
    {
        if (_rigidbody.linearVelocity.y < 0 &&isCrouching)
        {
            SetGravityScale(data.gravityScale * data.fastFallGravityMult);
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, Mathf.Max(_rigidbody.linearVelocity.y, -data.maxFastFallSpeed));
        }
        else if (isJumpCut)
        {
            SetGravityScale(data.gravityScale * data.jumpCutGravityMult);
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, Mathf.Max(_rigidbody.linearVelocity.y, -data.maxFallSpeed));
        }
        else if ((wasJumpPressed || isFalling) && Mathf.Abs(_rigidbody.linearVelocity.y) < data.jumpHangTimeThreshold)
        {
            SetGravityScale(data.gravityScale * data.jumpHangGravityMult);
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, Mathf.Max(_rigidbody.linearVelocity.y, -data.maxFallSpeed));
        }
        else if (_rigidbody.linearVelocity.y < 0)
        {
            SetGravityScale(data.gravityScale * data.fallGravityMult);
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, Mathf.Max(_rigidbody.linearVelocity.y, -data.maxFallSpeed));
        }
        else
        {
            SetGravityScale(data.gravityScale);
        }
    }

    #endregion

    #region Input Handlers

    public bool OnAttackInput()
    {
        _attackCooldownTime -= Time.deltaTime;
        if (_attackAction.action.WasPressedThisFrame() && _attackCooldownTime <=0)
        {
            LastPressedAttackTime = data.AttackInputBufferTime;
            _attackCooldownTime = data.attackCooldownTime;
        }
        return LastPressedAttackTime > 0;
    }
  
    public bool OnPunchInput()
    {
        _punchCooldownTime -= Time.deltaTime;
        if (_punchAction.action.WasPressedThisFrame() && _punchCooldownTime <=0)
        {
            LastPressedPunchTime = data.AttackInputBufferTime;
            _punchCooldownTime = data.PunchCooldownTime;
        }
        return LastPressedPunchTime > 0;
    }

    public bool OnJumpInput()
    {
        if (wasJumpPressed && _rigidbody.linearVelocity.y < 0)
        {
            return false;
        }

        if (_jumpAction.action.WasPressedThisFrame())
        {
            LastPressedJumpTime = data.jumpInputBufferTime;
            isJumping = true;
        }
        return LastPressedJumpTime > 0 ;
    }

    public bool OnParryInput()
    {
        _parryCooldownTime -= Time.deltaTime;

        if (_parryAction.action.WasPressedThisFrame() && _parryCooldownTime <= 0)
        {
            LastPressedParryTime = data.ParryInputBufferTime;
            _parryCooldownTime = data.parryCooldownTime;
        }
        return LastPressedParryTime > 0;
    }

    public bool OnDodgeInput()
    {
        _dogdeCooldownTime -= Time.deltaTime;
        if (_dogAction.action.WasPressedThisFrame() && _dogdeCooldownTime <= 0)
        {
            LastPressedDogdeTime = data.DodgeInputBufferTime;
            _dogdeCooldownTime = data.dogdeCooldownTime;
            _dogdeTimeElapsed = 0;
            isDodging = true;
        }
        return LastPressedDogdeTime > 0 ;
    }

    #endregion

    #region Combat & Actions

    public void Parrying()
    {
        successfulParryCount++;
        _parryCooldownTime = data.parryMulCooldownTime;
        _rigidbody.linearVelocity = new Vector2(-facingDirection * data.parryForce, 0);
        if (parryEffect != null ) Instantiate(parryEffect, transitionEffect.transform.position, Quaternion.identity);
    }

    public void ResetSuccessfulParryCount()
    {
        successfulParryCount = 0;
    }

    public void Hurting()
    {
        float force = data.hurtForce;
        if(wasHurtedHeavyAttack) force *=2;
        _rigidbody.linearVelocity = new Vector2(-facingDirection * force, _rigidbody.linearVelocity.y);
    }

    public void Die()
    {
        _rigidbody.linearVelocity = new Vector2(0,_rigidbody.linearVelocity.y);
    }

    private void RecoverHurt()
    {
        if(!wasHurted && !wasHurtedHeavyAttack) return;
        _timeHurtRecover -= Time.deltaTime;
        if (_timeHurtRecover <= 0)
        {
            _timeHurtRecover = data.timeRecover;
            wasHurted = false;
            wasHurtedHeavyAttack = false;
        }
    }

    #endregion

    #region Visual Effects

    public void DodgeEffect()
    {
        Instantiate(dodgeEffect, dodgePos.transform.position, Quaternion.identity);
    }

    public void Landing()
    {
        if(landingEffect != null && isGrounding) Instantiate(landingEffect, landingPos.transform.position, Quaternion.identity);
    }
    
    public void RunStop()
    {
        if (runStopEffect != null )
        {
            GameObject ef;
            ef = Instantiate(runStopEffect, runStopPos.transform.position, Quaternion.identity);
            if (wasParryPressed && facingDirection >0)
            {
                ef.transform.position += new Vector3(0.2f, 0,0);
            } else if (wasParryPressed && facingDirection < 0)
            {
                ef.transform.position -=new Vector3(0.2f, 0,0);
            }
            Vector3 scale = ef.transform.localScale; 
            scale.x *= facingDirection;
            ef.transform.localScale = scale;
        }
    }

    public void AttackEffect()
    {
        _rigidbody.linearVelocity = new Vector2(facingDirection * data.attackForce, 0);
    }

    #endregion

    #region Ground Checks

    private bool IsGrounded()
    {
        if (_groundSensor.State())
        {
            LastOnGroundTime = data.coyoteTime;
            isJumpCut = false;
        }
        return LastOnGroundTime > 0;
    }

    #endregion

    #region Helpers & Setters

    public void SetParryColdown()
    {
        _parryCooldownTime = data.parryCooldownTime;
    }

    #endregion
}
