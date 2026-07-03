using UnityEngine;

namespace BanditStateMachine
{
    public class CombatIdleState : IState
    {
        private static readonly int CombatIdleHash = Animator.StringToHash("Combat Idle");
        private Animator _animator;
        private BanditStateManager context;
        private BanditMovement movement;

        public CombatIdleState(BanditStateManager context, BanditMovement movement)
        {
            this.context = context;
            this.movement = movement;
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator?.Play(CombatIdleHash, 0, 0f);
            movement.StopMoving();
            movement.isCombatIdle = true;
        }

        public void Execute()
        {
            if (movement.isDeath) { context.ChangeState(context.Death); return; }
            if (movement.isHurting) { context.ChangeState(context.Hurt); return; }

            movement.CheckDirectionToFace();

            if (movement.CanAttack())
            {
                movement.TriggerAttack();
                context.ChangeState(context.Attack);
                return;
            }

            if (!movement.IsPlayerInRange(movement.attackRange))
            {
                context.ChangeState(context.Run);
            }
        }

        public void FixedExecute() { }

        public void Exit()
        {
            movement.isCombatIdle = false;
        }
    }
}
