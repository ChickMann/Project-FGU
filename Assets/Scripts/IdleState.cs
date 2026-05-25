using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StateSystem
{
    public class IdleState : IState
    {
        private static readonly int IdleHash = Animator.StringToHash("Idle");
        private Animator _animator;

        private PlayerStateMachine context;
        private PlayerController playerController;

        public IdleState(PlayerStateMachine context, PlayerController playerController)  
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
            playerController.CheckDirectionToFace();
            if (playerController._moveDirectionX != 0)
            {
                context.ChangeState(context.Walk);
            }

            if (playerController.isCrouching)
            {
                context.ChangeState(context.Crouch);
            }

            if (playerController.wasJumpPressed　&& playerController.isGrounding)
            {
                context.ChangeState(context.Jump);
            }

            if (playerController.wasParryPressed)
            {
                context.ChangeState(context.Parry);
            }

            if (playerController.wasAttackPressed)
            {
                context.ChangeState(context.Attack);
            }
            if (playerController.wasDodgePressed)
            {
                context.ChangeState(context.Dodge);
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

            if (!playerController.isGrounding)
            {
                context.ChangeState(context.Fall);
            }

            if (playerController.wasPunchPresssed)
            {
                context.ChangeState(context.Punch);
            }
            if (playerController.isFocus)
            {
                context.ChangeState(context.Focus);
            }

           
        }

        public void FixedExecute()
        {
         playerController.Moving();
         playerController.SetGravityScale(playerController.data.gravityScale);
         
        }

        public void Exit()
        {
        }
    }
}

