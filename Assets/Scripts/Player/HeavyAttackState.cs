using UnityEngine;


namespace StateMachinePlayer
{

    public class HeavyAttackState : IState
    {
        private static readonly int HeavyAttackHash = Animator.StringToHash("HeavyAttack");
        private static readonly int HeavyAttackHoldHash = Animator.StringToHash("HeavyAttackHold");
        private Animator _animator;

        private PlayerStateManager context;
        private PlayerController playerController;
        
        
        public HeavyAttackState(PlayerStateManager context, PlayerController playerController) 
        {
            this.context = context;
            this.playerController = playerController;
        }
        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(HeavyAttackHash, 0, 0f);
            playerController.playerSliderBar.heavyattackStamina();

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
            
            if (animState.shortNameHash == HeavyAttackHoldHash && animState.normalizedTime >= 1.0f)
            {
                context.ChangeState(context.SheathSword);
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
