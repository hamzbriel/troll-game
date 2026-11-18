# 12 — Testing and Quality Assurance

## 1. Testing goals

Verify that the game is playable, deterministic, completable, and stable. Testing is required after each milestone, not only before final submission.

## 2. Test layers

### Unit-level logic tests
Where practical, test pure logic without creating a graphics device:
- Input edge detection.
- Level ID/order progression.
- Death counter increment guard.
- Trap state transitions.
- Level data validation.
- Save data parsing/fallback.

### Integration tests
Verify systems working together:
- Player + platform collision.
- Player + hazard death.
- Death + respawn + trap reset.
- Checkpoint + respawn.
- Exit + level completion + unlock.

### Manual playtests
Use a keyboard and play each level from beginning to completion. Record defects with reproduction steps.

## 3. Core regression checklist

### Startup and rendering
- [ ] Application launches without an exception.
- [ ] Window title is SKILL ISSUE.
- [ ] Logical resolution is 960 × 540.
- [ ] Aspect ratio is preserved on resize.
- [ ] Pixel art uses nearest-neighbor sampling.

### Input
- [ ] A and Left Arrow move left.
- [ ] D and Right Arrow move right.
- [ ] W jumps.
- [ ] W does not auto-jump while held.
- [ ] R restarts current level.
- [ ] Escape pauses/resumes.

### Physics and collision
- [ ] Player lands on solid platforms.
- [ ] Player collides with walls and ceilings.
- [ ] Player falls under gravity.
- [ ] Player cannot pass through normal solid geometry.
- [ ] Moving platform carries the player correctly.
- [ ] Thin hazards do not get skipped at normal speeds.

### Death and retry
- [ ] Each lethal event increments death count exactly once.
- [ ] Automatic respawn is unlimited.
- [ ] Death counter persists through respawn.
- [ ] Manual restart does not increment death count by default.
- [ ] Spawn/checkpoint is safe.
- [ ] Traps reset according to documented policy.

### Traps
- [ ] Hidden spikes trigger and reset.
- [ ] Fake platform collapses and resets.
- [ ] Disappearing floor synchronizes collision/rendering.
- [ ] Falling block activates only when triggered.
- [ ] Moving platform path is stable.
- [ ] Fake exit never completes the level.
- [ ] Bait object activates once.
- [ ] Teleport destination is valid.
- [ ] Rotating saw collision is consistent.

### Progression and UI
- [ ] New save unlocks W1_L1 only.
- [ ] Completing a level unlocks the next level.
- [ ] Locked levels cannot be selected.
- [ ] Pause stops gameplay simulation.
- [ ] Level completion occurs once.
- [ ] Final level displays victory.
- [ ] Save data fallback works if file is absent/corrupt.

## 4. Level playtest record

For each level, record:

| Field | Value |
|---|---|
| Level ID | |
| Tester/build | |
| Completed? | Yes / No |
| Deaths during test | |
| Trap behavior correct? | Yes / No |
| Checkpoint correct? | Yes / No / N/A |
| Soft lock found? | Yes / No |
| Defects | |
| Retest result | |

## 5. Bug report format

Every bug report should include:
- Short title.
- Build/commit identifier.
- Level ID.
- Preconditions.
- Exact reproduction steps.
- Expected result.
- Actual result.
- Severity: blocker / major / minor / cosmetic.
- Screenshot or short recording if useful.

## 6. Severity definitions

- **Blocker:** game cannot launch, campaign cannot progress, or a required level cannot be completed.
- **Major:** core movement, collision, death, respawn, or progression is unreliable.
- **Minor:** non-blocking gameplay or UI issue.
- **Cosmetic:** visual/audio polish issue with no gameplay impact.

## 7. Release quality gate

Do not call the project release-ready until:
- All 15 levels have a recorded successful completion test.
- No blocker or major defect remains.
- The release build launches on the target Windows environment.
- Controls and pause behavior are verified.
- Asset sources/licenses are recorded.
- Documentation matches the delivered implementation.
