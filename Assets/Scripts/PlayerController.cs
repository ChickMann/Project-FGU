using System;
using StateSystem;
using UnityEngine;
using UnityEngine.InputSystem;

[ RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    
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
    public InputActionReference _PunchAction;
    
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

    [Header("Player Stats")] public PlayerData data;
    
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
    
    
    public bool wasJumpPressed { get; private set; }
    public bool wasHurted { get; private set; }
    public bool wasDodgePressed { get; private set; }
    public bool wasAttackPressed { get; private set; }
    public bool wasParryPressed { get; private set; }
    public bool wasPunchPresssed { get; private set; }
    
    
  
    
    public Rigidbody2D _rigidbody { get; private set; }
    public float _moveDirectionX { get; private set; }

    public int facingDirection { get; private set; }
    private float _lastNonZeroInputX = 1f;
    private Vector2 oldVelocity;
    private float _dogdeTimeElapsed ;
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
      
    }

    private void Start()
    {
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
        
        isDeath = _deathAction.action.IsPressed();
        _moveDirectionX = _moveAction.action.ReadValue<Vector2>().x;
        isRunning =  _runAction.action.IsPressed();
        isCrouching = _crouchAction.action.IsPressed();
        wasDodgePressed = OnDodgeInput();
        isGrounding = IsGrounded();
        wasParryPressed = OnParryInput();
        wasJumpPressed = OnJumpInput();
        wasAttackPressed = OnAttackInput();
        wasHurted = _hurtAction.action.WasPressedThisFrame();
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
    }

  

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
                
                //xuất phát với tốc độ nhanh gấp đôi vì thời gian giảm dần 
                float startVelocity = (2f * data.dogdeDistance) / data.dogdeTime;
                
                //tính phần trăm thời gian 
                float normalizedTime = _dogdeTimeElapsed / data.dogdeTime;
            
                //Tìm một điểm nằm giữa hai giá trị, dựa trên một tỷ lệ phần trăm.
                float currentSpeed = Mathf.Lerp(startVelocity, 0f, normalizedTime);
            
                _rigidbody.linearVelocity = new Vector2(facingDirection * currentSpeed, _rigidbody.linearVelocity.y);
            
                _dogdeTimeElapsed += Time.fixedDeltaTime;
                
            }
            else 
            {
                // Reset biến khi lướt xong để chuẩn bị cho lần sau
                _dogdeTimeElapsed = 0f;
               isDodging = false;
              
            }
        }
    }

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
        if (_PunchAction.action.WasPressedThisFrame() && _punchCooldownTime <=0)
        {
            LastPressedPunchTime = data.AttackInputBufferTime;
            _punchCooldownTime = data.PunchCooldownTime;
        }
        return LastPressedPunchTime > 0;
    }

    public void AttackEffect()
    {
        _rigidbody.linearVelocity = new Vector2(facingDirection * data.attackForce, 0);
    }
    public void Parrying()
    {
        _rigidbody.linearVelocity = new Vector2(-facingDirection * data.parryForce, 0);
        if (parryEffect != null ) Instantiate(parryEffect, transitionEffect.transform.position, Quaternion.identity);
    }
    public void Hurting()
    {
        
        _rigidbody.linearVelocity = new Vector2(-facingDirection * data.hurtForce, _rigidbody.linearVelocity.y);
        _parryCooldownTime = data.parryCooldownTime;
    }

    public void Die()
    {
        _rigidbody.linearVelocity = Vector2.zero;
    }
   

    private bool IsGrounded()
    {
        if (_groundSensor.State())
        {
            LastOnGroundTime = data.coyoteTime;
            isJumpCut = false;
           
        }
        return LastOnGroundTime > 0;
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

 

    public bool IsFalling()
    {
        return _rigidbody.linearVelocity.y<0 ;
    }
    // private void OnDrawGizmosSelected()
    // {
    //     if (groundCheckPoint != null)
    //     {
    //         Gizmos.color = isGrounding? Color.green : Color.red;
    //         Gizmos.DrawWireSphere(groundCheckPoint.position, checkRadius);
    //     }
    //   
    // }

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
            //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, Mathf.Max(_rigidbody.linearVelocity.y, -data.maxFastFallSpeed));
        }
        else if (isJumpCut)
        {
            //Higher gravity if jump button released
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

 
  
       
}
