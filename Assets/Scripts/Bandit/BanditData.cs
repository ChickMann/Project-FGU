using UnityEngine;

[CreateAssetMenu(menuName = "Bandit Data")]
public class BanditData : ScriptableObject
{
    [Header("Gravity")]
    [HideInInspector] public float gravityStrength;
    public float gravityScale;
    [Space(5)]

    [Header("Move")]
    public float runMaxSpeed = 7f;
    public float runAcceleration = 0.15f;
    [HideInInspector] public float runAccelAmount;
    public float runDecceleration = 0.15f;
    [HideInInspector] public float runDeccelAmount;
    [Space(5)]
    [Range(0.01f, 1)] public float accelInAir = 0.5f;
    [Range(0.01f, 1)] public float deccelInAir = 0.5f;
    public bool doConserveMomentum = true;
    [Space(5)]
    public float patrolSpeed = 2f;
    public float patrolRange = 4f;

    [Header("Detection")]
    public float lineOfSightDistance = 6f;
    public float detectionRange = 8f;
    public float jumpTriggerDistance = 4f;

    [Header("Attack")]
    public float attackRange = 1.5f;
    public float combatRange = 1.3f;
    public float attackForce = 3f;
    public float attackCooldown = 1.5f;

    [Header("Jump")]
    public float jumpForce;
    public float jumpTimeToApex = 0.4f;
    public float jumpHeight = 3f;
    public float jumpHangTimeThreshold = 0.1f;
    public float jumpHangAccelerationMult = 1.1f;
    public float jumpHangMaxSpeedMult = 1.3f;

    [Header("Hurt")]
    public float hurtForce = 3f;
    public float timeRecover = 0.5f;

    private void OnValidate()
    {
        gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);
        gravityScale = gravityStrength / Physics2D.gravity.y;

        jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;

        const float FixedUpdatesPerSecond = 50f;
        runAccelAmount = (FixedUpdatesPerSecond * runAcceleration) / runMaxSpeed;
        runDeccelAmount = (FixedUpdatesPerSecond * runDecceleration) / runMaxSpeed;

        runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
        runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
    }
}
