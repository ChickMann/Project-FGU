using UnityEngine;

namespace WerewolfStateMachine
{
    public class RunToWalkState : IState
    {
        private static readonly int RunToWalkHash = Animator.StringToHash("RunToWalk");
        private Animator _animator;

        private WerewolfStateManager context;
        private WerewolfMovement werewolfMovement;

        public RunToWalkState(WerewolfStateManager context, WerewolfMovement werewolfMovement) 
        {
            this.context = context;
            this.werewolfMovement = werewolfMovement;
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(RunToWalkHash, 0, 0f);
        }

        public void Execute()
        {
            if (werewolfMovement.isDeath)
            {
                context.ChangeState(context.Death);
                return;
            }
            if (werewolfMovement.isHurting)
            {
                context.ChangeState(context.Hurt);
                return;
            }

            AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
            if (animState.shortNameHash == RunToWalkHash && animState.normalizedTime >= 1.0f)
            {
                if (werewolfMovement.isWalking)
                {
                    context.ChangeState(context.Walk);
                    return;
                }
                else
                {
                    context.ChangeState(context.Idle);
                    return;
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
