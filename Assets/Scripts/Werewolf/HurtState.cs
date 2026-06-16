using UnityEngine;

namespace WerewolfStateMachine
{
    public class HurtState : IState
    {
        private static readonly int HurtHash = Animator.StringToHash("Hurt");
        private Animator _animator;

        private WerewolfStateManager context;
        private WerewolfMovement werewolfMovement;

        public HurtState(WerewolfStateManager context, WerewolfMovement werewolfMovement) 
        {
            this.context = context;
            this.werewolfMovement = werewolfMovement;
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(HurtHash, 0, 0f);
            werewolfMovement.slider.TakeDamage();
            if (werewolfMovement.isVer2)
            {
                werewolfMovement.isVer2 = false;
            }

            werewolfMovement.consecutiveHurtCount++;
            if (!werewolfMovement.isDeath && werewolfMovement.consecutiveHurtCount >= 2 && Random.value < 0.3f)
            {
                werewolfMovement.ClearHurt();
                werewolfMovement.ChooseNextAttack();
                if (werewolfMovement.isAttack1)
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
                else if (werewolfMovement.isAttack4)
                {
                    context.ChangeState(context.Jump);
                }
                else if (werewolfMovement.isAttack5)
                {
                    context.ChangeState(context.Attack5);
                }
                else
                {
                    context.ChangeState(context.Idle);
                }
            }
        }

        public void Execute()
        {
            if (werewolfMovement.isDeath)
            {
                context.ChangeState(context.Death);
                return;
            }

            AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
            if (animState.shortNameHash == HurtHash && animState.normalizedTime >= 1.0f)
            {
                if (!werewolfMovement.wasTransitionedToV2 && werewolfMovement.slider != null && werewolfMovement.slider.currentHealth <= werewolfMovement.slider.maxHealth * 0.5f)
                {
                    werewolfMovement.wasTransitionedToV2 = true;
                    werewolfMovement.isVer2 = true;
                    context.ChangeState(context.TransVer2);
                    return;
                }

                if (werewolfMovement.isAttack2Ready() && werewolfMovement.isPlayerNear())
                {
                    werewolfMovement.Atack2Input();
                    werewolfMovement.ClearHurt();
                    context.ChangeState(context.Attack3);
                    return;
                }
                else
                {
                    werewolfMovement.ChooseNextAttack();
                    context.ChangeState(context.Idle);
                    return;
                }
            }
        }

        public void FixedExecute()
        {
            werewolfMovement.Hurting();
        }

        public void Exit()
        {
        }
    }

}

