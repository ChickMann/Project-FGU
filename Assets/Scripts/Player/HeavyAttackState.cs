using UnityEngine;


namespace StateMachinePlayer
{

    public class HeavyAttackState : IState
    {
        private static readonly int HeavyAttackHash = Animator.StringToHash("HeavyAttack");
        private static readonly int SheathSwordHash = Animator.StringToHash("SheathSword");
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
            AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
            
            if (animState.IsName("HeavyAttackHold") && animState.normalizedTime >= 1.0f)
            {
                _animator.Play(SheathSwordHash, 0, 0f);
            }
            if (animState.IsName("SheathSword") && animState.normalizedTime >= 1.0f)
            {
                context.ChangeState(context.Idle);
            }
            if (playerController.wasHurted)
            {
                context.ChangeState(context.Hurt);
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
