using UnityEngine;

namespace StateMachinePlayer
{
    public class JumpState : IState
    {
        // 1. Dùng Hash thay vì String cho TẤT CẢ tương tác với Animator
        private static readonly int JumpHash = Animator.StringToHash("Jump");
        private static readonly int JumpToFallHash = Animator.StringToHash("JumpToFall");
        
        // 2. Bảo vệ dependency bằng 'readonly'
        private readonly PlayerStateManager context;
        private readonly PlayerController playerController;
        private Animator _animator;
    
        // 3. Đổi tên biến cho rõ nghĩa hơn
        private bool _isTransitioningToFall;

        public JumpState(PlayerStateManager context, PlayerController playerController) 
        {
            this.context = context;
            this.playerController = playerController;
        }

        public void Enter(Animator animator)
        {
            playerController.playerSliderBar.JumpStamina();
            _animator = animator;
            _animator.Play(JumpHash, 0, 0f);
            _isTransitioningToFall = false; 
        
        }

        public void Execute() // 4. Sửa lỗi chính tả Excute -> Execute
        {
            playerController.CheckDirectionToFace();
            
            // 5. Logic chuyển State mượt mà và an toàn
            if (playerController.isFalling  && !_isTransitioningToFall)
            {
                _animator.Play(JumpToFallHash, 0, 0f);
                _isTransitioningToFall = true;
            }

          
            if (_isTransitioningToFall)
            {
                AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
                
                if (animState.shortNameHash == JumpToFallHash && animState.normalizedTime >=  1.0f|| playerController.isGrounding)
                {
                    context.ChangeState(context.Fall);
                }
            }
            if (playerController.wasHurted)
            {
                context.ChangeState(context.Hurt);
            }
            if (playerController.isDeath)
            {
                context.ChangeState(context.Death);
            }
            if (playerController.isWallSliding && !playerController.isGrounding && playerController.isFalling)
            {
                context.ChangeState(context.WallSlide);
            }
        }

        public void FixedExecute()
        { 
            playerController.Jumping();
            playerController.Moving();
            playerController.Gravity();
           
        }

        public void Exit()
        {
            _isTransitioningToFall = false;
        }
    }
}