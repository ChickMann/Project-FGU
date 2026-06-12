using UnityEngine;

namespace WerewolfStateMachine
{
    public class LandingState : IState
    {
        private static readonly int LandingHash = Animator.StringToHash("Landing");
        private Animator _animator;

        private WerewolfStateManager context;
        private WerewolfMovement werewolfMovement;

        public LandingState(WerewolfStateManager context, WerewolfMovement werewolfMovement) 
        {
            this.context = context;
            this.werewolfMovement = werewolfMovement;
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(LandingHash, 0, 0f);
        }

        public void Execute()
        {
            AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
            if (animState.shortNameHash == LandingHash && animState.normalizedTime >= 1.0f)
            {
                if (werewolfMovement.isRunning)
                {
                    context.ChangeState(context.Run);
                }
                else
                {
                context.ChangeState(context.Idle);
                    
                }
            }
            if (werewolfMovement.isHurting)
            {
                context.ChangeState(context.Hurt);
            }
          
        }

        public void FixedExecute()
        {
            werewolfMovement.MovingToTarget(-1f);
        }

        public void Exit()
        {
        }
    }
}
