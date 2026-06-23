using UnityEngine;

namespace StateMachinePlayer
{
    public class LedgeClimbState : IState
    {
        private static readonly int LedgeClimbHash = Animator.StringToHash("LedgeClimb");
        private Animator _animator;

        private PlayerStateManager context;
        private PlayerController playerController;

        public LedgeClimbState(PlayerStateManager context, PlayerController playerController)
        {
            this.context = context;
            this.playerController = playerController;
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(LedgeClimbHash, 0, 0f);

            playerController._rigidbody.linearVelocity = Vector2.zero;
            playerController.SetGravityScale(0);
            playerController.DisableWallSensors();
        }

        public void Execute()
        {
            if (playerController.isDeath)
            {
                context.ChangeState(context.Death);
                return;
            }
            if (playerController.wasHurted)
            {
                playerController.SetGravityScale(playerController.data.gravityScale);
                context.ChangeState(context.Hurt);
                return;
            }

            AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
            if (animState.shortNameHash == LedgeClimbHash && animState.normalizedTime >= 1.0f)
            {
                context.ChangeState(context.Idle);
                return;
            }
        }

        public void FixedExecute()
        {
            playerController.SetGravityScale(0);
            playerController._rigidbody.linearVelocity = Vector2.zero;
        }

        public void Exit()
        {
        }
    }
}
