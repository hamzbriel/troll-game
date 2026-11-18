# 05 — Player and Physics Specification

## 1. Purpose

Define consistent player movement and collision behavior. Numeric tuning values are initial targets, not sacred constants; tune them through playtesting while preserving the behavior in this document.

## 2. Player representation

- Visual concept: small white square-like character with a simple face.
- Suggested initial collision size: approximately 24 × 30 logical pixels. Adjust to the final sprite but keep collision independent from transparent sprite padding.
- Position convention: use a clearly documented convention (prefer top-left position for rectangles, or center position for physics bodies) and use it consistently.
- Maintain separate visual bounds and collision bounds if needed.

## 3. Initial physics tuning

Use these as starting values and tune after a playable test:

| Parameter | Initial target |
|---|---:|
| Horizontal max speed | 220 logical px/s |
| Ground acceleration | 1800 logical px/s² |
| Ground deceleration | 2200 logical px/s² |
| Air acceleration | 1100 logical px/s² |
| Gravity | 1500 logical px/s² |
| Jump initial velocity | -520 logical px/s |
| Maximum fall speed | 850 logical px/s |
| Coyote time | 0.08 s (optional) |
| Jump buffer | 0.10 s (optional) |

MonoGame screen coordinates increase downward, so upward jump velocity is negative.

These values must be stored in one tuning/configuration location rather than duplicated throughout the code.

## 4. Time-based integration

Use elapsed seconds from `GameTime.ElapsedGameTime`.

Conceptual integration:

- `velocity.X` approaches the desired horizontal speed using acceleration/deceleration.
- `velocity.Y += gravity * deltaTime`.
- Clamp downward velocity to maximum fall speed.
- Apply movement and resolve collision.

Do not multiply acceleration or velocity by frame rate incorrectly. Test at different frame rates if possible.

## 5. Jump rules

- A jump starts on a new W key press, not merely because W remains held.
- The player must be grounded, or within the configured coyote-time window if that optional feature is enabled.
- Jumping sets vertical velocity to the configured negative jump velocity.
- Landing resets vertical velocity and grounded state.
- Releasing and pressing W again is required for another jump.
- Do not implement double jump unless a later design revision explicitly approves it.

## 6. Collision rules

Use AABB collision for the initial implementation.

Solid geometry:
- Blocks player movement.
- Resolves horizontal and vertical penetration.
- Landing on top of a platform sets grounded state.
- Hitting the underside of a platform cancels upward movement.

Hazards:
- Do not block movement unless the trap specification says they are solid.
- Active lethal overlap requests player death.

Triggers:
- Activate trap or checkpoint logic.
- Must not automatically kill the player unless configured as lethal.

## 7. Recommended resolution approach

For each update:
1. Read input and calculate desired horizontal movement.
2. Update horizontal velocity.
3. Apply horizontal displacement and resolve horizontal solid collisions.
4. Apply gravity to vertical velocity.
5. Apply vertical displacement and resolve vertical solid collisions.
6. Determine grounded state.
7. Check hazard overlap and trigger overlap.
8. Update animation state.

A swept collision or smaller substeps may be needed for fast-moving bodies to avoid tunneling through thin hazards. Start with AABB and add a robust solution only when tests reveal the need.

## 8. Player states

Minimum logical states:
- `Idle`
- `Running`
- `Rising`
- `Falling`
- `Dying`
- `Respawning`

Animation state is derived from movement/physics state; do not let animation logic change physics unexpectedly.

## 9. Death and respawn integration

The player should request death from the game/level system rather than directly resetting the entire level. The level/game manager owns death counting and respawn coordination.

When respawning:
- Place the player at the selected spawn/checkpoint.
- Clear velocity.
- Clear grounded/jump-buffer transient state.
- Restore collision and control after the respawn delay.
- Ensure the spawn is not inside a solid tile or active hazard.

## 10. Verification checklist

- [ ] Player moves left and right.
- [ ] Player stops predictably when input is released.
- [ ] Player jumps only when allowed.
- [ ] Holding W does not auto-jump.
- [ ] Player lands on platforms.
- [ ] Player collides with platform sides and undersides.
- [ ] Player falls when unsupported.
- [ ] Player cannot move through a solid floor.
- [ ] Behavior is not tied to frame rate.
- [ ] Death/respawn clears stale velocity and input state.
