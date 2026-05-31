using StateSystem;
using UnityEngine;


namespace StateSystem
{
    public class WallSlideState : IState
    {
        private static readonly int WallSlideHash = Animator.StringToHash("WallSlide");
        private Animator _animator;

        private PlayerStateMachine context;
        private PlayerController playerController;
        
        public WallSlideState(PlayerStateMachine context, PlayerController playerController) 
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
            AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
            if (playerController.isGrounding || !playerController.isWallSliding )
            {
                context.ChangeState(context.Idle);
            }

            if (playerController.facingDirection != playerController._moveDirectionX && 
                playerController._moveDirectionX !=0
                )
            {
                context.ChangeState(context.Fall);
            }
            if (playerController.wasHurted)
            {
                context.ChangeState(context.Hurt);
            }


        }

        public void FixedExecute()
        {
            playerController.WallSliding();
            playerController._playerStatsManager.IncreaseStamina(0.1f);
            
        }

        public void Exit()
        {
        }
    }

}

