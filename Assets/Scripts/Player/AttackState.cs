using UnityEngine;

namespace StateMachinePlayer
{
    public class AttackState : IState
    {
        private static readonly int Attack1Hash = Animator.StringToHash("Attack1");
        private static readonly int Attack2Hash = Animator.StringToHash("Attack2");
        private static readonly int AttackUpHash = Animator.StringToHash("AttackUp");
        private static readonly int Attack1HoldHash = Animator.StringToHash("Attack1Hold");
        private static readonly int Attack2HoldHash = Animator.StringToHash("Attack2Hold");
        private static readonly int AttackUpHoldHash = Animator.StringToHash("AttackUpHold");
        private static readonly int SheathSwordHash = Animator.StringToHash("SheathSword");
        private Animator _animator;

        private PlayerStateManager context;
        private PlayerController playerController;
        
        private bool isHolding;

        public AttackState(PlayerStateManager context, PlayerController playerController) 
        {
            this.context = context;
            this.playerController = playerController;
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            Attacking(Attack1Hash);
            isHolding = false;
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

            AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
            if (animState.shortNameHash == Attack1HoldHash || animState.shortNameHash == Attack2HoldHash || animState.shortNameHash == AttackUpHoldHash)
            {
                isHolding = true;
            }

            if (animState.shortNameHash == Attack1HoldHash && playerController.wasAttackPressed)
            {
                Attacking(Attack2Hash);
            }
            else if (animState.shortNameHash == Attack2HoldHash && playerController.wasAttackPressed)
            {
                Attacking(AttackUpHash);
            }
            else if (animState.shortNameHash == AttackUpHoldHash && playerController.wasAttackPressed)
            {
                Attacking(Attack1Hash);
            }

            if (isHolding)
            {
                if (playerController.wasParryPressed)
                {
                    context.ChangeState(context.Parry);
                    return;
                }
                if (playerController.wasDodgePressed)
                {
                    context.ChangeState(context.Dodge);
                    return;
                }
                if (playerController.wasJumpPressed || !playerController.isGrounding)
                {
                    context.ChangeState(context.Jump);
                    return;
                }
                if (playerController.isFocus)
                {
                    context.ChangeState(context.Focus);
                    return;
                }

                playerController.CheckDirectionToFace();
                if (playerController.wasPunchPresssed)
                {
                    context.ChangeState(context.Punch);
                    return;
                }

                if (animState.normalizedTime >= 1.0f)
                {
                    if (playerController.sheathSwordFeedback != null && !playerController.sheathSwordFeedback.IsPlaying)
                    {
                        playerController.sheathSwordFeedback.PlayFeedbacks();
                    }
                    _animator.Play(SheathSwordHash, 0, 0f);
                }
            }

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
        private void Attacking(int attack)
        {
            if (playerController.attackFeedback != null)
            {
                playerController.attackFeedback.PlayFeedbacks();
            }
            playerController.playerSliderBar.attackStamina();
            _animator.Play(attack, 0, 0f);
            playerController.AttackEffect();
        

        }
    }
}


