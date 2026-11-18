# 04 — Gameplay Specification

## 1. Overview

The player controls a small character in a side-view 2D world. The objective is to reach the exit of each level while avoiding hazards and learning the behavior of deceptive objects.

## 2. Controls

| Input | Action | Notes |
|---|---|---|
| A or Left Arrow | Move left | Continuous while held |
| D or Right Arrow | Move right | Continuous while held |
| W | Jump | Triggered on press; requires grounded state |
| R | Restart current level | Restart from level spawn; confirm exact behavior with Level Manager |
| Escape | Pause/resume | Does not advance simulation while paused |

Do not add alternative controls unless approved. Keyboard controls must be shown on the controls/help screen.

## 3. Movement behavior

- Horizontal movement is responsive and time-based.
- The player cannot pass through solid platforms or walls.
- The player is affected by gravity when airborne.
- Jump begins only when the player is grounded, except if a future explicit mechanic permits otherwise.
- Holding W must not cause an automatic sequence of repeated jumps.
- The player can change horizontal direction in the air, with tuning documented in the physics specification.

## 4. Level objective

Each level has one primary exit. Touching the exit completes the level when the player is alive and the level is in the active gameplay state.

A level may optionally require a key or switch, but such requirements must be explicitly stated in that level's data. Do not add mandatory collectibles by default.

## 5. Death behavior

A player death occurs when:
- The player overlaps an active lethal hazard.
- The player falls below the level's configured death boundary.
- A specifically documented level hazard requests death.

On death:
1. Ensure death is processed once.
2. Increment the session death counter by exactly one.
3. Play death feedback.
4. Temporarily disable player control during the death/respawn sequence.
5. Respawn at the latest activated checkpoint, or the level spawn if none is active.
6. Restore a valid movement state and clear inappropriate velocity.
7. Resume play.

There is no lives limit and no game over caused by exhausting lives.

## 6. Retry and restart

- Automatic death respawn is unlimited.
- `R` restarts the current level from its initial spawn.
- Decide and implement consistently whether manual restart increments the death counter. Default: **manual restart does not count as a death**, because it is a deliberate reset rather than a lethal event.
- Restart must reset trap states, moving-object positions, and level-local interactions.
- Restart must not revoke previously unlocked campaign progress.

## 7. Checkpoints

A checkpoint is an optional level object. When activated:
- It becomes the active respawn point for the current level.
- It provides visible/audio activation feedback.
- It remains active after death in that level.
- It resets to the level's initial checkpoint state when the level is manually restarted or reloaded, unless the level definition explicitly says otherwise.

Do not put checkpoints in every level automatically. Use them where a level's challenge would otherwise create excessive repetition.

## 8. Pause

When paused:
- Player movement, physics, traps, particles, and gameplay timers stop.
- Pause UI remains visible.
- Resume returns to the same gameplay state.
- Restart Level returns to the current level's initial state.
- Main Menu returns to the title screen after safe state cleanup.

## 9. Level completion

On valid exit contact:
- Stop player control and gameplay simulation.
- Show level-complete feedback.
- Mark the level complete and unlock the next level.
- For the final level, show the final completion screen.
- Prevent repeated completion events while the completion screen is active.

## 10. Difficulty and fairness

- Every trap must be possible to learn and avoid.
- A level must not require an input or movement that the game does not support.
- Trap timing must be deterministic.
- Hazards must not spawn directly on the player without a fair warning window, except where a clearly signaled deliberate surprise is the level's central mechanic.
- Respawn positions must not overlap lethal hazards.
- Provide enough space to understand the next jump.

## 11. HUD

During gameplay show:
- Current world/level label.
- Total session death counter.
- Optional checkpoint indicator.

Keep the HUD small and unobtrusive. The death counter should be legible and update immediately after death.
