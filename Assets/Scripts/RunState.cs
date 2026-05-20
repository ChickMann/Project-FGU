using UnityEngine;

namespace StateSystem
{
    public class RunState : IState
    {
        private static readonly int RunStartHash = Animator.StringToHash("RunStart");
        private Animator _animator;

        private PlayerStateMachine context;
        private PlayerController playerController;

        public RunState(PlayerStateMachine context, PlayerController playerController) 
        {
            this.context = context;
            this.playerController = playerController;
        }

        public void Enter(Animator animator)
        {
            Debug.Log("Run Enter");
            _animator = animator;
            _animator.Play(RunStartHash, 0, 0f);
        }

        public void Execute()
        {
            Debug.Log("Run execute");
            playerController.CheckDirectionToFace();

            if (playerController._moveDirectionX == 0 || !playerController.isRunning || playerController.isReversingDirection)
            {
                context.ChangeState(context.RunStop);
            }
           
            if (playerController.wasJumpPressed　|| !playerController.isGrounding)
            {
                context.ChangeState(context.Jump);
            }
            if (playerController.isCrouching)
            {
                context.ChangeState(context.Crouch);
                playerController.RunStop();
            }

            if (playerController.wasDodgePressed)
            {
                context.ChangeState(context.Dodge);
            }
            if (playerController.wasParryPressed)
            {
                context.ChangeState(context.Parry);
                playerController.RunStop();
            }
            if (playerController.wasAttackPressed)
            {
                context.ChangeState(context.Attack);
                playerController.RunStop();
            }
            if (playerController.wasHurted)
            {
                context.ChangeState(context.Hurt);
            }
            if (playerController.isDeath)
            {
                context.ChangeState(context.Death);
            }
            

        }

        public void FixedExecute()
        { 
            playerController.Moving();
        }

        public void Exit()
        {
            Debug.Log("Run exit");
        }
    }
}

