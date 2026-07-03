using UnityEngine;

namespace StateMachinePlayer
{
    public class DeathState : IState
    {
      
        private static readonly int DeathHash = Animator.StringToHash("Death");
        private Animator _animator;

        private PlayerStateManager context;
        private PlayerController playerController;
        
        public DeathState(PlayerStateManager context, PlayerController playerController) 
        {
            this.context = context;
            this.playerController = playerController;
        }
        public void Enter(Animator animator)
        {
            _animator = animator;
            _animator.Play(DeathHash, 0, 0f);
        }

        public void Execute()
        {
            if (_animator == null)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
                return;
            }

            AnimatorStateInfo animState = _animator.GetCurrentAnimatorStateInfo(0);
            if (animState.shortNameHash == DeathHash && animState.normalizedTime >= 1.0f)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
            }
        }

        public void FixedExecute()
        {
            playerController.Die();
            playerController.Moving(false);
            playerController.Gravity();
        }

        public void Exit()
        {
        }
    }

}

