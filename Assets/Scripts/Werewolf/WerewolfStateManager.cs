using System;
using UnityEngine;

namespace WerewolfStateMachine
{
    [RequireComponent( typeof(Animator)),RequireComponent(typeof(WerewolfMovement))]
    public class WerewolfStateManager : MonoBehaviour
    {
       private IState _currentState;
       private Animator _animator;
       
       private WerewolfMovement _movement;
       
       [Header("States")]
       public IdleState Idle { get; private set; }
       public WalkState Walk { get; private set; }
       public RunState Run { get; private set; }
       public Attack1State Attack1 { get; private set; }
       public Attack2State Attack2 { get; private set; }
       public Attack3State Attack3 { get; private set; }
       public Attack4State Attack4 { get; private set; }
       public Attack5State Attack5 { get; private set; }
       public DownState Down { get; private set; }
       public JumpState Jump { get; private set; }
       public FallState Fall { get; private set; }
       public LandingState Landing { get; private set; }
       public RunToWalkState RunToWalk { get; private set; }
       public WalkToRunState WalkToRun { get; private set; }
       public TransVer2State TransVer2 { get; private set; }
       public HurtState Hurt { get; private set; }  
       public DeathState Death { get; private set; }

       private void Awake()
       {
           _movement = GetComponent<WerewolfMovement>();
           _animator = GetComponent<Animator>();

           Idle = new IdleState(this, _movement);
           Walk = new WalkState(this, _movement);
           Run = new RunState(this, _movement);
           Attack1 = new Attack1State(this, _movement);
           Attack2 = new Attack2State(this, _movement);
           Attack3 = new Attack3State(this, _movement);
           Attack4 = new Attack4State(this, _movement);
           Attack5 = new Attack5State(this, _movement);
           Down = new DownState(this, _movement);
           Jump = new JumpState(this, _movement);
           Fall = new FallState(this, _movement);
           Landing = new LandingState(this, _movement);
           RunToWalk = new RunToWalkState(this, _movement);
           WalkToRun = new WalkToRunState(this, _movement);
           TransVer2 = new TransVer2State(this, _movement);
           Hurt = new HurtState(this, _movement);
           Death = new DeathState(this, _movement);
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

       public void Update()
       {
           _currentState?.Execute();
       }

       public void FixedUpdate()
       {
           _currentState?.FixedExecute();
       }
    }
}
