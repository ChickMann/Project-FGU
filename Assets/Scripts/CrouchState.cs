using Unity.VisualScripting;
using UnityEngine;

namespace StateSystem
{
    public class CrouchState : IState
    {
        private static readonly int CrouchHash = Animator.StringToHash("Crouch");
        private Animator _animator;

        private PlayerStateMachine context;
        private PlayerController playerController;

        public CrouchState(PlayerStateMachine context, PlayerController playerController) 
        {
            this.context = context;
            this.playerController = playerController;
        }

        public void Enter(Animator animator)
        {
            Debug.Log("Crouch Enter");
            _animator = animator;
            _animator.Play(CrouchHash, 0, 0f);
        }

        public void Execute()
        {
            Debug.Log("Crouch execute");
            playerController.CheckDirectionToFace();
            if (!playerController.isCrouching)
            {
                context.ChangeState(context.StandUp);
            }
            if (playerController.wasDodgePressed)
            {
                context.ChangeState(context.Dodge);
            }

            if (playerController.wasParryPressed)
            {
                context.ChangeState(context.Parry);
            }
            if (playerController.wasAttackPressed)
            {
                context.ChangeState(context.Attack);
            }
            if (playerController.wasHurted)
            {
                context.ChangeState(context.Hurt);
            }
            if (playerController.isDeath)
            {
                context.ChangeState(context.Death);
            }
        }

        public void FixedExecute()
        { 
           playerController.Moving(false);
        }

        public void Exit()
        {
            Debug.Log("Crouch exit");
        }
    }
}

