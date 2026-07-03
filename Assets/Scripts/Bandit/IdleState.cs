using UnityEngine;

namespace BanditStateMachine
{
    public class IdleState : IState
    {
        private static readonly int IdleHash = Animator.StringToHash("Idle");
        private Animator _animator;
        private BanditStateManager context;
        private BanditMovement movement;

        public IdleState(BanditStateManager context, BanditMovement movement)
        {
            this.context = context;
            this.movement = movement;
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator?.Play(IdleHash, 0, 0f);
            movement.StopMoving();
            movement.ClearAttack();
            movement.isCombatIdle = false;
        }

        public void Execute()
        {
            if (movement.isDeath) { context.ChangeState(context.Death); return; }
            if (movement.isHurting) { context.ChangeState(context.Hurt); return; }

            if (movement.DetectPlayer())
            {
                context.ChangeState(context.Run);
            }
        }

        public void FixedExecute()
        {
            movement.StopMoving();
        }

        public void Exit() { }
    }
}
