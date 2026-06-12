using UnityEngine;

namespace WerewolfStateMachine
{
    public class Attack3State : IState
    {
        private static readonly int Attack3Hash = Animator.StringToHash("Attack3");
        private Animator _animator;

        private WerewolfStateManager context;
        private WerewolfMovement werewolfMovement;

        public Attack3State(WerewolfStateManager context, WerewolfMovement werewolfMovement) 
        {
            this.context = context;
            this.werewolfMovement = werewolfMovement;
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(Attack3Hash, 0, 0f);
            werewolfMovement.StartAttack3Cooldown();
        }

        public void Execute()
        {
            AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
            if (animState.shortNameHash == Attack3Hash && animState.normalizedTime >= 1.0f)
            {
                if (werewolfMovement.isPlayerNear())
                {
                    werewolfMovement.ChooseNextAttack();
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
                    if (werewolfMovement.isAttack4)
                    {
                        context.ChangeState(context.Jump);
                    }
                    if (werewolfMovement.isAttack5)
                    {
                        context.ChangeState(context.Attack5);
                    }
                    if (werewolfMovement.isDeath)
                    {
                        context.ChangeState(context.Death);
                    }
                }
                else
                {
                    werewolfMovement.isAttack3 = false;
                    werewolfMovement.WalkInput();
                    
                }
            }
            if (werewolfMovement.isHurting)
            {
                context.ChangeState(context.Hurt);
            }

            if (werewolfMovement.isWalking)
            {
                context.ChangeState(context.Walk);
            }
            if (werewolfMovement.isHurting)
            {
                context.ChangeState(context.Hurt);
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
