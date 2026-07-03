using MoreMountains.Feedbacks;
using UnityEngine;

namespace BanditStateMachine
{
    [RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(BanditSliderBar))]
    public class BanditMovement : MonoBehaviour
    {
        [Header("References")]
        public BanditData data;
        public GameObject target;
        public Sensor groundSensor;
        public BanditSliderBar slider;

        [Header("State Flags")]
        public bool isFalling { get; private set; }
        public bool isGrounding { get; private set; }
        public bool isRunning;
        public bool isJumping;
        public bool isHurting;
        public bool isDeath;
        public bool isAttacking;
        public bool isCombatIdle;

        [Header("MM_Feedback")] public MMF_Player soundSword;

        public float patrolSpeed => data != null ? data.patrolSpeed : 2f;
        public float patrolRange => data != null ? data.patrolRange : 4f;
        public float lineOfSightDistance => data != null ? data.lineOfSightDistance : 6f;
        public float detectionRange => data != null ? data.detectionRange : 8f;
        public float attackRange => data != null ? data.attackRange : 1.5f;
        public float combatRange => data != null ? data.combatRange : 1.3f;
        public float jumpTriggerDistance => data != null ? data.jumpTriggerDistance : 4f;
        public float timeRecover => data != null ? data.timeRecover : 0.5f;

        public int facingDirection;
        public Rigidbody2D rb { get; private set; }

        private float _timeHurtRecover;
        private float _attackCooldownTimer;
        private Vector2 _startPosition;
        private int _patrolDirection;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            slider = GetComponent<BanditSliderBar>();
        }

        private void Start()
        {
            if (data != null)
                SetGravityScale(data.gravityScale);
            else
                SetGravityScale(1f);

            facingDirection = 1;
            _patrolDirection = facingDirection;
            target = GameObject.FindGameObjectWithTag("Player");
            _startPosition = transform.position;
        }

        private void Update()
        {
            if (isDeath) return;

            isGrounding = groundSensor != null ? groundSensor.State() : true;

            if (!isGrounding && rb.linearVelocity.y < -0.1f)
                isFalling = true;
            else if (isGrounding)
                isFalling = false;

            RecoverHurt();

            if (_attackCooldownTimer > 0)
                _attackCooldownTimer -= Time.deltaTime;

            if (slider != null && slider.IsDeath())
                isDeath = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isDeath) return;

            if (other.gameObject.CompareTag("Hit Box") && !isHurting)
            {
                isHurting = true;
                _timeHurtRecover = timeRecover;
            }
        }

        private void OnDrawGizmos()
        {
          
            Gizmos.color = Color.red;
            Vector3 rayStart = transform.position + Vector3.up * 0.5f;
            Gizmos.DrawRay(rayStart, Vector2.right * (-facingDirection) * lineOfSightDistance);
        }

        #region Movement

        public void Patrol()
        {
            float distanceFromStart = transform.position.x - _startPosition.x;
            if (Mathf.Abs(distanceFromStart) >= patrolRange)
            {
                _patrolDirection = distanceFromStart > 0 ? 1 : -1;
                Turn(_patrolDirection);
            }

            rb.linearVelocity = new Vector2(-facingDirection * patrolSpeed, rb.linearVelocity.y);
        }

        public bool MovingToTarget(float stopDistance)
        {
            if (target == null) return false;

            float distX = target.transform.position.x - transform.position.x;

            if (Mathf.Abs(distX) <= stopDistance)
            {
                StopMoving();
                return false;
            }

            int moveDirX = facingDirection * -1;
            float targetSpeed = moveDirX * data.runMaxSpeed;

            float accelRate = isGrounding ? data.runAccelAmount : data.runAccelAmount * data.accelInAir;

            if (data.doConserveMomentum
                && Mathf.Abs(rb.linearVelocity.x) > Mathf.Abs(targetSpeed)
                && rb.linearVelocity.x * targetSpeed > 0f
                && !isGrounding)
            {
                accelRate = 0f;
            }

            if ((isJumping || isFalling) && Mathf.Abs(rb.linearVelocity.y) < data.jumpHangTimeThreshold)
            {
                accelRate *= data.jumpHangAccelerationMult;
                targetSpeed *= data.jumpHangMaxSpeedMult;
            }

            float newVelocityX = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, accelRate * 15f * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(newVelocityX, rb.linearVelocity.y);
            return true;
        }

        public void Jumping()
        {
            if (!isGrounding) return;

            float force = data != null ? data.jumpForce : 10f;
            if (rb.linearVelocity.y < 0)
                force -= rb.linearVelocity.y;

            rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
            isJumping = true;
            isFalling = false;

            if (groundSensor != null) groundSensor.Disable(0.2f);
        }

        public void StopMoving()
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        public void SetGravityScale(float scale)
        {
            rb.gravityScale = scale;
        }

        public void CheckDirectionToFace()
        {
            if (target == null) return;
            float dirX = target.transform.position.x - transform.position.x;
            int targetDirection = dirX >= 0 ? -1 : 1;

            if (targetDirection != facingDirection)
                Turn(targetDirection);
        }

        public void Turn(int direction)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * direction;
            transform.localScale = scale;
            facingDirection = direction;
        }

        #endregion

        #region Combat

        public bool DetectPlayer()
        {
            Vector2 rayDir = Vector2.right * (-facingDirection);
            Vector3 rayStart = transform.position + Vector3.up * 0.5f;
            RaycastHit2D[] hits = Physics2D.RaycastAll(rayStart, rayDir, lineOfSightDistance);

            foreach (var hit in hits)
            {
                if (hit.collider == null || hit.collider.gameObject == gameObject) continue;
                if (hit.collider.isTrigger && !hit.collider.CompareTag("Player")) continue;

                if (hit.collider.CompareTag("Player"))
                    return true;

                if (hit.collider.CompareTag("Ground") || hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    break;
            }
            return false;
        }

        public bool IsPlayerInRange(float range)
        {
            if (target == null) return false;
            return Mathf.Abs(target.transform.position.x - transform.position.x) <= range;
        }

        public bool CanAttack()
        {
            return _attackCooldownTimer <= 0 && IsPlayerInRange(attackRange);
        }

        public void TriggerAttack()
        {
            _attackCooldownTimer = data != null ? data.attackCooldown : 1.5f;
            StopMoving();
            isAttacking = true;
        }

        public void ClearAttack()
        {
            isAttacking = false;
        }

        public void AttackEffect()
        {
            float force = data != null ? data.attackForce : 3f;
            if (!IsPlayerInRange(1f))
                rb.linearVelocity = new Vector2(-facingDirection * force, 0);
        }

        public bool ShouldJumpToTarget()
        {
            if (target == null || !isGrounding || isJumping) return false;
            float yDiff = target.transform.position.y - transform.position.y;
            float xDist = Mathf.Abs(target.transform.position.x - transform.position.x);
            return yDiff > 1.5f && xDist < jumpTriggerDistance;
        }

        #endregion

        #region Hurt

        public void Hurting()
        {
            float force = data != null ? data.hurtForce : 3f;
            rb.linearVelocity = new Vector2(facingDirection * force, rb.linearVelocity.y);
        }

        public void ClearHurt()
        {
            isHurting = false;
            _timeHurtRecover = timeRecover;
        }

        private void RecoverHurt()
        {
            if (!isHurting) return;
            _timeHurtRecover -= Time.deltaTime;
            if (_timeHurtRecover <= 0)
                ClearHurt();
        }

        #endregion

        public void AttackSound()
        {
            if(soundSword!=null) soundSword.PlayFeedbacks();
        }
    }
}
