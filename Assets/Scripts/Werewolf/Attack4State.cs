using UnityEngine;

namespace WerewolfStateMachine
{
    public class Attack4State : IState
    {
        private static readonly int Attack4Hash = Animator.StringToHash("attack4");
        private Animator _animator;

        private WerewolfStateManager context;
        private WerewolfMovement werewolfMovement;

        public Attack4State(WerewolfStateManager context, WerewolfMovement werewolfMovement) 
        {
            this.context = context;
            this.werewolfMovement = werewolfMovement;
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(Attack4Hash, 0, 0f);
            werewolfMovement.ResetConsecutiveHurt();
        }

        public void Execute()
        {
            if (werewolfMovement.isDeath)
            {
                context.ChangeState(context.Death);
                return;
            }
            

            AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
            if (animState.shortNameHash == Attack4Hash && animState.normalizedTime >= 1.0f)
            {
                werewolfMovement.isJumping = false;
                werewolfMovement.EnableSensor();
                context.ChangeState(context.Landing);
                return;
            }
        }

        public void FixedExecute()
        {
            werewolfMovement.MovingToTarget(0.5f);
        }

        public void Exit()
        {
        }
    }
}
