using UnityEngine;

namespace StateMachinePlayer
{
    public class HurtState : IState
    {
  
        private static readonly int HurtHash = Animator.StringToHash("Hurt");
        private Animator _animator;

        private PlayerStateManager context;
        private PlayerController playerController;
        
        public HurtState(PlayerStateManager context, PlayerController playerController) 
        {
            this.context = context;
            this.playerController = playerController;
        }
        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(HurtHash, 0, 0f);
            playerController.playerSliderBar.hurtDame();
            
        }

        public void Execute()
        {
            if (playerController.isDeath)
            {
                context.ChangeState(context.Death);
                return;
            }

            AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
            if (animState.shortNameHash == HurtHash && animState.normalizedTime >= 1.0f)
            {
                context.ChangeState(context.Idle);
                return;
            }

            if (animState.shortNameHash == HurtHash && animState.normalizedTime >= 0.5f && playerController.isFalling)
            {
                context.ChangeState(context.Fall);
                return;
            }
        }

        public void FixedExecute()
        {
            playerController.Hurting();
            playerController.Gravity();
          
        }

        public void Exit()
        {
            playerController.disableHurt();
        }
    }
}

