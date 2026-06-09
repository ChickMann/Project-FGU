using UnityEngine;

namespace WerewolfStateMachine
{
    public class WalkState : IState
    {
        private static readonly int WalkHash = Animator.StringToHash("Walk");
        private Animator _animator;

        private WerewolfStateManager context;
        private WerewolfMovement werewolfMovement;


        public WalkState(WerewolfStateManager context, WerewolfMovement werewolfMovement) 
        {
            this.context = context;
            this.werewolfMovement = werewolfMovement;
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(WalkHash, 0, 0f);
        }

        public void Execute()
        {
            werewolfMovement.CheckDirectionToFace();
            if (werewolfMovement.isAttack1)
            {
                context.ChangeState(context.Attack1);
            }
            if (werewolfMovement.isAttack2)
            {
                context.ChangeState(context.Attack2);
            }
            if (werewolfMovement.isAttack3)
            {
                context.ChangeState(context.Attack3);
            }
            if (!werewolfMovement.isWalking)
            {
                context.ChangeState(context.Idle);
            }
            else if (werewolfMovement.isRunning)
            {
                context.ChangeState(context.WalkToRun);
            }
            else if (werewolfMovement.isJumping)
            {
                context.ChangeState(context.Jump);
            }
            else if (werewolfMovement.isFalling)
            {
                context.ChangeState(context.Fall);
            }
        }

        public void FixedExecute()
        {
            if (!werewolfMovement.MovingToTarget(1.5f))
            {
               werewolfMovement.RandomAttack();
            }
        }

        public void Exit()
        {
            werewolfMovement.isWalking=false;
        }
    }
}

