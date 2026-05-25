using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StateSystem
{
    public class WalkState : IState
    {
        private static readonly int WalkHash = Animator.StringToHash("Walk");
        private Animator _animator;

        private PlayerStateMachine context;
        private PlayerController playerController;

        public WalkState(PlayerStateMachine context, PlayerController playerController) 
        {
            this.context = context;
            this.playerController = playerController;
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(WalkHash, 0, 0f);
        }

        public void Execute()
        {
            playerController.CheckDirectionToFace();
            if (playerController._moveDirectionX == 0)
            {
                context.ChangeState(context.Idle);
            }

            if (playerController.isRunning && playerController._moveDirectionX != 0)
            {
                context.ChangeState(context.Run);
            }
            if (playerController.wasJumpPressed　|| !playerController.isGrounding)
            {
                context.ChangeState(context.Jump);
            }
            if (playerController.isCrouching)
            {
                context.ChangeState(context.Crouch);
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
            if (playerController.wasPunchPresssed)
            {
                context.ChangeState(context.Punch);
            }
        }

        public void FixedExecute()
        { 
            playerController.Moving();
        }

        public void Exit()
        {
        }
    }
}

