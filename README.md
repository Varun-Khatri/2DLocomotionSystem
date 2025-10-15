# 2D Locomotion System

A flexible, strategy-based locomotion system for 2D games in Unity. Designed with clean architecture and extensibility in mind.

## 🚀 Features

- **Strategy Pattern Architecture** - Easily extendable with new movement types
- **Multiple Locomotion States** - Walk, run, jump, dash, climb, and more
- **Advanced Movement Mechanics**:
  - Coyote time for forgiving jumps
  - Wall detection and climbing
  - Gravity control
  - Smooth state transitions
- **Input-Agnostic** - Works with any input system
- **Modular Design** - Mix and match movement behaviors

## 🏗️ Architecture

The system uses a Strategy Pattern where each movement type (walking, dashing, climbing) is implemented as a separate strategy: 
LocomotionController → BaseStrategy → [WalkStrategy, DashStrategy, ClimbStrategy, etc.]

### Core Components

- **`LocomotionController`** - Main controller that manages active strategy and physics
- **`BaseStrategy`** - Abstract base class for all movement strategies
- **`BaseSettings`** - ScriptableObject configuration for each strategy
- **`LocomotionSettings`** - Global physics and environment settings

## 📦 Installation

1. Download the latest release or clone this repository
2. Add the files to your Unity project (2019.4+ recommended)
3. Create a `LocomotionController` component on your player character

## 🎮 Quick Start

### Basic Setup

**Add the Controller**:
```csharp
[SerializeField] private LocomotionController _locomotionController;
```
**Configure Settings**:

Create LocomotionSettings ScriptableObject

Set up ground/wall layers and physics values

**Create Movement Strategies**:

Inherit from BaseStrategy and BaseSettings

Implement Enter(), Exit(), Execute() methods

### Example Usage
```csharp
// Get current movement state
bool isDashing = _locomotionController.IsDashing;
bool isClimbing = _locomotionController.IsClimbing;

// Control gravity
_locomotionController.EnableGravity(false);

// Add forces
_locomotionController.AddForce(Vector2.up * 10f, ForceMode2D.Impulse);
```
## ⚙️ Configuration

**Locomotion Settings**

Coyote Time: Forgiving jump timing after leaving platforms

Gravity Control: Customizable gravity scale and application

Ground Detection: Configurable check radius and layers

Wall Detection: Distance and layer settings for wall interactions

**Strategy Transitions**

Define state transitions in your Strategy Settings:
```csharp
[SerializeField] private List<BaseSettings> _possibleTransitionsTo;
```

## 🔧 Extending the System

**Creating New Strategies**

Create Settings:

```csharp
[CreateAssetMenu(fileName = "FlySettings", menuName = "Locomotion/Fly")]
public class FlySettings : BaseSettings
{
    public override BaseStrategy GetStrategy(LocomotionController controller, InputHandler inputHandler)
    {
        return new FlyStrategy(controller, inputHandler, this);
    }
}
```

Implement Strategy:

```csharp
public class FlyStrategy : BaseStrategy
{
    public FlyStrategy(LocomotionController controller, InputHandler inputHandler, BaseSettings settings) 
        : base(controller, inputHandler, settings) { }

    public override void Execute()
    {
        // Implement flight logic
    }
}
```
## 🎯 Use Cases
- 2D Platformers - Precise jumping and movement

- Metroidvanias - Multiple movement abilities

- Top-Down Games - Directional movement with rotation

- Prototyping - Quick iteration on movement mechanics

## 🤝 Contributing
This system is part of my professional portfolio. Feel free to:

- Use in your personal or commercial projects
- Fork and modify for your needs
- Report issues or suggest improvements
