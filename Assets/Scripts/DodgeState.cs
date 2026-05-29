using UnityEngine;

namespace StateSystem
{
    public class DodgeState : IState
    {
        private static readonly int DodgeHash = Animator.StringToHash("Dodge");
        private Animator _animator;

        private PlayerStateMachine context;
        private PlayerController playerController;
        

        public DodgeState(PlayerStateMachine context, PlayerController playerController) 
        {
            this.context = context;
            this.playerController = playerController;
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(DodgeHash, 0, 0f);
            playerController.DodgeEffect(); 
            playerController._playerStatsManager.dogdeStamina();
        }

        public void Execute()
        {
            AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
           
            // if (playerController.wasJumpPressed && playerController.isGrounding )
            // {
            //     context.ChangeState(context.Jump);
            //     playerController.resetVel();
            // }
            if (animState.IsName("Dodge") && animState.normalizedTime >= 0.6f && playerController.wasJumpPressed)
            {
                context.ChangeState(context.Jump);
            }
            if (!playerController.isGrounding)
            {
                context.ChangeState(context.Fall);
            }
            if (animState.IsName("Dodge") && animState.normalizedTime >= 1f && !playerController.isDodging)
            {
                context.ChangeState(context.Idle);
            }
            if (animState.IsName("Dodge") && animState.normalizedTime >= 0.6f　&& playerController.wasAttackPressed)
            {
                context.ChangeState(context.Attack);
            }
            if (playerController.wasPunchPresssed)
            {
                context.ChangeState(context.Punch);
            }
            
        }

        public void FixedExecute()
        { 
            playerController.Dogding();
            playerController.Moving(false);
            playerController.SetGravityScale(0);
            
        }

        public void Exit()
        {
        }
    }
}

