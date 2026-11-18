# 13 — Build and Delivery

## 1. Delivery goal

Produce a Windows desktop build of SKILL ISSUE that can be launched and demonstrated without requiring the development environment to be open.

The exact publishing command depends on the project's actual target framework and MonoGame package versions. Inspect the `.csproj` before selecting publish parameters.

## 2. Before release

- Build Debug and resolve all errors.
- Run the full QA checklist.
- Build Release.
- Confirm all required content is included.
- Confirm the game starts from the output folder.
- Test keyboard controls.
- Test all 15 levels.
- Verify save behavior.
- Remove temporary debug overlays and test-only shortcuts.
- Keep source project and documentation in the submission.

## 3. Build commands

Typical commands:

```bash
dotnet restore
dotnet build
dotnet build -c Release
```

For a Windows publish, inspect the target framework and choose the matching runtime identifier. A common Windows x64 RID is `win-x64`, but confirm it against the installed SDK and project configuration.

Example pattern (adapt only after checking the project):

```bash
dotnet publish -c Release -r win-x64 --self-contained true
```

Do not assume the command succeeded; inspect the output and launch the published executable.

## 4. Submission contents

Recommended package:

```text
SKILL_ISSUE_SUBMISSION/
├── SKILL_ISSUE/                 # source project
├── Build/                       # tested release output
├── Documentation/
│   ├── README.md
│   ├── GAME_DESIGN_DOCUMENT.md
│   └── TESTING_SUMMARY.md
├── Report/
│   └── Computer_Graphics_Report.pdf
└── Demo/
    └── screenshots/
```

Do not include unnecessary intermediate build folders or personal machine-specific files.

## 5. Computer Graphics report topics

The final course report should explain:
1. Project background and objectives.
2. Tools and framework.
3. Logical resolution and viewport scaling.
4. Sprite rendering and SpriteBatch.
5. Coordinate systems and camera transform.
6. Translation and rotation in gameplay.
7. Gravity and time-based movement.
8. AABB collision detection.
9. Trigger-based trap activation.
10. Animation and pixel-art sampling.
11. Testing results and limitations.
12. Conclusion and future work.

Only describe techniques that are actually implemented. Include screenshots and small code excerpts from the final project.

## 6. Demo flow

Suggested live demo:
1. Show title screen and controls.
2. Start W1_L1 and demonstrate movement/jump.
3. Trigger a visible hazard and show death counter.
4. Demonstrate automatic respawn.
5. Show one deceptive trap.
6. Complete a level and show progression.
7. Demonstrate a later level with a more advanced trap.
8. Show final victory screen if the campaign is complete.

## 7. Final handover checklist

- [ ] Source code included.
- [ ] Release build included and tested.
- [ ] README has accurate setup/run instructions.
- [ ] Documentation matches actual implementation.
- [ ] All 15 levels tested.
- [ ] Asset register complete.
- [ ] No copyrighted/ripped assets without permission.
- [ ] No secrets or personal files included.
- [ ] Computer Graphics report prepared.
- [ ] Demo screenshots/video prepared if required.
