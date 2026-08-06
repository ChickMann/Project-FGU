# Project-FGU: 2D Action Combat Soulslike Platformer

A 2D action platformer built in Unity focused on tight combat timing, parry mechanics, and responsive movement controls.

## Overview

Project-FGU is a 2D Soulslike action platformer. Gameplay relies on precise melee attacks, parrying, dodge rolling, and wall mobility through hazardous environments.

## Features & Mechanics

- **Melee Combo System:** Chain light attacks, heavy attacks, and punches.
- **Parry & Counter:** Timed blocking window that negates damage and stuns enemies.
- **Dodge Roll:** Invulnerability frames (I-frames) for dodging past incoming attacks.
- **Wall Mobility:** Wall sliding and directional wall jumps.
- **Inventory & Items:** Item pickups (`ItemPickup.cs`), health tracking (`PlayerHealth.cs`), and inventory management (`PlayerInventory.cs`).

## Software Architecture

The codebase follows SOLID design principles and uses an interface-driven State Pattern to handle character logic.

### Finite State Machine (FSM)

Player behavior is split into separate state classes implementing `IState` (`Enter`, `Execute`, `FixedExecute`, `Exit`), keeping movement and animation logic decoupled:

- **State Interface:** `IState.cs`
- **Core States:** `IdleState`, `WalkState`, `RunState`, `RunStopState`, `JumpState`, `FallState`, `CrouchState`, `StandUpState`, `AttackState`, `HeavyAttackState`, `PunchState`, `ParryState`, `DodgeState`, `WallSlideState`, `HurtState`, `DeathState`, `FocusState`.

### SOLID Implementation

- **Single Responsibility:** Individual classes manage distinct tasks (`PlayerHealth` handles damage, `PlayerInventory` manages items, each state handles one action).
- **Open/Closed:** New abilities are added by implementing `IState` without modifying the core state machine.
- **Dependency Inversion:** `PlayerStateMachine` interacts with the `IState` interface rather than concrete state implementations.

## Controls & Movement Feel

Movement tuning uses custom parameters in `PlayerData.cs` to keep controls responsive:

- **Coyote Time (`coyoteTime`):** Jump grace period after stepping off platform edges.
- **Input Buffering (`jumpInputBufferTime`, `AttackInputBufferTime`, `ParryInputBufferTime`, `DodgeInputBufferTime`):** Queues inputs pressed shortly before the current action completes.
- **Jump Apex Floatiness (`jumpHangMaxSpeedMult`):** Reduces gravity near the peak of a jump for finer air control.
- **Stopping Friction (`RunStopState`):** Deceleration curves that prevent abrupt movement stops.

### Input Mapping (Unity New Input System)

| Action | Controls |
|---|---|
| Move Left / Right | `A` / `D` or `Left Stick` |
| Jump / Wall Jump | `Space` or `A` Button |
| Light Attack | `Left Click` or `X` Button |
| Heavy Attack | `Right Click` or `Y` Button |
| Parry / Block | `E` or `LB` Button |
| Dodge Roll | `Left Shift` or `B` Button |
| Crouch | `S` or `Down Arrow` |

## Project Structure

```
Assets/
└── Scripts/
    ├── IState.cs                  # FSM interface contract
    ├── PlayerStateMachine.cs      # State machine controller
    ├── PlayerController.cs        # Input handling and player references
    ├── PlayerMovement.cs          # Physics execution
    ├── PlayerData.cs              # ScriptableObject for movement values
    ├── PlayerHealth.cs            # Health management
    ├── PlayerInventory.cs         # Inventory logic
    └── ...                        # Concrete state implementations
```

## Tech Stack

- Unity (2D, Universal Render Pipeline)
- C#
- Unity New Input System
- 2D Rigidbody Physics
