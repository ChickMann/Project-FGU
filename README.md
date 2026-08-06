# ⚔️ Project-FGU – 2D Action Combat Soulslike Platformer

<div align="center">

[![Engine](https://img.shields.io/badge/Engine-Unity--6--2D-black?style=for-the-badge&logo=unity&logoColor=white)](https://unity.com)
[![Language](https://img.shields.io/badge/Language-C%23-blue?style=for-the-badge&logo=csharp&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Render Pipeline](https://img.shields.io/badge/Render--Pipeline-URP--2D-purple?style=for-the-badge)](#)
[![Architecture](https://img.shields.io/badge/Architecture-FSM--%7C--SOLID-green?style=for-the-badge)](#)
[![Genre](https://img.shields.io/badge/Genre-2D--Soulslike--Platformer-red?style=for-the-badge)](#)

<br/>

> **A fluid, high-octane 2D Soulslike Action Platformer focusing on tight combat mechanics, precise timing, and polished Game Feel.**

</div>

---

## 📖 Table of Contents

- [Overview](#-overview)
- [Key Features & Gameplay Mechanics](#-key-features--gameplay-mechanics)
- [Software Architecture & Design Patterns](#-software-architecture--design-patterns)
- [Game Feel & Movement Assists](#-game-feel--movement-assists)
- [Controls](#-controls)
- [Project Structure](#-project-structure)
- [Tech Stack](#-tech-stack)

---

## 🎮 Overview

**Project-FGU** is a 2D Soulslike Action Combat Platformer developed in Unity. The game features fast-paced, high-risk combat requiring players to master attack combos, parries, dodge-rolling, and wall-sliding while navigating hazardous 2D environments.

---

## ✨ Key Features & Gameplay Mechanics

| Feature | Description |
|---|---|
| ⚔️ **Fluid Combo System** | Light attack combos, heavy attacks, and punch strikes seamlessly chained together. |
| 🛡️ **Parry & Counter** | Precise timing parry window to block incoming damage and stun opponents. |
| 🌀 **Dodge Roll (I-Frames)** | Invulnerability-frame dodge roll to evade enemy attacks and reposition. |
| 🧗 **Wall Movement** | Wall slide and wall jump mechanics with directional push-off forces. |
| 🎒 **Item & Inventory System** | Interactive item pickups (`ItemPickup.cs`), health management, and player inventory (`PlayerInventory.cs`). |

---

## 🏗️ Software Architecture & Design Patterns

The codebase is built adhering to **SOLID Principles** and decoupled software engineering patterns:

### 🏆 1. State Pattern (Finite State Machine - FSM)
Player behavior is managed by an interface-driven **Finite State Machine (`IState`, `PlayerStateMachine`)**, completely decoupling animation, physics, and state transitions without giant `switch/if-else` blocks.

- **Interface:** `IState.cs` (`Enter()`, `Execute()`, `FixedExecute()`, `Exit()`)
- **Managed States (15+ states):** `IdleState`, `WalkState`, `RunState`, `RunStopState`, `JumpState`, `FallState`, `CrouchState`, `StandUpState`, `AttackState`, `HeavyAttackState`, `PunchState`, `ParryState`, `DodgeState`, `WallSlideState`, `HurtState`, `DeathState`, `FocusState`.

### 🛡️ 2. SOLID Principles Implementation
- **Single Responsibility Principle (SRP):** Each state class manages only its own behavior; `PlayerHealth` handles damage; `PlayerInventory` manages items.
- **Open/Closed Principle (OCP):** Adding new player abilities requires creating a new `IState` implementation without modifying existing code.
- **Liskov Substitution & Interface Segregation:** All states inherit from a clean, lightweight `IState` interface.
- **Dependency Inversion Principle (DIP):** `PlayerStateMachine` depends on the `IState` abstraction rather than concrete implementations.

---

## 🎯 Game Feel & Movement Assists

Project-FGU incorporates fine-tuned platformer physics and input assists to deliver an exceptionally responsive player experience:

* **Coyote Time (`coyoteTime`):** A grace period (`0.01s – 0.5s`) allowing players to jump even after walking off a platform edge.
* **Input Buffering (`jumpInputBufferTime`, `AttackInputBufferTime`, `ParryInputBufferTime`, `DodgeInputBufferTime`):** Automatically executes queued commands pressed shortly before current animations finish.
* **Jump Apex Floatiness (`jumpHangMaxSpeedMult`):** Reduced gravity at peak jump height for better mid-air control.
* **Stopping Friction (`RunStopState`):** Deceleration curves that give weight and momentum to character movement.
* **Dodge Roll I-Frames:** Invincibility frames during dodge roll execution.

---

## 🕹️ Controls

Powered by **Unity's New Input System (`PlayerInput.inputactions`)**:

| Action | Control (Keyboard / Gamepad) |
|---|---|
| **Move Left / Right** | `A / D` or `Left Stick` |
| **Jump / Wall Jump** | `Space` or `Button South (A)` |
| **Light Attack / Combo** | `Left Click` or `Button West (X)` |
| **Heavy Attack** | `Right Click` or `Button North (Y)` |
| **Parry / Block** | `E` or `Left Bumper (LB)` |
| **Dodge Roll** | `Left Shift` or `Button East (B)` |
| **Crouch** | `S` or `Down Arrow` |

---

## 📁 Project Structure

```
Assets/
├── Scripts/
│   ├── IState.cs                  # FSM Interface (Enter, Execute, FixedExecute, Exit)
│   ├── PlayerStateMachine.cs      # Central FSM Controller
│   ├── PlayerController.cs        # Main Player logic and Input handling
│   ├── PlayerMovement.cs          # Physics movement execution
│   ├── PlayerData.cs              # ScriptableObject configuration for Game Feel parameters
│   ├── PlayerHealth.cs            # Health & damage processing
│   ├── PlayerInventory.cs         # Inventory & item management
│   ├── ItemPickup.cs              # Interactive world items
│   ├── AttackState.cs             # Combo attack state logic
│   ├── ParryState.cs              # Parry timing logic
│   ├── DodgeState.cs              # Dodge roll state logic
│   ├── WallSlideState.cs          # Wall slide physics logic
│   └── ...                        # Additional state implementations
└── ...
```

---

## 🔧 Tech Stack

- **Engine:** Unity (2D, Universal Render Pipeline - URP)
- **Language:** C#
- **Input:** Unity New Input System (`UnityEngine.InputSystem`)
- **Physics:** 2D Rigidbody & Custom FixedUpdate Integration
- **Architecture:** State Pattern (FSM), SOLID Principles, Event Architecture

---

<div align="center">

**Made with ❤️ by ChickMan**

</div>
