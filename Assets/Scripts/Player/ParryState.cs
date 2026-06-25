using UnityEngine;

namespace StateMachinePlayer
{
    public class ParryState : IState
    {
        private static readonly int ParryStanceHash = Animator.StringToHash("ParryStance");
        private static readonly int SheathSwordHash = Animator.StringToHash("SheathSword");
        private static readonly int ParryHash = Animator.StringToHash("Parry");
        private Animator _animator;

        private PlayerStateManager context;
        private PlayerController playerController;

        public ParryState(PlayerStateManager context, PlayerController playerController) 
        {
            this.context = context;
            this.playerController = playerController;
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(ParryStanceHash, 0, 0f);
            playerController.SetParryColdown();
            
        }

        public void Execute()
        {
            if (playerController.isDeath)
            {
                context.ChangeState(context.Death);
                return;
            }
            if (playerController.wasHurtedHeavyAttack)
            {
                context.ChangeState(context.Hurt);
                return;
            }

            AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);

            if (animState.shortNameHash == ParryStanceHash && playerController.wasHurted && !playerController.wasHurtedHeavyAttack)
            {
                _animator.Play(ParryHash, 0, 0f);
                playerController.Parrying();
            }

            if (animState.shortNameHash == ParryStanceHash && animState.normalizedTime >= 1.0f)
            {
                if(playerController.playerFeedbackManager !=null) playerController.playerFeedbackManager.PlayFeedback(PlayerFeedbackType.SheathSword);
                _animator.Play(SheathSwordHash, 0, 0f);
            }

            if (animState.shortNameHash == ParryHash && animState.normalizedTime >= 1.0f)
            {
                if(playerController.playerFeedbackManager !=null) playerController.playerFeedbackManager.PlayFeedback(PlayerFeedbackType.SheathSword);
                _animator.Play(SheathSwordHash, 0, 0f);
            }

            if (animState.shortNameHash == SheathSwordHash)
            {
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
                if (playerController.wasParryPressed)
                {
                    context.ChangeState(context.Attack);
                    return;
                }
                if (animState.normalizedTime >= 1.0f)
                {
                    context.ChangeState(context.Idle);
                    return;
                }
            }

            if (animState.shortNameHash == ParryHash)
            {
                if (animState.normalizedTime >= 0.3f && playerController.wasAttackPressed)
                {
                    context.ChangeState(context.Attack);
                    return;
                }
                if (animState.normalizedTime >= 0.5f&& playerController.wasParryPressed)
                {
                    context.ChangeState(context.Parry);
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
                if (playerController.isFocus)
                {
                    context.ChangeState(context.Focus);
                    return;
                }
            }
        }

        public void FixedExecute()
        {
            playerController.Moving(false);
            AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
            if (animState.shortNameHash == ParryHash)
            {
                playerController.playerSliderBar.IncreaseStamina(0.7f);
            }
        }

        public void Exit()
        {
        }
    }
    
}


