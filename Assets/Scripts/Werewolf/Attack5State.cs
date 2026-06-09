using UnityEngine;

namespace WerewolfStateMachine
{
    public class Attack5State : IState
    {
        private static readonly int Attack5Hash = Animator.StringToHash("Attack5");
        private Animator _animator;

        private WerewolfStateManager context;
        private WerewolfMovement werewolfMovement;

        public Attack5State(WerewolfStateManager context, WerewolfMovement werewolfMovement) 
        {
            this.context = context;
            this.werewolfMovement = werewolfMovement;
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(Attack5Hash, 0, 0f);
        }

        public void Execute()
        {
            AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
            if (animState.shortNameHash == Attack5Hash && animState.normalizedTime >= 1.0f)
            {
                
                context.ChangeState(context.Landing);
            }
        }

        public void FixedExecute()
        {
            werewolfMovement.Dogding();
            werewolfMovement.MovingToTarget(1.5f);
        }

        public void Exit()
        {
        }
    }
}
