using UnityEngine;

namespace WerewolfStateMachine
{
    public class DeathState : IState
    {
        private static readonly int DeathHash = Animator.StringToHash("Death");
        private Animator _animator;

        private WerewolfStateManager context;
        private WerewolfMovement werewolfMovement;

        public DeathState(WerewolfStateManager context, WerewolfMovement werewolfMovement) 
        {
            this.context = context;
            this.werewolfMovement = werewolfMovement;
        }
        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(DeathHash, 0, 0f);
            if (werewolfMovement.deadFeedback != null)
            {
                werewolfMovement.deadFeedback.PlayFeedbacks();
            }
        }

        public void Execute()
        {
        }

        public void FixedExecute()
        {
        }

        public void Exit()
        {
        }
    }
}
