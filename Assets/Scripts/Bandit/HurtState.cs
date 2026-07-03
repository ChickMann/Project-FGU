using UnityEngine;

namespace BanditStateMachine
{
    public class HurtState : IState
    {
        private static readonly int HurtHash = Animator.StringToHash("Hurt");
        private Animator _animator;
        private BanditStateManager context;
        private BanditMovement movement;

        public HurtState(BanditStateManager context, BanditMovement movement)
        {
            this.context = context;
            this.movement = movement;
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator?.Play(HurtHash, 0, 0f);

            if (movement.slider != null)
                movement.slider.TakeDamage(20f);
        }

        public void Execute()
        {
            if (_animator == null)
            {
                movement.ClearHurt();
                context.ChangeState(movement.isDeath ? context.Death : context.Run);
                return;
            }

            AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
            if (animState.shortNameHash == HurtHash && animState.normalizedTime >= 1.0f)
            {
                movement.ClearHurt();
                if (movement.isDeath)
                {
                    context.ChangeState(context.Death);
                }
                else
                {
                    context.ChangeState(movement.IsPlayerInRange(movement.attackRange) ? context.CombatIdle : context.Run);
                }
            }
        }

        public void FixedExecute()
        {
            movement.Hurting();
        }

        public void Exit() { }
    }
}
