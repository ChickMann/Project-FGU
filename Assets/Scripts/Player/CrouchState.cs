using Unity.VisualScripting;
using UnityEngine;

namespace StateMachinePlayer
{
    public class CrouchState : IState
    {
        private static readonly int CrouchHash = Animator.StringToHash("Crouch");
        private Animator _animator;

        private PlayerStateManager context;
        private PlayerController playerController;

        public CrouchState(PlayerStateManager context, PlayerController playerController) 
        {
            this.context = context;
            this.playerController = playerController;
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(CrouchHash, 0, 0f);
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

            playerController.CheckDirectionToFace();

            if (playerController.wasDodgePressed)
            {
                context.ChangeState(context.Dodge);
                return;
            }
            if (playerController.wasParryPressed)
            {
                context.ChangeState(context.Parry);
                return;
            }
            if (playerController.wasAttackPressed)
            {
                context.ChangeState(context.Attack);
                return;
            }
            if (!playerController.isCrouching)
            {
                context.ChangeState(context.StandUp);
                return;
            }
        }

        public void FixedExecute()
        { 
           playerController.Moving(false);
           playerController.playerSliderBar.IncreaseStamina(0.7f);
        }

        public void Exit()
        {
        }
    }
}

