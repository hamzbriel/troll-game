# SKILL ISSUE

**"It's Not a Bug. It's Your Skill Issue."**

SKILL ISSUE is an original 2D pixel-art troll platformer built with C#, .NET, and MonoGame DesktopGL.

## Technology Stack

- **Framework:** MonoGame 3.8.6.1 (DesktopGL)
- **.NET SDK:** 10.0.403
- **Target Framework:** net9.0
- **Platform:** Windows 10/11 desktop
- **IDE:** Visual Studio (with .NET desktop development workload)

## Project Structure

```
troll-game/                          # Repository root
├── SKILL_ISSUE/                     # MonoGame project
│   ├── Content/                     # MonoGame Content Pipeline
│   ├── Game1.cs                     # Main game class
│   ├── Program.cs                   # Entry point
│   └── SKILL_ISSUE.csproj           # Project file
├── SKILL_ISSUE_Documentation/       # Complete game specification
│   ├── .claude/rules/               # Claude Code implementation rules
│   ├── docs/                        # 13 design documents
│   ├── CLAUDE.md                    # Claude Code instructions
│   └── README.md                    # Documentation overview
├── CLAUDE.md                        # (Copy for root access)
└── README.md                        # This file
```

## Documentation

See `SKILL_ISSUE_Documentation/` for complete specifications:
- Game design and technical architecture
- Player physics and trap systems
- 15-level campaign plan
- Implementation roadmap
- Visual and audio guidelines

## Build & Run

```bash
cd SKILL_ISSUE
dotnet build
dotnet run
```

## Current Status

**Milestone 0 (Setup & Verification):** ✅ Complete
- MonoGame project builds successfully
- Documentation structure established
- SDK and framework versions recorded

**Next:** Milestone 1 - Window and rendering foundation (960×540 resolution, viewport scaling, pixel-art setup)