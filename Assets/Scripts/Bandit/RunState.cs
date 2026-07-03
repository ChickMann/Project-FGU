using UnityEngine;

namespace BanditStateMachine
{
    public class RunState : IState
    {
        private static readonly int RunHash = Animator.StringToHash("Run");
        private Animator _animator;
        private BanditStateManager context;
        private BanditMovement movement;

        public RunState(BanditStateManager context, BanditMovement movement)
        {
            this.context = context;
            this.movement = movement;
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator?.Play(RunHash, 0, 0f);
            movement.isRunning = true;
        }

        public void Execute()
        {
            if (movement.isDeath) { context.ChangeState(context.Death); return; }
            if (movement.isHurting) { context.ChangeState(context.Hurt); return; }

            movement.CheckDirectionToFace();

            if (movement.ShouldJumpToTarget())
                movement.Jumping();

            if (movement.CanAttack())
            {
                movement.TriggerAttack();
                context.ChangeState(context.Attack);
                return;
            }

            if (movement.IsPlayerInRange(movement.attackRange))
            {
                context.ChangeState(context.CombatIdle);
                return;
            }

            if (!movement.IsPlayerInRange(movement.detectionRange))
            {
                context.ChangeState(context.Idle);
            }
        }

        public void FixedExecute()
        {
            movement.MovingToTarget(movement.combatRange);
        }

        public void Exit()
        {
            movement.isRunning = false;
            movement.StopMoving();
        }
    }
}
