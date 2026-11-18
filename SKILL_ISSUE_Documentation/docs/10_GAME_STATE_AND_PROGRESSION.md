# 10 — Game State and Progression

## 1. Game states

Use an explicit state model. Minimum states:

- `MainMenu`
- `LevelSelect`
- `Playing`
- `Paused`
- `LevelComplete`
- `Victory`

A temporary player death/respawn sequence may be represented as a player state or a gameplay sub-state. Choose one approach and use it consistently.

## 2. State transition rules

- MainMenu → LevelSelect: player chooses level selection.
- MainMenu → Playing: start game loads the first unlocked level.
- LevelSelect → Playing: player chooses an unlocked level.
- Playing → Paused: Escape pressed.
- Paused → Playing: Resume selected or Escape pressed.
- Paused → Playing: Restart Level selected; reload current level.
- Playing → LevelComplete: player touches valid exit.
- LevelComplete → Playing: Next Level selected, if one exists.
- LevelComplete → LevelSelect/MainMenu: player chooses navigation.
- W5_L3 completion → Victory.
- Victory → MainMenu: player chooses return.

Do not allow state transitions to trigger repeatedly in the same frame.

## 3. Death counter

- Counter starts at zero for a new game session.
- Increment exactly once when a lethal event is accepted.
- Automatic respawn does not reset the counter.
- Manual `R` restart does not increment the counter by default.
- Changing levels does not reset the session total.
- If a per-level count is implemented, label it separately from total session deaths.
- Do not enforce a maximum number of deaths.

## 4. Respawn process

1. Lethal collision or fall boundary requests death.
2. Guard prevents duplicate death requests.
3. Increment total session death count.
4. Enter dying feedback state.
5. After a short configured delay, reset traps according to level policy.
6. Place player at active checkpoint or initial spawn.
7. Clear velocity and transient movement states.
8. Re-enable control.
9. Return to Playing.

Keep the sequence short and make the player invulnerable only as long as needed to prevent immediate repeated death from overlapping hazards.

## 5. Checkpoint state

Checkpoint state belongs to the active level session.
- Activating a checkpoint updates the active respawn position.
- Death respawns at the checkpoint.
- Manual restart returns to initial spawn and resets checkpoint activation by default.
- Loading a different level initializes that level's checkpoint state.

## 6. Progression and unlocks

- New save: W1_L1 unlocked.
- Completing a level unlocks the next level in campaign order.
- Completing a level again does not remove later unlocks.
- Completing W5_L3 marks campaign complete.
- Progression must be validated even if save data is missing or corrupted.

## 7. Save data

Recommended minimal save data:
- Schema/version number.
- Highest unlocked level index or completion flags.
- Optional settings.

Do not persist the session death counter in the first release unless explicitly approved. Do not store secrets or personal information.

When loading save data:
- If no file exists, use defaults.
- If file is malformed or from an unsupported version, recover safely and explain/log the fallback.
- Write saves atomically where practical to reduce corruption risk.

## 8. Reset invariants

After level reset:
- Player is at initial spawn.
- Velocity is cleared.
- All trap timers and states are reset.
- Moving hazards return to initial positions.
- Collectible/interaction state returns to initial level state unless explicitly persistent.
- Level completion is false.
- Death total remains unchanged.
- Campaign unlock progress remains unchanged.
