using UnityEngine;

namespace StateMachinePlayer
{
    public class SheathSwordState : IState
    {
        private static readonly int SheathSwordHash = Animator.StringToHash("SheathSword");
        private Animator _animator;

        private PlayerStateManager context;
        private PlayerController playerController;

        public SheathSwordState(PlayerStateManager context, PlayerController playerController) 
        {
            this.context = context;
            this.playerController = playerController;
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(SheathSwordHash, 0, 0f);
            if (playerController.playerFeedbackManager != null) 
            {
                playerController.playerFeedbackManager.PlayFeedback(PlayerFeedbackType.SheathSword);
            }
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
            if (playerController.wasDodgePressed)
            {
                context.ChangeState(context.Dodge);
                return;
            }
            if (playerController.wasPunchPresssed)
            {
                context.ChangeState(context.Punch);
                return;
            }

            AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
            if (animState.shortNameHash == SheathSwordHash && animState.normalizedTime >= 1.0f)
            {
                context.ChangeState(context.Idle);
                return;
            }
        }

        public void FixedExecute()
        {
            playerController.Moving(false);
        }

        public void Exit()
        {
        }
    }
}
