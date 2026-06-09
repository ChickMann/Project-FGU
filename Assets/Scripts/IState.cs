using UnityEngine;

public interface IState
    {
        void Enter(Animator animator);
        void Execute();
        void FixedExecute();
        void Exit();
    }


