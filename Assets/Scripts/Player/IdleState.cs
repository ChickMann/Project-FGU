using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StateMachinePlayer
{
    public class IdleState : IState
    {
        private static readonly int IdleHash = Animator.StringToHash("Idle");
        private Animator _animator;

        private PlayerStateManager context;
        private PlayerController playerController;

        public IdleState(PlayerStateManager context, PlayerController playerController)  
        {
            this.context = context;
            this.playerController = playerController;
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(IdleHash,0,0f);
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

            if (!playerController.isGrounding)
            {
                context.ChangeState(context.Fall);
                return;
            }

            playerController.CheckDirectionToFace();

            if (playerController.wasJumpPressed && playerController.isGrounding)
            {
                context.ChangeState(context.Jump);
                return;
            }
            if (playerController.wasDodgePressed)
            {
                context.ChangeState(context.Dodge);
                return;
            }
            if (playerController.wasParryPressed)
            {
                context.ChangeState(context.Parry);
                return;
            }
            if (playerController.wasAttackPressed)
            {
                context.ChangeState(context.Attack);
                return;
            }
            if (playerController.wasPunchPresssed)
            {
                context.ChangeState(context.Punch);
                return;
            }
            if (playerController.isFocus)
            {
                context.ChangeState(context.Focus);
                return;
            }
            if (playerController._moveDirectionX != 0)
            {
                context.ChangeState(context.Walk);
                return;
            }
            if (playerController.isCrouching)
            {
                context.ChangeState(context.Crouch);
                return;
            }
            if (playerController.isWallSliding && !playerController.isGrounding && playerController.isFalling)
            {
                context.ChangeState(context.WallSlide);
                return;
            }
        }

        public void FixedExecute()
        {
         playerController.Moving(false);
         playerController.SetGravityScale(playerController.data.gravityScale);
         playerController.playerSliderBar.IncreaseStamina(0.5f);
        }

        public void Exit()
        {
        }
    }
}

