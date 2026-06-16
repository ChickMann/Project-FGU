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

            werewolfMovement.CheckDirectionToFace();

            if (werewolfMovement.isWalking)
            {
                context.ChangeState(context.Walk);
                return;
            }
            if (werewolfMovement.isRunning)
            {
                context.ChangeState(context.Run);
                return;
            }
            if (werewolfMovement.isJumping)
            {
                context.ChangeState(context.Jump);
                return;
            }
            if (werewolfMovement.isFalling)
            {
                context.ChangeState(context.Fall);
                return;
            }
            if (werewolfMovement.isAttack1)
            {
                context.ChangeState(context.Attack1);
                return;
            }
            if (werewolfMovement.isAttack2)
            {
                context.ChangeState(context.Attack2);
                return;
            }
            if (werewolfMovement.isAttack3)
            {
                context.ChangeState(context.Attack3);
                return;
            }
            if (werewolfMovement.isAttack4)
            {
                context.ChangeState(context.Jump);
                return;
            }
            if (werewolfMovement.isAttack5)
            {
                context.ChangeState(context.Attack5);
                return;
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

