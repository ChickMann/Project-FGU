using UnityEngine;

namespace BanditStateMachine
{
    [RequireComponent(typeof(Animator)), RequireComponent(typeof(BanditMovement))]
    public class BanditStateManager : MonoBehaviour
    {
        private IState _currentState;
        private Animator _animator;
        private BanditMovement _movement;

        public IdleState Idle { get; private set; }
        public RunState Run { get; private set; }
        public AttackState Attack { get; private set; }
        public HurtState Hurt { get; private set; }
        public DeathState Death { get; private set; }
        public CombatIdleState CombatIdle { get; private set; }

        private void Awake()
        {
            _movement = GetComponent<BanditMovement>();
            _animator = GetComponent<Animator>();

            Idle = new IdleState(this, _movement);
            Run = new RunState(this, _movement);
            Attack = new AttackState(this, _movement);
            Hurt = new HurtState(this, _movement);
            Death = new DeathState(this, _movement);
            CombatIdle = new CombatIdleState(this, _movement);
        }

        private void Start()
        {
            ChangeState(Idle);
        }

        public void ChangeState(IState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter(_animator);
        }

        private void Update()
        {
            _currentState?.Execute();
        }

        private void FixedUpdate()
        {
            _currentState?.FixedExecute();
        }
    }
}
