using Unity.VisualScripting;
using UnityEngine;

namespace StateMachinePlayer
{
    public class FocusState : IState
    {
        private static readonly int FocusHash = Animator.StringToHash("Focus");
        private Animator _animator;

        private PlayerStateManager context;
        private PlayerController playerController;


        public FocusState(PlayerStateManager context, PlayerController playerController)
        {
            this.context = context;
            this.playerController = playerController;
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(FocusHash, 0, 0f);
        }

        public void Execute()
        {
            AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);

            if (playerController.isHeavyAttack)
            {
                context.ChangeState(context.HeavyAttack);
            }

        }

        public void FixedExecute()
        {
            playerController.playerSliderBar.DecreaseStamina(0.1f);
            if (playerController.playerSliderBar.IsOutStamina(playerController.playerSliderBar.StatsData
                    .heavyAttackStamina))
            {
                context.ChangeState(context.HeavyAttack);
            }
            if (playerController.wasHurted)
            {
                context.ChangeState(context.Hurt);
            }
        }

        public void Exit()
        {
        }
    }
}
