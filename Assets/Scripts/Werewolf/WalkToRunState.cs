using UnityEngine;

namespace WerewolfStateMachine
{
    public class WalkToRunState : IState
    {
        private static readonly int WalkToRunHash = Animator.StringToHash("WalkToRun");
        private Animator _animator;

        private WerewolfStateManager context;
        private WerewolfMovement werewolfMovement;

        public WalkToRunState(WerewolfStateManager context, WerewolfMovement werewolfMovement) 
        {
            this.context = context;
            this.werewolfMovement = werewolfMovement;
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(WalkToRunHash, 0, 0f);
        }

        public void Execute()
        {
            werewolfMovement.CheckDirectionToFace();
            AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
            if (animState.shortNameHash == WalkToRunHash && animState.normalizedTime >= 1.0f)
            {
                if (werewolfMovement.isRunning)
                {
                    context.ChangeState(context.Run);
                }
                else if (werewolfMovement.isWalking)
                {
                    context.ChangeState(context.Walk);
                }
                else
                {
                    context.ChangeState(context.Idle);
                }
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
