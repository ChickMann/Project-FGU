using UnityEngine;

namespace StateMachinePlayer
{
    public class FallState : IState
    {
        private static readonly int FallHash = Animator.StringToHash("Fall");
        private static readonly int LandingHash = Animator.StringToHash("Landing");
        private static readonly int CrouchHash = Animator.StringToHash("Crouch");
        private Animator _animator;

        private PlayerStateManager context;
        private PlayerController playerController;
        
        bool isLanding = false;
        bool isFallCrouch = false;

        public FallState(PlayerStateManager context, PlayerController playerController) 
        {
            this.context = context;
            this.playerController = playerController;
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(FallHash, 0, 0f);
        }

        public void Execute()
        {
            if (playerController.isDeath)
            {
                context.ChangeState(context.Death);
                return;
            }
            if (playerController.wasHurted)
            {
                context.ChangeState(context.Hurt);
                return;
            }

            var ledge = playerController.GetGrabableLedge();
            Debug.Log(ledge);
            if (ledge != null)
            {
                playerController.climbPosition = ledge.transform.position + new Vector3(ledge.topClimbPosition.x, ledge.topClimbPosition.y, 0);
                if (playerController.facingDirection == 1)
                {
                    playerController.transform.position = ledge.transform.position + new Vector3(ledge.leftGrabPosition.x, ledge.leftGrabPosition.y, 0);
                }
                else
                {
                    playerController.transform.position = ledge.transform.position + new Vector3(ledge.rightGrabPosition.x, ledge.rightGrabPosition.y, 0);
                }

                context.ChangeState(context.GrabLedge);
                return;
            }

            playerController.CheckDirectionToFace();
            if (isFallCrouch && playerController.isGrounding && !isLanding)
            {
                _animator.Play(CrouchHash, 0, 0f);
                isLanding = true;
            }
            if (playerController.isGrounding && !isLanding)
            {
                _animator.Play(LandingHash, 0, 0f);
                isLanding = true;
            }
            AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
            if (playerController.isGrounding && animState.shortNameHash == LandingHash && animState.normalizedTime >= 1.0f)
            {
                context.ChangeState(context.Idle);
                return;
            }
            if (animState.shortNameHash == CrouchHash && animState.normalizedTime >= 1.0f)
            {
                context.ChangeState(context.Idle);
                return;
            }
            if (playerController.wasJumpPressed && playerController.isGrounding)
            {
                context.ChangeState(context.Jump);
                return;
            }

            if (isLanding)
            {
                if (playerController.wasDodgePressed)
                {
                    context.ChangeState(context.Dodge);
                    return;
                }
                if (playerController.isCrouching)
                {
                    context.ChangeState(context.Crouch);
                    return;
                }
                if (playerController.wasAttackPressed)
                {
                    context.ChangeState(context.Attack);
                    return;
                }
                if (playerController.wasParryPressed)
                {
                    context.ChangeState(context.Parry);
                    return;
                }
                if (playerController.isRunning)
                {
                    context.ChangeState(context.Run);
                    return;
                }
            }

            if (playerController.isWallSliding && !playerController.isGrounding && playerController.isFalling)
            {
                context.ChangeState(context.WallSlide);
                return;
            }

            if (Mathf.Abs(playerController._rigidbody.linearVelocity.y) >= playerController.data.maxFallSpeed *2/3 )
            {
                isFallCrouch = true;
            }
        }

        public void FixedExecute()
        { 
            if(!isLanding)
             playerController.Moving();
            
            playerController.Jumping();
            playerController.Gravity();
        }

        public void Exit()
        {
            isLanding = false;
            isFallCrouch = false;
        }
    }
}

