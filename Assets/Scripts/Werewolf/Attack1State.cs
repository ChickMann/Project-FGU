using UnityEngine;

namespace WerewolfStateMachine
{
    public class Attack1State : IState
    {
        private static readonly int Attack1Hash = Animator.StringToHash("Attack1");
        private static readonly int Attack12Hash = Animator.StringToHash("Attack1_2");
        private Animator _animator;

        private WerewolfStateManager context;
        private WerewolfMovement werewolfMovement;

        public Attack1State(WerewolfStateManager context, WerewolfMovement werewolfMovement) 
        {
            this.context = context;
            this.werewolfMovement = werewolfMovement;
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(Attack1Hash, 0, 0f);
            werewolfMovement.AttackEffect();
            werewolfMovement.ResetConsecutiveHurt();
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

            if (animState.shortNameHash == Attack1Hash && animState.normalizedTime >= 1.0f)
            {
                _animator.Play(Attack12Hash, 0, 0f);
                werewolfMovement.CheckDirectionToFace();
                werewolfMovement.AttackEffect();
                return;
            }

            if (animState.shortNameHash == Attack12Hash && animState.normalizedTime >= 1.0f)
            {
                if (werewolfMovement.isPlayerNear())
                {
                    werewolfMovement.ChooseNextAttack();
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
                else
                {
                    werewolfMovement.isAttack1 = false; 
                    werewolfMovement.WalkInput();
                }
            }

            if (werewolfMovement.isWalking)
            {
                context.ChangeState(context.Walk);
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
