using UnityEngine;

namespace StateMachinePlayer
{
    public class GrabLedgeState : IState
    {
        private static readonly int LedgeGrabHash = Animator.StringToHash("LedgeGrab");
        private Animator _animator;

        private PlayerStateManager context;
        private PlayerController playerController;

        public GrabLedgeState(PlayerStateManager context, PlayerController playerController)
        {
            this.context = context;
            this.playerController = playerController;
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(LedgeGrabHash, 0, 0f);
            
            // Cancel movement velocity and gravity when grabbing the ledge
            playerController._rigidbody.linearVelocity = Vector2.zero;
            playerController.SetGravityScale(0);
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

            // Read vertical move direction
            float moveDirectionY = playerController._moveAction.action.ReadValue<Vector2>().y;

            if (moveDirectionY > 0.1f || playerController.wasJumpPressed)
            {
                context.ChangeState(context.LedgeClimb);
                return;
            }
            else if (moveDirectionY < -0.1f)
            {
                playerController.DisableWallSensors();
                context.ChangeState(context.Fall);
                return;
            }
        }

        public void FixedExecute()
        {
            playerController._rigidbody.linearVelocity = Vector2.zero;
            playerController.playerSliderBar.IncreaseStamina(0.3f);
        }

        public void Exit()
        {
        }
    }
}
