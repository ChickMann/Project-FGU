using UnityEngine;

namespace StateSystem
{
    public class FallState : IState
    {
        private static readonly int FallHash = Animator.StringToHash("Fall");
        private static readonly int LandingHash = Animator.StringToHash("Landing");
        private static readonly int CrouchHash = Animator.StringToHash("Crouch");
        private Animator _animator;

        private PlayerStateMachine context;
        private PlayerController playerController;
        
        bool isLanding = false;
        bool isFallCrouch = false;

        public FallState(PlayerStateMachine context, PlayerController playerController) 
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
            
            playerController.CheckDirectionToFace();
            if (isFallCrouch && playerController.isGrounding && !isLanding)
            {
                _animator.Play(CrouchHash, 0, 0f);
                playerController.Landing();
                isLanding = true;
            }
            if (playerController.isGrounding & !isLanding )
            {
                _animator.Play(LandingHash, 0, 0f);
                playerController.Landing();
                isLanding = true;
            }
            AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
            if (playerController.isGrounding && animState.IsName("Landing") && animState.normalizedTime >= 1.0f)
            {
                context.ChangeState(context.Idle);
            }
            if (playerController.wasJumpPressed && playerController.isGrounding )
            {
                context.ChangeState(context.Jump);
            }

            if (isLanding && playerController.wasDodgePressed)
            {
                context.ChangeState(context.Dodge);
            }
            if (isLanding && playerController.isCrouching)
            {
                context.ChangeState(context.Crouch);
            }
            if (isLanding && playerController.wasAttackPressed)
            {
                context.ChangeState(context.Attack);
            }
            if (isLanding && playerController.wasParryPressed)
            {
                context.ChangeState(context.Parry);
            }
            if (isLanding && playerController.isRunning)
            {
                context.ChangeState(context.Run);
            }
            if (playerController.wasHurted)
            {
                context.ChangeState(context.Hurt);
            }
            if (playerController.isDeath)
            {
                context.ChangeState(context.Death);
            }
            if (playerController.isWallSliding && !playerController.isGrounding && playerController.isFalling)
            {
                context.ChangeState(context.WallSlide);
            }
　
            if (Mathf.Abs(playerController._rigidbody.linearVelocity.y) >= playerController.data.maxFallSpeed *2/3 )
            {
                isFallCrouch = true;
               
            }

            if (animState.shortNameHash == CrouchHash && animState.normalizedTime >= 1.0f)
            {
                context.ChangeState(context.Idle);
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

