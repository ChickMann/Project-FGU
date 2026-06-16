using UnityEngine;

namespace StateMachinePlayer
{
    public class DodgeState : IState
    {
        private static readonly int DodgeHash = Animator.StringToHash("Dodge");
        private Animator _animator;

        private PlayerStateManager context;
        private PlayerController playerController;
        

        public DodgeState(PlayerStateManager context, PlayerController playerController) 
        {
            this.context = context;
            this.playerController = playerController;
        }

        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(DodgeHash, 0, 0f);
            playerController.DodgeEffect(); 
            playerController.playerSliderBar.dogdeStamina();
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

            if (!playerController.isGrounding)
            {
                context.ChangeState(context.Fall);
                return;
            }

            AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
           
            if (animState.shortNameHash == DodgeHash)
            {
                if (animState.normalizedTime >= 0.6f)
                {
                    if (playerController.wasJumpPressed)
                    {
                        context.ChangeState(context.Jump);
                        return;
                    }
                    if (playerController.wasAttackPressed)
                    {
                        context.ChangeState(context.Attack);
                        return;
                    }
                }

                if (animState.normalizedTime >= 1f && !playerController.isDodging)
                {
                    if (playerController._moveDirectionX != 0)
                    {
                        context.ChangeState(context.Walk);
                        return;
                    }
                    else
                    {
                        context.ChangeState(context.Idle);
                        return;
                    }
                }
            }

            if (playerController.wasPunchPresssed)
            {
                context.ChangeState(context.Punch);
                return;
            }
        }

        public void FixedExecute()
        { 
            playerController.Dogding();
            playerController.Moving(false);
            playerController.SetGravityScale(0);
            
        }

        public void Exit()
        {
        }
    }
}

