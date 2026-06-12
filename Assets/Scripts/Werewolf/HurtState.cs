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
        }

        public void Execute()
        {
        

            AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
            if (werewolfMovement.isDeath)
            {
                context.ChangeState(context.Death);
            }
            if (animState.shortNameHash == HurtHash && animState.normalizedTime >= 1.0f)
            {
                if (werewolfMovement.isAttack2Ready() && werewolfMovement.isPlayerNear())
                {
                    werewolfMovement.Atack2Input();
                    werewolfMovement.ClearHurt();
                    context.ChangeState(context.Attack3);
                }
                else
                {
                    werewolfMovement.ChooseNextAttack();
                context.ChangeState(context.Idle);
                    
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

