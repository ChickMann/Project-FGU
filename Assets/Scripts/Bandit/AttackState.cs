using UnityEngine;

namespace BanditStateMachine
{
    public class AttackState : IState
    {
        private static readonly int AttackHash = Animator.StringToHash("Attack");
        private Animator _animator;
        private BanditStateManager context;
        private BanditMovement movement;
        private float _timeInState;

        public AttackState(BanditStateManager context, BanditMovement movement)
        {
            this.context = context;
            this.movement = movement;
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            _timeInState = 0f;
            _animator?.Play(AttackHash, 0, 0f);
            movement.StopMoving();
        }

        public void Execute()
        {
            if (movement.isDeath) { context.ChangeState(context.Death); return; }
            if (movement.isHurting) { context.ChangeState(context.Hurt); return; }

            _timeInState += Time.deltaTime;

            if (_animator == null) { context.ChangeState(context.CombatIdle); return; }

            AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
            bool animFinished = animState.shortNameHash == AttackHash && animState.normalizedTime >= 1.0f;

            if (animFinished || _timeInState >= 0.8f)
                context.ChangeState(context.CombatIdle);
        }

        public void FixedExecute() { }

        public void Exit()
        {
            movement.ClearAttack();
        }
    }
}
