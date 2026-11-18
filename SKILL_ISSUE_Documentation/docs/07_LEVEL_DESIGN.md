# 07 — Level Design Plan

## 1. Campaign structure

The campaign has exactly 15 levels: five worlds, three levels per world.

Each level should be compact and focused. The plan below defines the intended teaching and challenge progression. It is a design target; each layout must be validated in the playable build.

## 2. World overview

| World | Theme | Main lesson |
|---|---|---|
| 1. The Beginning | Grassland | Controls and basic surprise |
| 2. The Unfair Cave | Cave | False platforms and timing |
| 3. The Broken City | Mechanical city | Moving hazards and mechanisms |
| 4. The Dark Dimension | Surreal dark world | Concealment and route reading |
| 5. The Final Joke | Final gauntlet | Mastery and combinations |

## 3. Level specifications

### World 1 — The Beginning

#### W1_L1 — Welcome, Rookie
- Purpose: teach movement, jumping, and a basic visible spike hazard.
- Layout: flat start, small gap, safe platform, visible spikes, exit.
- Traps: visible spikes only.
- Difficulty: very easy.
- Checkpoint: none.
- Exit: obvious door at the far right.
- Tests: movement works; player can clear the gap; spikes kill; exit completes.

#### W1_L2 — The Floor Is Lying
- Purpose: introduce disappearing floor.
- Layout: safe starting platform, suspicious floor segment, landing platform, exit.
- Traps: one disappearing floor section with a warning cue.
- Difficulty: easy.
- Checkpoint: none.
- Exit: reachable by timing a jump.
- Tests: floor changes collision and visibility together; level remains completable.

#### W1_L3 — A Little Too Close
- Purpose: introduce hidden spikes and trigger distance.
- Layout: short corridor with a safe-looking section and a jump route.
- Traps: hidden spikes with warning animation.
- Difficulty: easy.
- Checkpoint: optional near the middle only if required by playtesting.
- Exit: beyond the hazard.
- Tests: trigger fires once; warning is visible; retry is quick.

### World 2 — The Unfair Cave

#### W2_L1 — Trust Issues
- Purpose: introduce fake platforms.
- Layout: several platforms across a cave gap.
- Traps: one fake/collapsing platform among stable platforms.
- Difficulty: easy-medium.
- Checkpoint: none.
- Exit: reachable after learning platform behavior.
- Tests: fake platform collapses only after configured contact.

#### W2_L2 — Don't Look Up
- Purpose: introduce falling blocks.
- Layout: corridor with overhead trigger zones and safe alcoves.
- Traps: falling block/ceiling.
- Difficulty: medium.
- Checkpoint: after the first falling-block sequence.
- Exit: at the far end.
- Tests: block falls only after trigger; player can avoid it; reset restores block.

#### W2_L3 — The Door Is a Liar
- Purpose: introduce fake exit.
- Layout: apparent door near the middle, actual route beyond a short trap sequence.
- Traps: fake exit and one basic hazard.
- Difficulty: medium.
- Checkpoint: before the fake exit sequence.
- Exit: one clearly designated real exit.
- Tests: fake exit never completes the level; real exit always does.

### World 3 — The Broken City

#### W3_L1 — Keep Moving
- Purpose: introduce moving platforms.
- Layout: gaps bridged by platforms moving between endpoints.
- Traps: moving platforms.
- Difficulty: medium.
- Checkpoint: after the first moving-platform section.
- Exit: beyond the final platform.
- Tests: platform carries player correctly; endpoint reversal is stable.

#### W3_L2 — Saw What?
- Purpose: introduce rotating saw hazard.
- Layout: fixed platforms with saws in timed positions.
- Traps: rotating saws.
- Difficulty: medium-hard.
- Checkpoint: before the second saw section.
- Exit: beyond a timing challenge.
- Tests: rotation and collision remain consistent; no unexpected safe/unsafe mismatch.

#### W3_L3 — One Bad Step
- Purpose: combine fake platform and moving hazard.
- Layout: short multi-step traversal with one deceptive surface.
- Traps: fake platform + moving hazard.
- Difficulty: hard.
- Checkpoint: near the middle.
- Exit: final platform after the combined sequence.
- Tests: each trap can be learned independently; combination remains completable.

### World 4 — The Dark Dimension

#### W4_L1 — Now You See It
- Purpose: introduce hidden/temporarily invisible hazard.
- Layout: dim environment with readable silhouettes and subtle cues.
- Traps: hidden spikes or timed hazard.
- Difficulty: hard.
- Checkpoint: before the main hidden-hazard sequence.
- Exit: beyond the sequence.
- Tests: hazard is not permanently impossible to read; cues remain visible.

#### W4_L2 — Wrong Way
- Purpose: introduce teleport trap.
- Layout: branching-looking route with a teleport trigger and safe recovery area.
- Traps: teleport trap and a basic hazard.
- Difficulty: hard.
- Checkpoint: before teleport section.
- Exit: reachable from the intended destination.
- Tests: teleport does not loop indefinitely; destination is valid.

#### W4_L3 — The Long Shortcut
- Purpose: combine hidden hazards, moving platforms, and a fake route.
- Layout: compact route choice with a deceptive shortcut.
- Traps: hidden spikes + moving platform + fake route.
- Difficulty: very hard.
- Checkpoint: at a fair midpoint.
- Exit: beyond the true route.
- Tests: both routes behave consistently; at least one route is completable.

### World 5 — The Final Joke

#### W5_L1 — Everything Is Suspicious
- Purpose: test recognition of all previously introduced traps.
- Layout: short sequence of separate trap rooms/sections.
- Traps: mixed earlier traps, introduced in readable sequence.
- Difficulty: very hard.
- Checkpoint: between major sections.
- Exit: final door.
- Tests: all traps reset correctly; no accidental overlap causes instant chain deaths.

#### W5_L2 — No More Tutorials
- Purpose: test timing and movement mastery.
- Layout: longer precision traversal with moving platforms and timed hazards.
- Traps: moving platforms, falling blocks, spikes, fake platform.
- Difficulty: expert.
- Checkpoint: at least one mid-level checkpoint.
- Exit: after the final timed sequence.
- Tests: all jumps are physically possible using the defined movement tuning.

#### W5_L3 — The Final Joke
- Purpose: final campaign challenge and humorous payoff.
- Layout: final gauntlet followed by a fake victory/exit gag and then the real exit.
- Traps: curated combinations of learned traps; do not introduce a new essential mechanic here.
- Difficulty: expert but fair.
- Checkpoint: before the final gauntlet.
- Exit: actual final exit after the gag.
- Tests: final level is completable; fake victory does not end campaign; real victory screen appears exactly once.

## 4. Level acceptance checklist

For every level:
- [ ] Stable ID and display name exist.
- [ ] Spawn position is safe.
- [ ] Exit exists and is reachable.
- [ ] Every trap has a stable ID and reset behavior.
- [ ] Collision geometry matches the visual layout.
- [ ] Checkpoint behavior matches this specification.
- [ ] Death and retry work.
- [ ] The level can be completed after repeated attempts.
- [ ] No accidental soft lock exists.
- [ ] Difficulty is appropriate for its campaign position.

## 5. Level authoring convention

Use logical world pixels. Document the coordinate origin (top-left of level), dimensions, spawn, exit, and object positions. Prefer data-driven definitions over embedding level geometry in draw code.

If the project uses a JSON level format, include a schema/example and validate it. If it uses C# level builders, keep the data separate from gameplay system behavior.
