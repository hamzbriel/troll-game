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
- [x] A/Left moves left.
- [x] D/Right moves right.
- [x] W jumps only when allowed.
- [x] Holding W does not auto-jump.
- [x] Movement is frame-rate independent.

**Completed:** 18 Nov 2026  
**Files created:**
- `Source/Input/InputManager.cs` - Keyboard state tracking with edge detection
- `Source/Entities/Player.cs` - Physics-based player movement
- Modified `Game1.cs` - Input polling, player update, temporary ground collision

**Physics implemented:**
- Time-based integration using GameTime.ElapsedGameTime
- Horizontal acceleration/deceleration (different for ground/air)
- Gravity with terminal velocity
- Edge-triggered jump (no auto-jump on hold)
- Coyote time (0.08s grace period after leaving ground)
- Jump buffer (0.10s early input tolerance)
- Velocity clamping to max speed/fall speed

**Temporary collision:**
- Ground plane at Y=460
- Side walls for boundary testing
- Proper collision system in Milestone 3

## Milestone 3 — Platforms and collision

Tasks:
- Implement solid AABB collision.
- Add horizontal and vertical resolution.
- Add test floor, wall, and platform.
- Add fall boundary.

Acceptance:
- [x] Player stands on floor.
- [x] Player cannot pass through walls/floor.
- [x] Player can land on platforms.
- [x] Player falls when unsupported.
- [x] No obvious tunneling at normal movement speeds.

**Completed:** 18 Nov 2026  
**Files created:**
- `Source/Physics/CollisionWorld.cs` - AABB collision detection and resolution
- Modified `Game1.cs` - Platform definitions, collision integration, R restart

**Collision features:**
- Solid platform AABB detection
- Separate horizontal/vertical collision resolution
- Landing detection (grounded state)
- Ceiling hit detection
- Wall collision with horizontal stop
- Hazard zones (death on overlap)
- IsGrounded check (1px below feet)

**Test level geometry:**
- Floor + side walls
- 4 elevated platforms at different heights
- Fall boundary below screen (death zone)
- Exit marker placeholder

**Controls updated:**
- W/Space/Up Arrow → Jump
- R → Restart level

## Milestone 4 — Death, respawn, and counter

Tasks:
- Implement death request guard.
- Add death feedback.
- Add session death counter.
- Add respawn at spawn.
- Add R restart behavior.

Acceptance:
- [x] Hazard/fall causes one death increment.
- [x] Respawn is automatic and unlimited.
- [x] Death count is retained after respawn.
- [x] R restarts level without counting as death by default.
- [x] No repeated death increments during one death sequence.

**Completed:** 18 Nov 2026  
**Files modified:**
- `Source/Core/GameConfig.cs` - Death/respawn timing constants
- `Source/Entities/Player.cs` - Death state machine, RequestDeath(), IsDead flag
- `Game1.cs` - Death counter tracking, death freeze, respawn fade-in, visual counter

**Death system features:**
- Request/consume pattern prevents double-counting
- 0.3s death freeze before respawn
- 0.2s fade-in alpha after respawn
- Manual restart (R key) does NOT increment counter
- Death counter displayed as crimson bars (placeholder for font)
- Player hidden during death state
- Unlimited retries maintained

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
- [x] Each trap is reproducible.
- [x] No trap causes an unintended soft lock.
- [x] Moving platform carry is stable.
- [x] Fake exit does not complete the level.

**Completed:** 18 Nov 2026  
**Files created:**
- `Source/Traps/MovingPlatform.cs` - Platform with player carry behavior
- `Source/Traps/FakeExit.cs` - Deceptive exit with teleport
- `Source/Traps/BaitObject.cs` - Collectible trap trigger
- `Source/Traps/TeleportTrap.cs` - Position teleporter
- `Source/Traps/RotatingSaw.cs` - Rotating hazard with patrol

**Features implemented:**
- Moving platform with velocity-based player carry (no jitter)
- Fake exit visually identical to real exit, teleports player back
- Bait object with pulsing animation
- Teleport trap with cooldown system
- Rotating saw with animated blades and patrol movement
- Special trap handling in Game1.cs (teleport, fake exit, bait collection)

## Milestone 7 — Level data and first world

Tasks:
- Implement level definition/loading/validation.
- Build W1_L1, W1_L2, W1_L3.
- Add restart and checkpoint handling.

Acceptance:
- [x] Three levels load from data.
- [x] Spawn and exit are valid.
- [x] All three levels are completable.
- [x] Unlock progression works.

**Completed:** 18 Nov 2026  
**Files created:**
- `Source/Level/LevelData.cs` - Level structure with platforms, hazards, traps
- `Source/Level/LevelLoader.cs` - JSON parsing and trap instantiation
- `Source/Level/WorldProgressionManager.cs` - Level unlock and progression tracking
- `Content/Levels/W1_L1.json` - Tutorial level (simple spikes, bait)
- `Content/Levels/W1_L2.json` - Trap introduction (4 basic traps)
- `Content/Levels/W1_L3.json` - Advanced traps (moving platform, saw, teleport, fake exit)

**Features implemented:**
- JSON-based level format
- Automatic trap instantiation from data
- Exit collision triggers level completion
- Progression system unlocks next level
- Death counter resets on level completion
- W1_L1 always unlocked at start

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
- [x] All screens are keyboard navigable.
- [x] UI matches visual guidelines.
- [x] Pause freezes gameplay.
- [x] Progression screens behave correctly.

**Completed:** 18 Nov 2026  
**Files created:**
- `Source/UI/GameState.cs` - Game state enum
- `Source/UI/UIHelper.cs` - Shared text/button rendering with 3x5 pixel font
- `Source/UI/MainMenu.cs` - Title screen with Play/Exit
- `Source/UI/LevelSelectMenu.cs` - World/level selection with unlock display
- `Source/UI/PauseMenu.cs` - Pause overlay with Resume/Restart/Main Menu
- `Source/UI/LevelCompleteScreen.cs` - Victory screen with death count feedback

**Features implemented:**
- State machine architecture (MainMenu, LevelSelect, Playing, Paused, LevelComplete, GameComplete)
- Main menu with title and navigation
- Level select with 5 worlds × 3 levels grid, locked/unlocked display
- Pause menu with dark overlay
- Level complete screen with 2.5s display before auto-advance
- Death count feedback messages on completion
- Game complete screen after all levels
- W/S/Arrows for navigation, Space/Enter to select, Escape to back/pause
- All UI using custom pixel font (consistent with game style)

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
