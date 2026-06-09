using UnityEngine;

namespace WerewolfStateMachine
{
    public class DownState : IState
    {
        private static readonly int DownHash = Animator.StringToHash("Down");
        private Animator _animator;

        private WerewolfStateManager context;
        private WerewolfMovement werewolfMovement;

        public DownState(WerewolfStateManager context, WerewolfMovement werewolfMovement) 
        {
            this.context = context;
            this.werewolfMovement = werewolfMovement;
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(DownHash, 0, 0f);
        }

        public void Execute()
        {
            werewolfMovement.CheckDirectionToFace();
            AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
            if (animState.shortNameHash == DownHash && animState.normalizedTime >= 1.0f)
            {
               
                int random = Random.Range(0, 2);
                if (random == 0)
                {
                    werewolfMovement.JumpInput();
                    context.ChangeState(context.Jump);
                }
                else
                {
                    werewolfMovement.DodgeInput();
                    context.ChangeState(context.Attack5);
                }
              
            }
        }

        public void FixedExecute()
        {
        }

        public void Exit()
        {
        }
    }
}
