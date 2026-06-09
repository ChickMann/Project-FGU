using UnityEngine;

namespace WerewolfStateMachine
{
    public class TransVer2State : IState
    {
        private static readonly int TransVer2Hash = Animator.StringToHash("Trans ver2");
        private Animator _animator;

        private WerewolfStateManager context;
        private WerewolfMovement werewolfMovement;

        public TransVer2State(WerewolfStateManager context, WerewolfMovement werewolfMovement) 
        {
            this.context = context;
            this.werewolfMovement = werewolfMovement;
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(TransVer2Hash, 0, 0f);
        }

        public void Execute()
        {
            AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
            if (animState.shortNameHash == TransVer2Hash && animState.normalizedTime >= 1.0f)
            {
                context.ChangeState(context.Idle);
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
