using UnityEngine;

namespace StateMachinePlayer
{
    public class DeathState : IState
    {
      
        private static readonly int DeathHash = Animator.StringToHash("Death");
        private Animator _animator;

        private PlayerStateManager context;
        private PlayerController playerController;
        
        public DeathState(PlayerStateManager context, PlayerController playerController) 
        {
            this.context = context;
            this.playerController = playerController;
        }
        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(DeathHash, 0, 0f);
        }

        public void Execute()
        {
            AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
            if (animState.shortNameHash == DeathHash && animState.normalizedTime >= 1.0f && playerController._moveDirectionX !=0)
            {
                context.ChangeState(context.Walk);
                return;
            }

        }

        public void FixedExecute()
        {
            playerController.Die();
            playerController.Moving(false);
            playerController.Gravity();
        }

        public void Exit()
        {
        }
    }

}

