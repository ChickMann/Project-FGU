using UnityEngine;

namespace StateSystem
{
    public class HurtState : IState
    {
  
        private static readonly int HurtHash = Animator.StringToHash("Hurt");
        private Animator _animator;

        private PlayerStateMachine context;
        private PlayerController playerController;
        
        public HurtState(PlayerStateMachine context, PlayerController playerController) 
        {
            this.context = context;
            this.playerController = playerController;
        }
        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(HurtHash, 0, 0f);
            playerController._playerStatsManager.hurtDame();
        }

        public void Execute()
        {
           AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
           if (animState.shortNameHash == HurtHash && animState.normalizedTime >= 1.0f)
           {
               context.ChangeState(context.Idle);
           }

           if (animState.shortNameHash == HurtHash && animState.normalizedTime >= 0.5f && playerController.isFalling)
           {
               context.ChangeState(context.Fall);
           }

        
        }

        public void FixedExecute()
        {
            playerController.Hurting();
            playerController.Gravity();
          
        }

        public void Exit()
        {
        }
    }
}

