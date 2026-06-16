using UnityEngine;

namespace StateMachinePlayer
{
    public class RunState : IState
    {
        private static readonly int RunStartHash = Animator.StringToHash("RunStart");
        private Animator _animator;

        private PlayerStateManager context;
        private PlayerController playerController;

        public RunState(PlayerStateManager context, PlayerController playerController) 
        {
            this.context = context;
            this.playerController = playerController;
        }

        public void Enter(Animator animator)
        {
            
            _animator = animator;
            _animator.Play(RunStartHash, 0, 0f);
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

            playerController.CheckDirectionToFace();

            if (playerController.wasJumpPressed || !playerController.isGrounding)
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
                playerController.RunStop();
                context.ChangeState(context.Parry);
                return;
            }
            if (playerController.wasAttackPressed)
            {
                playerController.RunStop();
                context.ChangeState(context.Attack);
                return;
            }
            if (playerController.wasPunchPresssed)
            {
                playerController.RunStop();
                context.ChangeState(context.Punch);
                return;
            }
            if (playerController.isCrouching)
            {
                playerController.RunStop();
                context.ChangeState(context.Crouch);
                return;
            }
            if (playerController._moveDirectionX == 0 || !playerController.isRunning || playerController.isReversingDirection)
            {
                context.ChangeState(context.RunStop);
                return;
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

