using UnityEngine;

namespace WerewolfStateMachine
{
    public class RunState : IState
    {
        private static readonly int RunHash = Animator.StringToHash("Run");
        private Animator _animator;

        private WerewolfStateManager context;
        private WerewolfMovement werewolfMovement;

        public RunState(WerewolfStateManager context, WerewolfMovement werewolfMovement) 
        {
            this.context = context;
            this.werewolfMovement = werewolfMovement;
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(RunHash, 0, 0f);
        }

        public void Execute()
        {
            werewolfMovement.CheckDirectionToFace();
            if (!werewolfMovement.isRunning)
            {
                // if (werewolfMovement.isWalking)
                // {
                //     context.ChangeState(context.RunToWalk);
                // }
                // else
                // {
                //     context.ChangeState(context.Idle);
                // }
                context.ChangeState(context.Down);
            }
            if (werewolfMovement.isJumping)
            {
                context.ChangeState(context.Jump);
            }
            if (werewolfMovement.isFalling)
            {
                context.ChangeState(context.Fall);
            }
            if (werewolfMovement.isHurting)
            {
                context.ChangeState(context.Hurt);
            }
            if (werewolfMovement.isDeath)
            {
                context.ChangeState(context.Death);
            }
        }

        public void FixedExecute()
        {
            float random = Random.Range(2f, 6f);
            if (!werewolfMovement.MovingToTarget(random))
            {
                context.ChangeState(context.Down);
            }
        }

        public void Exit()
        {
        }
    }
}
