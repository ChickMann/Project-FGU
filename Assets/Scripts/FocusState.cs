using Unity.VisualScripting;
using UnityEngine;

namespace StateSystem
{
    public class FocusState : IState
    {
        private static readonly int FocusHash = Animator.StringToHash("Focus");
        private Animator _animator;

        private PlayerStateMachine context;
        private PlayerController playerController;


        public FocusState(PlayerStateMachine context, PlayerController playerController)
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
            playerController._playerStatsManager.DecreaseStamina(0.1f);
            if (playerController._playerStatsManager.IsOutStamina(playerController._playerStatsManager.StatsData
                    .heavyAttackStamina))
            {
                context.ChangeState(context.HeavyAttack);
            }
        }

        public void Exit()
        {
        }
    }
}
