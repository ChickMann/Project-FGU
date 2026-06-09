using UnityEngine;

namespace WerewolfStateMachine
{
    public class HurtState : IState
    {
        private static readonly int HurtHash = Animator.StringToHash("Hurt");
        private Animator _animator;

        private WerewolfStateManager context;
        private WerewolfMovement werewolfMovement;

        public HurtState(WerewolfStateManager context, WerewolfMovement werewolfMovement) 
        {
            this.context = context;
            this.werewolfMovement = werewolfMovement;
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(HurtHash, 0, 0f);
        }

        public void Execute()
        {
            AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
            if (animState.shortNameHash == HurtHash && animState.normalizedTime >= 1.0f)
            {
                context.ChangeState(context.Idle);
                werewolfMovement.WalkInput();
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

