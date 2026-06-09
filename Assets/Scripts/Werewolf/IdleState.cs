using UnityEngine;

namespace WerewolfStateMachine
{
    public class IdleState : IState
    {
        private static readonly int IdleHash = Animator.StringToHash("Idle");

        private Animator _animator;

        private WerewolfStateManager context;
        private WerewolfMovement werewolfMovement;
        
 
        public IdleState(WerewolfStateManager context, WerewolfMovement werewolfMovement) 
        {
            this.context = context;
            this.werewolfMovement = werewolfMovement;
        }

      
        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(IdleHash,0,0f);
        }

        public void Execute()
        {
            werewolfMovement.CheckDirectionToFace();
            if (werewolfMovement.isWalking)
            {
                context.ChangeState(context.Walk);
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
            else if (werewolfMovement.isAttack1)
            {
                context.ChangeState(context.Attack1);
            }
            else if (werewolfMovement.isAttack2)
            {
                context.ChangeState(context.Attack2);
            }
            else if (werewolfMovement.isAttack3)
            {
                context.ChangeState(context.Attack3);
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

