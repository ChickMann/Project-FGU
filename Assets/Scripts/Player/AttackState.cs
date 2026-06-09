using UnityEngine;

namespace StateMachinePlayer
{
    public class AttackState : IState
    {
        private static readonly int Attack1Hash = Animator.StringToHash("Attack1");
        private static readonly int Attack2Hash = Animator.StringToHash("Attack2");
        private static readonly int AttackUpHash = Animator.StringToHash("AttackUp");
        private static readonly int SheathSwordHash = Animator.StringToHash("SheathSword");
        private Animator _animator;

        private PlayerStateManager context;
        private PlayerController playerController;
        
        private bool isAttacking ;

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
            AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
            if (animState.IsName("Attack1Hold") || animState.IsName("Attack2Hold")|| animState.IsName("AttackUpHold"))
            {
                isHolding = true;
            }
            if (isHolding && animState.normalizedTime >= 1.0f)
            {
                _animator.Play(SheathSwordHash, 0, 0f);
                isAttacking = false;
            }
            
            
            if ( animState.IsName("Attack1Hold")  && playerController.wasAttackPressed)
            {
                Attacking(Attack2Hash);
            }
            if (  animState.IsName("Attack2Hold")  && playerController.wasAttackPressed)
            {
                Attacking(AttackUpHash);
            }
            if (  animState.IsName("AttackUpHold")  && playerController.wasAttackPressed)
            {
                Attacking(Attack1Hash);
            }
            // if ( animState.IsName("Attack1") && animState.normalizedTime >= 0.5f && playerController.wasAttackPressed)
            // {
            //     Attacking(Attack2Hash);
            // }
            // if ( animState.IsName("Attack2") && animState.normalizedTime >= 0.5f && playerController.wasAttackPressed)
            // {
            //     Attacking(AttackUpHash);
            // }
            // if ( animState.IsName("AttackUp") && animState.normalizedTime >= 0.5f && playerController.wasAttackPressed)
            // {
            //     Attacking(Attack1Hash);
            // }
            if (isHolding && playerController.wasParryPressed)
            {
                context.ChangeState(context.Parry);
            }
            if (isHolding)
            {
                playerController.CheckDirectionToFace();
                if (playerController.wasPunchPresssed)
                {
                    context.ChangeState(context.Punch);
                }
            }
            // if ( (animState.IsName("Attack1") || animState.IsName("Attack2") || animState.IsName("AttackUp")) && animState.normalizedTime >= 0.5f && playerController.isAttacking)
            // {
            //     context.ChangeState(context.Attack);
            // }
            if (animState.IsName("SheathSword") && animState.normalizedTime >= 1.0f)
            {
                context.ChangeState(context.Idle);
            }

         
            // if ( (animState.IsName("Attack1") || animState.IsName("Attack2") || animState.IsName("AttackUp")) && animState.normalizedTime >= 0.3f && playerController.isParrying)
            // {
            //     context.ChangeState(context.Parry);
            // }
           


            if (isHolding && playerController.wasDodgePressed)
            {
                context.ChangeState(context.Dodge);
            }
            if ( isHolding && (playerController.wasJumpPressed　|| !playerController.isGrounding))
            {
                context.ChangeState(context.Jump);
            }
            if (playerController.wasHurted)
            {
                context.ChangeState(context.Hurt);
            }
            if (playerController.isDeath)
            {
                context.ChangeState(context.Death);
            }
            if (isHolding && playerController.isFocus)
            {
                context.ChangeState(context.Focus);
            }
        }

        public void FixedExecute()
        { 
            playerController.Moving(false);
            
        }

        public void Exit()
        {
            isAttacking = false;
        }
        private void Attacking(int attack)
        {
            playerController._playerStatsManager.attackStamina();
            _animator.Play(attack, 0, 0f);
            playerController.AttackEffect();
        

        }
    }
}


