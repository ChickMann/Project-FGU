using UnityEngine;

namespace WerewolfStateMachine
{
    public class FallState : IState
    {
        private static readonly int FallHash = Animator.StringToHash("fall");
        private Animator _animator;

        private WerewolfStateManager context;
        private WerewolfMovement werewolfMovement;

        public FallState(WerewolfStateManager context, WerewolfMovement werewolfMovement) 
        {
            this.context = context;
            this.werewolfMovement = werewolfMovement;
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(FallHash, 0, 0f);
        }

        public void Execute()
        {
            if (werewolfMovement.isGrounding)
            {
                context.ChangeState(context.Landing);
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
