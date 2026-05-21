using UnityEngine;

[CreateAssetMenu(menuName = "Player Data")] // Thêm dòng này để tạo file dễ dàng
public class PlayerData : ScriptableObject
{
    [Header("Gravity")]
    [HideInInspector] public float gravityStrength; //Downwards force (gravity) needed for the desired jumpHeight and jumpTimeToApex.
     public float gravityScale; //Strength of the player's gravity as a multiplier of gravity (set in ProjectSettings/Physics2D).
    //Also the value the player's rigidbody2D.gravityScale is set to.
    [Space(5)]
    public float fallGravityMult; //Multiplier to the player's gravityScale when falling.
    public float maxFallSpeed; //Maximum fall speed (terminal velocity) of the player when falling.
    [Space(5)]
    public float fastFallGravityMult; //Larger multiplier to the player's gravityScale when they are falling and a downwards input is pressed.
    //Seen in games such as Celeste, lets the player fall extra fast if they wish.
    public float maxFastFallSpeed; //Maximum fall speed(terminal velocity) of the player when performing a faster fall.
    [Space(20)]
    
    [Header("Move")]
    public float walkMaxSpeed = 5f; 
    public float walkAcceleration = 0.1f; 
    [HideInInspector] public float walkAccelAmount; 
    [Space(5)]
    public float runMaxSpeed = 10f; 
    public float runAcceleration = 0.2f; 
    [HideInInspector] public float runAccelAmount; 
    public float runDecceleration = 0.2f; 
    [HideInInspector] public float runDeccelAmount; 
    [Space(5)]
    [Range(0.01f, 1)] public float accelInAir = 0.5f; 
    [Range(0.01f, 1)] public float deccelInAir = 0.5f;
    public bool doConserveMomentum = true;
    
    [Header("Jump")]
    public float jumpHeight; //Height of the player's jump
    public float jumpTimeToApex; //Time between applying the jump force and reaching the desired jump height. These values also control the player's gravity and jump force.
     public float jumpForce; //The actual force applied (upwards) to the player when they jump.

    [Header("Both Jumps")]
    public float jumpCutGravityMult; //Multiplier to increase gravity if the player releases thje jump button while still jumping
    [Range(0f, 1)] public float jumpHangGravityMult; //Reduces gravity while close to the apex (desired max height) of the jump
    public float jumpHangTimeThreshold; //Speeds (close to 0) where the player will experience extra "jump hang". The player's velocity.y is closest to 0 at the jump's apex (think of the gradient of a parabola or quadratic function)
    [Space(0.5f)]
    public float jumpHangAccelerationMult; 
    public float jumpHangMaxSpeedMult; 				

    [Header("Wall Jump")]
    public Vector2 wallJumpForce; //The actual force (this time set by us) applied to the player when wall jumping.
    [Space(5)]
    [Range(0f, 1f)] public float wallJumpRunLerp; //Reduces the effect of player's movement while wall jumping.
    [Range(0f, 1.5f)] public float wallJumpTime; //Time after wall jumping the player's movement is slowed for.
    public bool doTurnOnWallJump; //Player will rotate to face wall jumping direction

    [Space(20)]

    [Header("Slide")]
    public float slideSpeed;
    public float slideAccel;
    public float slideSpeedFaster;
    public float slideSpeedLower;

    [Header("Assists")]
    [Range(0.01f, 0.5f)] public float coyoteTime; //Grace period after falling off a platform, where you can still jump
    [Range(0.01f, 0.5f)] public float jumpInputBufferTime; //Grace period after pressing jump where a jump will be automatically performed once the requirements (eg. being grounded) are met.
    [Range(0.01f, 0.5f)] public float DodgeInputBufferTime; 
    [Range(0.01f, 0.5f)] public float AttackInputBufferTime;
    [Range(0.01f, 0.5f)] public float ParryInputBufferTime;
    
    
    [Header("Dogde")]
    public float dogdeDistance;
    public float dogdeTime;
    [HideInInspector] public float dogdeForce;
    public float dogdeCooldownTime;
    
    [Header("Attack")]
     public float attackForce;
    public float attackCooldownTime;
    public float hurtForce;
    public float PunchCooldownTime;
    
    [Header("Parry")]
    public float parryForce;
    public float parryCooldownTime;
    private void OnValidate()
    {
        
        //Calculate gravity strength using the formula (gravity = 2 * jumpHeight / timeToJumpApex^2) 
        // Công thức tính trọng lực khi có đầu vào thời gian nhảy tới đỉnh và độ cao muốn nhảy
        gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);
		
        //Calculate the rigidbody's gravity scale (ie: gravity strength relative to unity's gravity value, see project settings/Physics2D)
        // Công thức chuyển sang trọng lực trong unity 
        gravityScale = gravityStrength / Physics2D.gravity.y;
        
        //Calculate jumpForce using the formula (initialJumpVelocity = gravity * timeToJumpApex)
        // Công thức lức v0 lực đẩy ban đầu để đạt được độ cao và thời gian đạt đỉnh như mong muốn v0 = |g|.t
        jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;
        
        dogdeForce = dogdeDistance / dogdeTime;
        
        const float FixedUpdatesPerSecond = 50f;
        
        walkAccelAmount = (FixedUpdatesPerSecond * walkAcceleration) / walkMaxSpeed;
        runAccelAmount = (FixedUpdatesPerSecond * runAcceleration) / runMaxSpeed;
        runDeccelAmount = (FixedUpdatesPerSecond * runDecceleration) / runMaxSpeed;
        
        walkAcceleration = Mathf.Clamp(walkAcceleration, 0.01f, walkMaxSpeed);
        runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
        runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
        
        
        slideSpeedFaster  = Mathf.Abs(slideSpeedFaster) < Mathf.Abs(slideSpeed) && slideSpeed * slideSpeedFaster <0 ? slideSpeed : slideSpeedFaster;

    }
}