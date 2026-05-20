using UnityEngine;

namespace StateSystem
{
    public class StandUpState : IState
    {
        private static readonly int StandUpHash = Animator.StringToHash("StandUp");
        private Animator _animator;

        private PlayerStateMachine context;
        private PlayerController playerController;

        public StandUpState(PlayerStateMachine context, PlayerController playerController) 
        {
            this.context = context;
            this.playerController = playerController;
        }

        public void Enter(Animator animator)
        {
            Debug.Log("StandUp Enter");
            _animator = animator;
            _animator.Play(StandUpHash, 0, 0f);
        }

        public void Execute()
        {
            Debug.Log("StandUp execute");
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
            Debug.Log("StandUp exit");
        }
    }
 
}


