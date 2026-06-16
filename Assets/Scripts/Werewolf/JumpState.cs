using UnityEngine;

namespace WerewolfStateMachine
{
    public class JumpState : IState
    {
        private static readonly int JumpHash = Animator.StringToHash("Jump");
        private Animator _animator;

        private WerewolfStateManager context;
        private WerewolfMovement werewolfMovement;

        public JumpState(WerewolfStateManager context, WerewolfMovement werewolfMovement) 
        {
            this.context = context;
            this.werewolfMovement = werewolfMovement;
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(JumpHash, 0, 0f);
        }

        public void Execute()
        {
            if (werewolfMovement.isDeath)
            {
                context.ChangeState(context.Death);
                return;
            }
        

            AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
            if (animState.shortNameHash == JumpHash && animState.normalizedTime >= 1)
            {
                context.ChangeState(context.Attack4);
                return;
            }
        }

        public void FixedExecute()
        {
            werewolfMovement.Jumping();
            werewolfMovement.MovingToTarget(0.5f);
        }

        public void Exit()
        {
            
        }
    }
}
