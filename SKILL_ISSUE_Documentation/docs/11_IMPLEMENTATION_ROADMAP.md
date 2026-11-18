# 11 — Implementation Roadmap

## Working method

Implement one milestone at a time. Do not ask Claude Code to build the entire game in one large change. At the end of each milestone, build, test, summarize, and update documentation.

A milestone is complete only when its acceptance criteria pass.

## Milestone 0 — Repository inspection and setup

Tasks:
- Inspect generated MonoGame DesktopGL project.
- Record SDK and MonoGame package versions.
- Confirm project builds and launches.
- Add this documentation structure.
- Add `.gitignore` appropriate for Visual Studio/.NET if using Git.

Acceptance:
- [x] Project builds.
- [x] Empty MonoGame window launches.
- [x] Documentation is present.
- [x] No unnecessary package upgrades.

**Completed:** 18 Nov 2026  
**Environment:** .NET 10.0.403, MonoGame 3.8.6.1, net9.0, Windows 10

## Milestone 1 — Window and rendering foundation

Tasks:
- Set logical resolution to 960 × 540.
- Set title to SKILL ISSUE.
- Implement aspect-ratio-preserving viewport scaling.
- Set pixel-art sampler to PointClamp.
- Draw a temporary background, floor, and placeholder player.

Acceptance:
- [x] Window shows correct title.
- [x] Logical scene is 960 × 540.
- [x] Resizing does not distort aspect ratio.
- [x] Placeholder scene renders cleanly.

**Completed:** 18 Nov 2026  
**Files created:**
- `Source/Core/GameConfig.cs` - Game constants and tuning values
- `Source/Graphics/ViewportScaler.cs` - Aspect ratio preservation
- Modified `Game1.cs` - Render target, viewport scaling, placeholder scene

**Graphics concepts demonstrated:**
- Logical vs physical resolution separation
- Render target rendering
- Viewport scaling with letterboxing/pillarboxing
- Nearest-neighbor sampling (SamplerState.PointClamp) for pixel art
- Primitive rectangle rendering

## Milestone 2 — Input and player movement

Tasks:
- Implement InputManager.
- Implement left/right movement.
- Implement jump on W press.
- Add gravity and maximum fall speed.
- Add grounded state and basic player states.

Acceptance:
- [ ] A/Left moves left.
- [ ] D/Right moves right.
- [ ] W jumps only when allowed.
- [ ] Holding W does not auto-jump.
- [ ] Movement is frame-rate independent.

## Milestone 3 — Platforms and collision

Tasks:
- Implement solid AABB collision.
- Add horizontal and vertical resolution.
- Add test floor, wall, and platform.
- Add fall boundary.

Acceptance:
- [ ] Player stands on floor.
- [ ] Player cannot pass through walls/floor.
- [ ] Player can land on platforms.
- [ ] Player falls when unsupported.
- [ ] No obvious tunneling at normal movement speeds.

## Milestone 4 — Death, respawn, and counter

Tasks:
- Implement death request guard.
- Add death feedback.
- Add session death counter.
- Add respawn at spawn.
- Add R restart behavior.

Acceptance:
- [ ] Hazard/fall causes one death increment.
- [ ] Respawn is automatic and unlimited.
- [ ] Death count is retained after respawn.
- [ ] R restarts level without counting as death by default.
- [ ] No repeated death increments during one death sequence.

## Milestone 5 — Trap framework

Tasks:
- Implement trap base/state contract.
- Implement hidden spikes.
- Implement fake platform.
- Implement disappearing floor.
- Implement falling block.
- Add deterministic reset behavior.

Acceptance:
- [ ] Each trap has trigger/state/reset.
- [ ] Visual and collision states agree.
- [ ] Death is counted once.
- [ ] Traps reset after respawn/restart according to spec.

## Milestone 6 — Remaining traps

Tasks:
- Implement moving platform and carry behavior.
- Implement fake exit.
- Implement bait object.
- Implement teleport trap.
- Implement rotating saw.
- Add warning feedback.

Acceptance:
- [ ] Each trap is reproducible.
- [ ] No trap causes an unintended soft lock.
- [ ] Moving platform carry is stable.
- [ ] Fake exit does not complete the level.

## Milestone 7 — Level data and first world

Tasks:
- Implement level definition/loading/validation.
- Build W1_L1, W1_L2, W1_L3.
- Add restart and checkpoint handling.

Acceptance:
- [ ] Three levels load from data.
- [ ] Spawn and exit are valid.
- [ ] All three levels are completable.
- [ ] Unlock progression works.

## Milestone 8 — Full campaign

Tasks:
- Build Worlds 2–5.
- Add all 15 level definitions.
- Balance difficulty and checkpoints.
- Test each level.

Acceptance:
- [ ] Exactly 15 levels are selectable as unlocked.
- [ ] Campaign order and unlocks are correct.
- [ ] Every level has been completed in a test run.
- [ ] No known soft locks remain.

## Milestone 9 — UI and presentation

Tasks:
- Main menu.
- Level selection.
- Controls screen.
- HUD.
- Pause menu.
- Level-complete and victory screens.
- Visual transitions and feedback.

Acceptance:
- [ ] All screens are keyboard navigable.
- [ ] UI matches visual guidelines.
- [ ] Pause freezes gameplay.
- [ ] Progression screens behave correctly.

## Milestone 10 — Art and audio integration

Tasks:
- Replace placeholders with original pixel art.
- Add player animations.
- Add environment and trap art.
- Add licensed/original sound effects and music.
- Maintain asset register.

Acceptance:
- [ ] No missing required assets.
- [ ] Pixel art is crisp.
- [ ] Animations match gameplay states.
- [ ] Audio feedback is clear and balanced.
- [ ] Asset licenses/sources are recorded.

## Milestone 11 — QA, optimization, and release

Tasks:
- Run regression checklist.
- Fix collision and progression defects.
- Test window scaling and keyboard input.
- Build Release configuration.
- Test on a clean target Windows environment.
- Prepare project report/demo notes.

Acceptance:
- [ ] Release build starts.
- [ ] All 15 levels are completable.
- [ ] Core controls work.
- [ ] Death counter and retry work.
- [ ] No blocking defects remain.
- [ ] Submission package is complete.

## Recommended Claude Code task prompt

For each milestone, use a request like:

> Read CLAUDE.md and the relevant specification documents. Inspect the current implementation. Implement only Milestone N from `docs/11_IMPLEMENTATION_ROADMAP.md`. Do not change fixed design decisions. Build the project and run relevant tests. If a requirement conflicts with the existing code or is ambiguous, explain before making a major change. At the end, report changed files, build/test results, known issues, and documentation updates.
