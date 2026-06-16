using StateMachinePlayer;
using UnityEngine;


namespace StateMachinePlayer
{
    public class WallSlideState : IState
    {
        private static readonly int WallSlideHash = Animator.StringToHash("WallSlide");
        private Animator _animator;

        private PlayerStateManager context;
        private PlayerController playerController;
        
        public WallSlideState(PlayerStateManager context, PlayerController playerController) 
        {
            this.context = context;
            this.playerController = playerController;
        }
        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(WallSlideHash, 0, 0f);
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
                context.ChangeState(context.Hurt);
                return;
            }

            if (playerController.isGrounding || !playerController.isWallSliding)
            {
                context.ChangeState(context.Idle);
                return;
            }

            if (playerController.facingDirection != playerController._moveDirectionX && playerController._moveDirectionX != 0)
            {
                context.ChangeState(context.Fall);
                return;
            }
        }

        public void FixedExecute()
        {
            playerController.WallSliding();
            playerController.playerSliderBar.IncreaseStamina(0.1f);
            
        }

        public void Exit()
        {
        }
    }

}

