using UnityEngine;

namespace WerewolfStateMachine
{
    public class DownState : IState
    {
        private static readonly int DownHash = Animator.StringToHash("Down");
        private Animator _animator;

        private WerewolfStateManager context;
        private WerewolfMovement werewolfMovement;

        public DownState(WerewolfStateManager context, WerewolfMovement werewolfMovement) 
        {
            this.context = context;
            this.werewolfMovement = werewolfMovement;
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(DownHash, 0, 0f);
        }

        public void Execute()
        {
            werewolfMovement.CheckDirectionToFace();
            AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
            if (werewolfMovement.isHurting)
            {
                context.ChangeState(context.Hurt);
            }
            if (animState.shortNameHash == DownHash && animState.normalizedTime >= 1.0f)
            {
               werewolfMovement.RandomAttackV2();
                if (werewolfMovement.isJumping || werewolfMovement.isAttack4)
                {
                    context.ChangeState(context.Jump);
                }
                else if (werewolfMovement.isDodging || werewolfMovement.isAttack5)
                {
                    context.ChangeState(context.Attack5);
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
