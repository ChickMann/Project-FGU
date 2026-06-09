using StateMachinePlayer;
using UnityEngine;

namespace StateMachinePlayer
{
    public class RunStopState : IState
    {
        private static readonly int RunStopHash = Animator.StringToHash("RunStop");
        private Animator _animator;

        private PlayerStateManager context;
        private PlayerController playerController;

        public RunStopState(PlayerStateManager context, PlayerController playerController) 
        {
            this.context = context;
            this.playerController = playerController;
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(RunStopHash, 0, 0f);
                playerController.RunStop();
            
        }

        public void Execute()
        {
            playerController.CheckDirectionToFace();
            AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
            if (animState.IsName("RunStop") && animState.normalizedTime >= 1.0f)
            {
                context.ChangeState(context.Idle);
            }
            if (animState.IsName("RunStop") && animState.normalizedTime >= 0.5f && playerController._moveDirectionX != 0)
            {
                context.ChangeState(context.Walk);
            }
            if (playerController.wasParryPressed)
            {
                context.ChangeState(context.Parry);
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
        }
    }
}


