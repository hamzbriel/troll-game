# 02 — Technical Design

## 1. Technology baseline

- Language: C#.
- Framework: MonoGame DesktopGL.
- Runtime: .NET SDK/runtime compatible with the selected MonoGame package version.
- IDE: Visual Studio.
- Target deliverable: Windows desktop.
- Input: MonoGame keyboard input.
- Rendering: MonoGame `SpriteBatch` and textures.
- Content: MonoGame Content Pipeline where appropriate; runtime-generated primitive textures may be used for early prototypes.

Use the current compatible MonoGame DesktopGL template when creating the project. Do not pin arbitrary package versions. Record the actual SDK and MonoGame package versions in the repository once created.

## 2. Application architecture

Keep the architecture understandable and appropriately small for a student project.

Main responsibilities:

- `Game1`: MonoGame lifecycle, initialize systems, update active state, draw active state.
- `GameManager`: high-level state transitions and shared game services.
- `InputManager`: keyboard state and edge-press helpers.
- `Player`: player state, movement intent, physics integration, animation state.
- `CollisionWorld`: queries and resolution for solid geometry.
- `Trap` implementations: activation state and hazard behavior.
- `LevelManager`: load/reset current level, checkpoint and exit handling.
- `LevelData`: declarative level layout and object definitions.
- `Camera2D`: world-to-screen transform for scrolling levels.
- UI screens: menu, pause, HUD, level complete, game over if used.

Avoid creating abstractions that do not solve a current need. Do not put all systems into `Game1`.

## 3. Update and draw loop

Use MonoGame's lifecycle:

- `Initialize`: initialize non-content systems.
- `LoadContent`: load textures, fonts, sounds, and create required resources.
- `Update(GameTime)`: process input, state transitions, simulation, collision, traps, player, and progression.
- `Draw(GameTime)`: clear frame, begin SpriteBatch, draw world, entities, effects, and UI, end SpriteBatch.
- `UnloadContent`: dispose manually created disposable resources when necessary.

Simulation must use elapsed time from `GameTime`. Do not use `Thread.Sleep` or frame-count-based movement.

## 4. Logical resolution and viewport

The game's logical coordinate system is 960 × 540.

The window may be resized or displayed at another physical size, but the rendered game must preserve the 16:9 aspect ratio. Use a letterboxed/pillarboxed viewport rather than stretching non-uniformly.

For pixel-art crispness:
- Prefer integer scale factors when practical.
- Use `SamplerState.PointClamp`.
- Avoid unnecessary fractional sprite positions.
- If the window size does not permit integer scaling, preserve aspect ratio and accept a non-integer scale rather than distort the image.

All level layout coordinates are expressed in logical world pixels unless explicitly documented otherwise.

## 5. Coordinate systems

- **World coordinates:** positions inside the level.
- **Screen coordinates:** positions on the 960 × 540 logical canvas.
- **Viewport coordinates:** physical window region used to display the logical canvas.

Use a camera transform for world rendering. UI is drawn in screen/logical coordinates and should not move with the camera.

## 6. Rendering pipeline

Recommended draw order:

1. Background layers.
2. Static level geometry.
3. Moving platforms and traps.
4. Collectibles and interactable objects.
5. Player.
6. Foreground decoration.
7. Particles and temporary effects.
8. HUD and screen UI.

Use separate SpriteBatch calls only when required by different blend/sampler/transform settings. Keep draw order explicit.

## 7. Collision approach

Start with axis-aligned bounding boxes (AABB) for the player, platforms, and simple hazards.

Separate:
- Solid collision: blocks movement.
- Hazard overlap: causes death.
- Trigger overlap: activates an event without necessarily blocking movement.
- Exit overlap: requests level completion if requirements are met.

For platforming, resolve horizontal and vertical movement separately where practical. Track whether the player is grounded. Do not use sprite transparency or visual pixels as the collision shape.

## 8. Data-driven levels

Level layouts should be data, not hardcoded drawing instructions. A simple C# level definition or JSON format may be used. Choose one approach and document it; do not add a complex editor.

Each level definition should include:
- Stable ID.
- World ID and display name.
- Spawn position.
- Solid platforms.
- Trap instances and configuration.
- Checkpoints.
- Exit position and condition.
- Optional background/theme identifiers.

Validate level data at load time where practical: missing spawn, missing exit, duplicate IDs, invalid dimensions, or unknown trap types should produce clear errors.

## 9. Persistence

For the initial release, save only what is necessary:
- Highest unlocked level or completion flags.
- Optional sound settings.

The death counter is a session statistic unless a later approved design decision makes it persistent. Do not make persistence a blocker for the first playable prototype.

Use a simple local save file in the platform-appropriate application data directory. Handle missing or malformed save data safely by restoring defaults.

## 10. Performance and resource management

The game is small; prioritize clarity and correctness.
- Load textures once and reuse them.
- Avoid allocating large objects every frame.
- Dispose graphics/audio resources appropriately.
- Do not create a new `Texture2D` per entity per frame.
- Use simple collision queries suitable for compact levels.
- Keep debug rendering optional and disabled in release mode.

## 11. Error handling and diagnostics

- Log actionable content/level loading errors.
- Fail clearly when a required asset is missing.
- Provide a placeholder fallback only when it does not hide a serious content error.
- Keep debug overlays behind a development flag.
- Never swallow exceptions silently.

## 12. Graphics concepts to explain in the course report

Document how the project uses:
- 2D raster rendering and sprite batching.
- Translation for player movement and moving hazards.
- Scaling for viewport presentation.
- Rotation for rotating hazards.
- Coordinate transformations for the camera.
- AABB collision detection.
- Time-based animation.
- Layered backgrounds/parallax if implemented.
- Nearest-neighbor sampling for pixel-art rendering.
