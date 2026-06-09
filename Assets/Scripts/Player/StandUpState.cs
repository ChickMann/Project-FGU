using UnityEngine;

namespace StateMachinePlayer
{
    public class StandUpState : IState
    {
        private static readonly int StandUpHash = Animator.StringToHash("StandUp");
        private Animator _animator;

        private PlayerStateManager context;
        private PlayerController playerController;

        public StandUpState(PlayerStateManager context, PlayerController playerController) 
        {
            this.context = context;
            this.playerController = playerController;
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(StandUpHash, 0, 0f);
        }

        public void Execute()
        {
          AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
          if (animState.IsName("StandUp") && animState.normalizedTime >= 1.0f)
          {
              context.ChangeState(context.Idle);
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
            
        }

        public void Exit()
        {
        }
    }
 
}


