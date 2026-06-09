using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class WerewolfMovement : MonoBehaviour
{
    [Header("References")]
    public WerewolfData data;
    public GameObject target;
    
    
    [Header("sensors")]
    public Sensor_Prototype _groundSensor;
    private Rigidbody2D _rigidbody;
    
    [Header("movement")]
    public bool isFalling{ get; private set; }
    public bool isGrounding { get; private set; }
    public bool isWalking;
    public bool isRunning;
    public bool isJumping;
    public bool isHeavyAttacking { get; private set; }
    public bool isHurting;
    public bool isDodging;

    [Header("Action")] 
    public bool isAttack1;

    public bool isAttack2;
    public bool isAttack3;
    public bool isAttack4;
    public bool isAttack5;

    [Header("event")] public bool isVer2;
    
    [Header("Debug")] public bool walk;

    public int facingDirection;
    private float _dogdeTimeElapsed ;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        SetGravityScale(data.gravityScale);
        facingDirection = 1;
        target = GameObject.FindGameObjectWithTag("Player");
        
    }

    void Update()
    {
        isGrounding = _groundSensor.State();
     
    }
    
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

    public void RandomAttack()
    {
        int random = Random.Range(0, 6);
        if (random <= 3)
        {
            Attack1Input();
                    
        }
        else if (random == 4)
        {
            Atack2Input();
        }
        else 
        {
            Atack3Input();
        }
    }
    
    public void Attack1Input()
    {
        isAttack1 = true;
    }

    public void Atack2Input()
    {
        isAttack2 = true;
    }
    public void Atack3Input()
    {
        isAttack3 = true;
    }
    public void WalkInput()
    {
        isWalking = true;
    }

    public void RunInput()
    {
        isRunning = true;
    }

    public void JumpInput()
    {
        isJumping = true;
    }

    public void DodgeInput()
    {
        isDodging = true;
    }

    public void DisableSensor()
    {
        _groundSensor.gameObject.SetActive(false);
    }
    public void EnableSensor()
    {
        _groundSensor.gameObject.SetActive(true);
    }

    public void SetGravityScale(float scale)
    {
        _rigidbody.gravityScale = scale;
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
}
