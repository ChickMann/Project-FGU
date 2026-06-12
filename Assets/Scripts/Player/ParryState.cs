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
            AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
            if (playerController.wasHurtedHeavyAttack)
            {
                context.ChangeState(context.Hurt);
            }
            if (animState.IsName("ParryStance") && animState.normalizedTime >= 1.0f)
            {
                _animator.Play(SheathSwordHash, 0, 0f);
            }
            if  (animState.IsName("ParryStance") && playerController.wasHurted && !playerController.wasHurtedHeavyAttack)
            {
                _animator.Play(ParryHash, 0, 0f);
                playerController.Parrying();
            }
            if (animState.IsName("Parry") && animState.normalizedTime >= 1.0f)
            {
                _animator.Play(SheathSwordHash, 0, 0f);
            }
            if (animState.IsName("SheathSword") && animState.normalizedTime >= 1.0f)
            {
                context.ChangeState(context.Idle);
            }
            if (animState.IsName("Parry") && animState.normalizedTime >= 0.3f && playerController.wasAttackPressed)
            {
                context.ChangeState(context.Attack);
            }
            if (animState.IsName("Parry") && animState.normalizedTime >= 0.1f && playerController.wasParryPressed)
            {
                context.ChangeState(context.Parry);
            }
            
            if (animState.IsName("Parry") && playerController.wasDodgePressed)
            {
                context.ChangeState(context.Dodge);
            }
            if (animState.IsName("SheathSword") && playerController.wasHurted)
            {
                context.ChangeState(context.Hurt);
            }
            if (animState.IsName("Parry")&& playerController.wasPunchPresssed)
            {
                context.ChangeState(context.Punch);
            }
            if (animState.IsName("Parry")&& playerController.isFocus)
            {
                context.ChangeState(context.Focus);
            }

        }

        public void FixedExecute()
        {
            playerController.Moving(false);
            AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
            if (animState.IsName("Parry"))
            {
                playerController.playerSliderBar.IncreaseStamina(0.7f);
            }
        }

        public void Exit()
        {
        }
    }
    
}


