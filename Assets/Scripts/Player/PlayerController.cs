using System;
using MoreMountains.Feedbacks;
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
    public InputActionReference _punchAction;
    public InputActionReference _heavyAttackAction;
    
    [Header("MM_Effect")]
    public PlayerFeedbackManager  playerFeedbackManager {get; private set;}
    
    [Header("Sensors")]
    public PlayerSensorManager playerSensorManager;
  

    [Header("Player Stats")] 
    public PlayerData data;
    
    [Header("Buffer Time")]
    public float LastOnGroundTime { get; private set; }
    public float LastPressedJumpTime { get; private set; }
    public float LastPressedAttackTime { get; private set; }
    public float LastPressedPunchTime { get; private set; }
    public float LastPressedParryTime { get; private set; }
    public float LastPressedDogdeTime { get; private set; }
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
    public bool isGrabbing { get; private set; }
    
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
    public Vector3 climbPosition { get; set; }
    public LayerMask LayerMaskGrab;
    
    [Header("refs")]
    public Rigidbody2D _rigidbody { get; private set; }
    public PlayerSliderBar playerSliderBar;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        playerSliderBar = GetComponent<PlayerSliderBar>();
        playerFeedbackManager   = GetComponent<PlayerFeedbackManager>();
        
        if (playerSensorManager == null)
            playerSensorManager = GetComponent<PlayerSensorManager>();
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
        LastPressedParryTime -= Time.unscaledDeltaTime;

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
        wasPunchPresssed = OnPunchInput();
        
        isReversingDirection = _moveDirectionX * _lastNonZeroInputX < 0f;
        if (_moveDirectionX != 0)
        {
            _lastNonZeroInputX = _moveDirectionX;
        }


        if (isFalling) isJumping = false;
        JumpCut();
        RecoverHurt();
        if(playerSliderBar.currentHealth <=0)
        {
            isDeath = true;
        }
        isGrabbing = IsGrabbing();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Hit Box") && !IsInvincible())
        {
             if (other.GetComponent<HitBox>().isHeavyAttack )
            {
                wasHurtedHeavyAttack = true;
                wasHurted = true;
                _timeHurtRecover = data.timeRecover;
            }
            else
            {
                wasHurted = true;
                _timeHurtRecover = data.timeRecover;
            }

            // recovers 2% max health on hit
            var werewolf = other.GetComponentInParent<WerewolfMovement>();
            if (werewolf != null && werewolf.wasTransitionedToV2)
            {
                float healAmount = werewolf.slider.maxHealth * 0.02f;
                werewolf.slider.IncreaseHealth(healAmount);
            }
        }
        else if (other.gameObject.CompareTag("Health potion") && playerSliderBar.currentHealth<=playerSliderBar.StatsData.maxHealth-20 )
        {
            playerFeedbackManager.PlayFeedback(PlayerFeedbackType.Health);
            playerSliderBar.HealthPotion();
            Destroy(other.gameObject);
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
        if (playerSensorManager == null || playerSensorManager.wallSensorR1 == null) return false;
        return playerSensorManager.wallSensorR1.State();
    }

    public void WallSliding()
    {
        
        float slideSpeed = isCrouching ? data.slideSpeedFaster : _jumpAction.action.IsPressed()? data.slideSpeedLower : data.slideSpeed;
        
        float speedDif = slideSpeed - _rigidbody.linearVelocity.y;	
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

    private bool IsGrabbing()
    {
        if (playerSensorManager == null) return false;
        return playerSensorManager.wallSensorR2.State() || playerSensorManager.wallSensorR1.State() || playerSensorManager.wallSensorL2.State() || playerSensorManager.wallSensorL1.State();
    }

    public void Jumping()
    {
        if (isGrounding && wasJumpPressed )
        {
            LastPressedJumpTime = 0;
            LastOnGroundTime = 0;
            isJumpCut = false;
            float force = data.jumpForce;
            if (_rigidbody.linearVelocity.y < 0)
                force -= _rigidbody.linearVelocity.y;
        
            _rigidbody.AddForce(Vector2.up * force, ForceMode2D.Impulse);
            isFalling = false;
            if (playerSensorManager != null) playerSensorManager.groundSensor.Disable(0.2f);
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
        _attackCooldownTime -= Time.timeScale;
        if (_attackAction.action.WasPressedThisFrame() && _attackCooldownTime <=0)
        {
            LastPressedAttackTime = data.AttackInputBufferTime;
            _attackCooldownTime = data.attackCooldownTime;
        }
        return LastPressedAttackTime > 0;
    }
  
    public bool OnPunchInput()
    {
        _punchCooldownTime -= Time.unscaledDeltaTime;
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
        _parryCooldownTime -= Time.unscaledDeltaTime;

        if (_parryAction.action.WasPressedThisDynamicUpdate() && _parryCooldownTime <= 0)
        {
            LastPressedParryTime = data.ParryInputBufferTime;
            _parryCooldownTime = data.parryCooldownTime;
        }
        return LastPressedParryTime > 0;
    }

    public bool OnDodgeInput()
    {
        _dogdeCooldownTime -= Time.unscaledDeltaTime;
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
        _rigidbody.linearVelocity = new Vector2(-facingDirection * data.parryForce, 0);
        _parryCooldownTime = data.parryMulCooldownTime;
        wasHurted = false;
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
        if (_timeHurtRecover > 0)
        {
            _timeHurtRecover -= Time.deltaTime;
        }
    }

    private bool IsInvincible()
    {
        return _timeHurtRecover > 0;
    }

    public void disableHurt()
    {
        wasHurted = false;
        wasHurtedHeavyAttack = false;
    }

    #endregion

    #region Visual Effects

    public void RunStop()
    {
        if (playerFeedbackManager != null )
        {
            playerFeedbackManager.PlayFeedback(PlayerFeedbackType.Run_Stop);
             MMF_InstantiateObject instantiateFeedback = playerFeedbackManager.GetFeedback(PlayerFeedbackType.Run_Stop).GetFeedbackOfType<MMF_InstantiateObject>();
            if (wasParryPressed && facingDirection >0)
            {
                instantiateFeedback.PositionOffset += new Vector3(0.2f, 0,0);
            } else if (wasParryPressed && facingDirection < 0)
            {
                instantiateFeedback.PositionOffset -=new Vector3(0.2f, 0,0);
            }

            Vector3 scale =  instantiateFeedback.GameObjectToInstantiate.transform.localScale;
              scale.x = MathF.Abs(scale.x) * -facingDirection;
            instantiateFeedback.GameObjectToInstantiate.transform.localScale = scale;
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
        if (playerSensorManager != null && playerSensorManager.groundSensor.State())
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

    public GrabableLedge GetGrabableLedge()
    {
        if (isGrabbing)
        {
            Vector3 rayStart;
            if (facingDirection == 1)
                rayStart = playerSensorManager.wallSensorR2.transform.position + new Vector3(0.2f, 0.0f, 0.0f);
            else
                rayStart = playerSensorManager.wallSensorL2.transform.position - new Vector3(0.6f, 0.0f, 0.0f);

            var hit = Physics2D.Raycast(rayStart, Vector2.down, 1.0f,LayerMaskGrab);
            if (hit)
            {
                return hit.transform.GetComponent<GrabableLedge>();
            }
        }
        return null;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying && playerSensorManager == null)
            playerSensorManager = GetComponent<PlayerSensorManager>();
            
        if (playerSensorManager == null) return;

        if (playerSensorManager.wallSensorR2 != null && playerSensorManager.wallSensorL2 != null)
        {
            if (facingDirection == 1)
            {
                // Right side raycast check & visualization
                Vector3 rightRayStart = playerSensorManager.wallSensorR2.transform.position + new Vector3(0.2f, 0.0f, 0.0f);
                var rightHit = Physics2D.Raycast(rightRayStart, Vector2.down, 1.0f);
                bool rightLedgeDetected = rightHit && rightHit.transform.GetComponent<GrabableLedge>() != null;

                Gizmos.color = rightLedgeDetected ? Color.green : Color.red;
                Gizmos.DrawLine(rightRayStart, rightRayStart + Vector3.down * 1.0f);
                Gizmos.DrawWireSphere(rightRayStart, 0.05f);
            }
            else if  (facingDirection == -1)
            {
                // Left side raycast check & visualization
                Vector3 leftRayStart = playerSensorManager.wallSensorL2.transform.position - new Vector3(0.6f, 0.0f, 0.0f);
                var leftHit = Physics2D.Raycast(leftRayStart, Vector2.down, 1.0f);
                bool leftLedgeDetected = leftHit && leftHit.transform.GetComponent<GrabableLedge>() != null;

                Gizmos.color = leftLedgeDetected ? Color.green : Color.red;
                Gizmos.DrawLine(leftRayStart, leftRayStart + Vector3.down * 1.0f);
                Gizmos.DrawWireSphere(leftRayStart, 0.05f);
            }
            
         
        }
    }

    public void SetPositionToClimbPosition()
    {
        transform.position = climbPosition;
        SetGravityScale(data.gravityScale);
        if (playerSensorManager != null)
        {
            playerSensorManager.wallSensorR1.Disable(3.0f / 14.0f);
            playerSensorManager.wallSensorR2.Disable(3.0f / 14.0f);
            playerSensorManager.wallSensorL1.Disable(3.0f / 14.0f);
            playerSensorManager.wallSensorL2.Disable(3.0f / 14.0f);
        }
    }

    public void DisableWallSensors()
    {
        if (playerSensorManager != null)
        {
            playerSensorManager.wallSensorR1.Disable(0.8f);
            playerSensorManager.wallSensorR2.Disable(0.8f);
            playerSensorManager.wallSensorL1.Disable(0.8f);
            playerSensorManager.wallSensorL2.Disable(0.8f);
        }
        SetGravityScale(data.gravityScale);
    }


    #endregion
}
