# 06 — Trap System Specification

## 1. Design principles

Traps are the signature mechanic of SKILL ISSUE. They surprise the player, but must behave consistently and remain completable.

Each trap should have:
- Stable ID.
- World-space bounds.
- Activation condition.
- Explicit state.
- Update behavior.
- Collision/hazard behavior.
- Visual feedback.
- Reset behavior.
- Optional sound cue.
- Test cases.

## 2. Common trap lifecycle

A trap may use these states where relevant:

`Dormant → Warning → Active → Cooldown → Dormant`

Not every trap needs every state. State transitions must be deterministic and use elapsed time, not frame counts.

## 3. Trap types in the initial game

### 3.1 Hidden Spikes
- Concept: spikes emerge from an apparently safe surface.
- Trigger: player enters a configured proximity/trigger zone.
- Sequence: hidden → warning cue → extend → active hazard → retract/reset if designed to repeat.
- Must not activate repeatedly every frame in a way that restarts the animation.
- Test: enter trigger, observe warning, verify active collision, die once, respawn, verify reset.

### 3.2 Fake Platform
- Concept: a platform appears solid but collapses after contact.
- Trigger: player lands on it or touches its configured trigger.
- Sequence: stable appearance → brief warning/crack → fall or disappear.
- Define whether it can reset after death; default: yes, on level reset/respawn reset.
- Test: stand on it, confirm collapse timing and that it cannot remain incorrectly solid after disappearing.

### 3.3 Disappearing Floor
- Concept: a floor section vanishes when the player crosses a trigger.
- Trigger: player crosses a trigger region.
- Sequence: safe → warning → hidden/non-solid → optional reappear after delay.
- Test: confirm collision and rendering state remain synchronized.

### 3.4 Falling Block / Ceiling
- Concept: a block drops from above after the player enters a trigger area.
- Trigger: player enters the activation region.
- Sequence: idle → warning → falling → impact → settled/reset.
- Hazard: block may kill on contact or crush against configured geometry.
- Test: ensure the block does not fall before the trigger and resets on level restart.

### 3.5 Moving Platform
- Concept: platform travels between defined endpoints.
- Trigger: always moving or activated by proximity, as configured.
- Motion: deterministic path and speed.
- Carry behavior: if the player stands on it, platform should carry the player without jitter or clipping.
- Test: ride platform, jump off, verify endpoint reversal and collision.

### 3.6 Fake Exit
- Concept: an apparent exit is a deception.
- Trigger: player reaches the door.
- Behavior: show a harmless gag, reveal a new obstacle, or redirect to a nearby safe area. Do not soft-lock the level.
- The actual exit must remain reachable.
- Test: fake exit never marks level complete; real exit always does.

### 3.7 Fake Coin / Bait Object
- Concept: a collectible-looking object is a trap trigger.
- Trigger: player touches it.
- Behavior: activate a documented nearby trap or reveal a hazard.
- Do not make every collectible deceptive; use sparingly so the visual language remains meaningful.
- Test: activation occurs once and the player can recover/retry.

### 3.8 Teleport Trap
- Concept: player is moved to a configured destination.
- Trigger: overlap with trigger bounds.
- Destination must be safe or intentionally lethal with a clear level design reason.
- Prevent repeated teleport loops by disabling/rearming the trigger correctly.
- Test: destination placement, velocity reset policy, and reactivation.

### 3.9 Rotating Saw
- Concept: moving/rotating lethal hazard.
- Motion: rotate visually around a stable center and optionally move along a path.
- Collision: use a documented conservative AABB/circle approximation; do not assume rotated sprite bounds automatically provide accurate collision.
- Test: visual rotation, hazard bounds, and reset.

## 4. Trigger and collision separation

Do not use one collision flag for all purposes. A trap can have:
- Solid bounds.
- Trigger bounds.
- Hazard bounds.

These may overlap but have different behavior.

## 5. Reset contract

On level reset:
- Reset all trap states.
- Reset positions and timers.
- Restore solid/non-solid states.
- Restore moving platforms to initial positions.
- Reset one-shot trigger flags.
- Clear temporary trap effects.

On checkpoint respawn, use a consistent level policy. Default: traps reset to their initial level state unless a specific level design entry explicitly requires a persistent state. Do not implement mixed behavior accidentally.

## 6. Authoring and balancing rules

- A level should introduce a trap type before combining it with another type.
- Provide visual or audio hints where possible.
- Avoid placing a trigger so close to a hazard that the player has no reaction opportunity unless the intended challenge is a deliberate surprise with a safe retry.
- Every trap needs a reproducible test.
- Trap behavior must not depend on random chance in the initial campaign.

## 7. Trap implementation checklist

- [ ] Trigger is activated once per intended activation.
- [ ] Warning/active timing is deterministic.
- [ ] Visual state matches collision state.
- [ ] Death is requested once.
- [ ] Level reset restores the initial state.
- [ ] Checkpoint respawn follows documented reset policy.
- [ ] No trap can permanently block the exit.
- [ ] The level remains completable.
