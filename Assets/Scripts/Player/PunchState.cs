using UnityEngine;

namespace StateMachinePlayer
{
    public class PunchState : IState
    {
        private static readonly int PunchHash = Animator.StringToHash("Throw");
        private Animator _animator;

        private PlayerStateManager context;
        private PlayerController playerController;

        public PunchState(PlayerStateManager context, PlayerController playerController) 
        {
            this.context = context;
            this.playerController = playerController;
            
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(PunchHash, 0, 0f);
            playerController.playerSliderBar.punchStamina();
        }

        public void Execute()
        {
            AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
            if (animState.IsName("Throw") && animState.normalizedTime >= 0.5f)
            {
                context.ChangeState(context.Idle);
            }
        }

        public void FixedExecute()
        { 
            playerController.Moving(false);
           

        }

        public void Exit()
        {
           
        }
    }
}
