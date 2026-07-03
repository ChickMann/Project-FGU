using UnityEngine;

namespace BanditStateMachine
{
    public class DeathState : IState
    {
        private static readonly int DeathHash = Animator.StringToHash("Death");
        private Animator _animator;
        private BanditStateManager context;
        private BanditMovement movement;

        public DeathState(BanditStateManager context, BanditMovement movement)
        {
            this.context = context;
            this.movement = movement;
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator?.Play(DeathHash, 0, 0f);
            movement.StopMoving();
            movement.rb.linearVelocity = new Vector2(0f, movement.rb.linearVelocity.y);

            if (movement.slider != null && movement.slider.healthBar != null)
            {
                movement.slider.healthBar.gameObject.SetActive(false);
            }
        }

        public void Execute() { }

        public void FixedExecute() { }

        public void Exit() { }
    }
}
