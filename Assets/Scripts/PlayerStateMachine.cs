using System;
using UnityEngine;


namespace StateSystem
{
    [RequireComponent( typeof(Animator)),RequireComponent(typeof(PlayerController))]
    public class PlayerStateMachine : MonoBehaviour
    {
        private IState _currentState;
       [SerializeField] private Animator _animator;
        public IdleState Idle { get; private set; }
        public WalkState Walk { get; private set; }
        public RunState Run { get; private set; }
        public RunStopState RunStop { get; private set; }
        public CrouchState Crouch { get; private set; }
        public StandUpState StandUp { get; private set; }
        public JumpState Jump { get; private set; }
        public FallState Fall { get; private set; }
        public DodgeState Dodge { get; private set; }
        public ParryState Parry { get; private set; }
        public AttackState Attack { get; private set; }
        public HurtState Hurt { get; private set; }
        public DeathState Death { get; private set; }
        public PunchState Punch { get; private set; }
        public WallSlideState WallSlide { get; private set; }
        
        public PlayerController playerController;



        private void Awake()
        {
            _animator = GetComponent<Animator>();
            playerController = GetComponent<PlayerController>();
            Idle = new IdleState(this,playerController);
            Walk = new WalkState(this,playerController);
            Run = new RunState(this,playerController);
            RunStop = new RunStopState(this,playerController);
            Crouch = new CrouchState(this,playerController);
            StandUp = new StandUpState(this,playerController);
            Jump = new JumpState(this,playerController);
            Fall = new FallState(this,playerController);
            Dodge = new DodgeState(this,playerController);
            Parry = new ParryState(this,playerController);
            Attack = new AttackState(this,playerController);
            Hurt = new HurtState(this,playerController);
            Death = new DeathState(this,playerController);
            WallSlide = new WallSlideState(this,playerController);
            Punch = new PunchState(this,playerController);
        }

        private void Start()
        {
            ChangeState(Idle);
        }

      
        public void ChangeState(IState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            
            _currentState?.Enter(_animator);
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

