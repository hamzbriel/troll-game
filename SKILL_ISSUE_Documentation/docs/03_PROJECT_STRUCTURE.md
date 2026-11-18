# 03 — Project Structure

## 1. Target repository layout

The exact generated MonoGame template files may differ slightly. Preserve required template files and adapt this structure without breaking the project.

```text
SKILL_ISSUE/
├── .claude/
│   └── rules/
├── Assets/
│   ├── Sprites/
│   │   ├── Player/
│   │   ├── Traps/
│   │   ├── Environment/
│   │   └── UI/
│   ├── Backgrounds/
│   ├── Audio/
│   │   ├── Music/
│   │   └── SFX/
│   └── Fonts/
├── Content/
│   └── Content.mgcb
├── docs/
├── Source/
│   ├── Core/
│   ├── Input/
│   ├── Graphics/
│   ├── Entities/
│   ├── Physics/
│   ├── Traps/
│   ├── Levels/
│   ├── States/
│   ├── UI/
│   └── Audio/
├── Tests/
├── Game1.cs
├── Program.cs
├── SKILL_ISSUE.csproj
├── CLAUDE.md
└── README.md
```

Do not move or rename the generated entry point until the project has been inspected and the namespace/build implications are understood.

## 2. Folder responsibilities

- `Assets/`: source art/audio files and asset register. Runtime-ready content may be copied or built into `Content/`.
- `Content/`: MonoGame Content Pipeline project and processed content.
- `Source/Core/`: game configuration, game manager, state coordination.
- `Source/Input/`: keyboard polling and input abstraction.
- `Source/Graphics/`: camera, viewport scaling, render helpers, debug drawing.
- `Source/Entities/`: player and other world entities.
- `Source/Physics/`: collision shapes, collision world, movement resolution.
- `Source/Traps/`: reusable trap base and concrete trap types.
- `Source/Levels/`: level data, loading, validation, progression.
- `Source/States/`: menu, gameplay, pause, completion states.
- `Source/UI/`: HUD and reusable UI elements.
- `Source/Audio/`: sound/music playback wrapper.
- `Tests/`: automated tests for logic that can be tested without a graphics device.

## 3. Class responsibility contract

### Core
- `GameConfig`: constants/configuration such as logical width, height, title, and default tuning values.
- `GameManager`: active state, transitions, shared service ownership.
- `Game1`: MonoGame lifecycle and top-level orchestration only.

### Input
- `InputManager`: current and previous keyboard states; methods such as `IsDown` and `WasPressed`.

### Graphics
- `Camera2D`: world camera position and transform matrix.
- `ViewportScaler`: compute a 16:9 destination rectangle and preserve aspect ratio.
- `DebugDraw`: optional collision/trigger outlines.

### Entities and physics
- `Player`: input intent, movement state, animation state, death request.
- `PhysicsBody` (if needed): velocity, gravity, collision dimensions.
- `CollisionWorld`: solid and hazard/trigger queries.

### Traps
- `TrapBase`: common ID, bounds, activation/update/draw contract.
- Concrete trap classes: own their specific state and behavior.

### Levels
- `LevelData`: declarative level description.
- `LevelManager`: active level, spawn/checkpoint, restart, completion.
- `LevelValidator`: checks required data before gameplay.

### States and UI
- `GameState` or equivalent: menu/gameplay/pause/completion state contract.
- `MainMenuState`, `GameplayState`, `PauseState`, `LevelCompleteState`.
- `HUD`: death counter and level/world label.

## 4. Dependency direction

Prefer this direction:

`Game1 → GameManager → active state → gameplay systems`

Entities and traps should not directly depend on menu screens. UI should request actions through a manager/event or clear method rather than directly mutating unrelated internal fields.

Avoid circular references. Avoid global mutable static state except immutable configuration constants.

## 5. Naming conventions

- Types and public members: `PascalCase`.
- Local variables and private fields: follow consistent C# conventions; private fields may use `_camelCase`.
- Constants: `PascalCase` or a consistent project-wide convention.
- Level IDs: `W1_L1` format.
- Asset names: descriptive, lowercase or consistent PascalCase; avoid spaces.
- Namespaces should reflect the project, e.g. `SkillIssue.Core`, `SkillIssue.Entities`.

## 6. File creation policy for Claude Code

Before creating a new class:
1. Check whether an existing class already owns the responsibility.
2. Prefer extending a focused existing system over duplicating it.
3. Keep one primary public type per file unless a small private/helper type is tightly coupled.
4. Update this document if the architecture changes materially.
5. Build after namespace or project-file changes.
