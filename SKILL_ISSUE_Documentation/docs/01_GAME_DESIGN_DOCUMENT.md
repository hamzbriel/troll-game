# 01 — Game Design Document (GDD)

## 1. Overview

**Title:** SKILL ISSUE  
**Tagline:** “It's Not a Bug. It's Your Skill Issue.”  
**Genre:** 2D side-scrolling troll platformer  
**Player count:** Single-player  
**Primary platform:** Windows desktop  
**Engine/framework:** MonoGame DesktopGL  
**Logical resolution:** 960 × 540  
**Art direction:** Retro 16-bit-inspired pixel art

SKILL ISSUE is a short-level platform game built around surprise, timing, observation, and repeated attempts. The player traverses compact environments while the world occasionally violates the player's expectations through hidden or reactive hazards.

The game should be challenging and funny, not arbitrary or impossible. A trap may be unexpected on the first attempt, but its activation must be consistent so the player can learn and overcome it.

## 2. Product goals

1. Deliver a complete, playable 2D platformer suitable for a Computer Graphics practical final project.
2. Demonstrate core 2D graphics concepts in visible gameplay.
3. Create a recognizable visual identity using a restrained pixel-art palette.
4. Provide a campaign with 15 increasingly challenging levels.
5. Make repeated attempts quick through unlimited retries and fast respawning.
6. Keep the scope achievable for an individual student project.

## 3. Non-goals

The initial release will not include:

- Multiplayer or online leaderboards.
- Online accounts, cloud saves, telemetry, or networking.
- Procedural infinite levels.
- Large open-world exploration.
- Complex RPG inventory or skill trees.
- Voice acting or cinematic cutscenes.
- A level editor for end users.
- Mobile or console release requirements.

These may be considered only after the required campaign is complete and stable.

## 4. Player fantasy and tone

The player is a small adventurer attempting to escape a deliberately mischievous world. The world appears simple and readable, but objects may behave unexpectedly. The tone is playful, sarcastic, and lightly mischievous rather than graphic or horror-focused.

The game should encourage the player to think: “I understand what happened; I can avoid that next time.”

## 5. Core loop

`Start level → observe → move/jump → trigger or avoid trap → reach exit OR die → increment death counter → respawn → retry`

Level completion unlocks the next level. Death does not remove unlocked progress.

## 6. Pillars

### 6.1 Surprise with consistency
Traps can be hidden or deceptive, but their rules must be deterministic. The same trigger and conditions should produce the same result.

### 6.2 Fast retry
Death-to-respawn time should be short. Avoid long unskippable animations.

### 6.3 Readable platforming
The player, solid surfaces, hazards, and exits must be visually distinguishable.

### 6.4 Compact levels
Each level should focus on one or two main ideas. Avoid excessive level length.

### 6.5 Original identity
Use the broad genre concept as inspiration, but create original characters, level layouts, trap combinations, visual assets, and audio.

## 7. Audience and difficulty

The intended audience is players who enjoy challenging, humorous platformers. Difficulty rises across the 15-level campaign.

Difficulty progression:
- World 1: teach controls and basic trap language.
- World 2: introduce deceptive surfaces and timing.
- World 3: introduce mechanical and moving hazards.
- World 4: combine concealment, timing, and route decisions.
- World 5: test mastery through combinations and a final sequence.

## 8. Campaign structure

The campaign contains exactly five worlds with three levels each:

1. The Beginning — grassland / introductory traps.
2. The Unfair Cave — cave / false platforms and visibility.
3. The Broken City — mechanical ruins / moving hazards.
4. The Dark Dimension — dark surreal world / timing and concealment.
5. The Final Joke — final gauntlet / combinations of learned mechanics.

Stable level IDs: `W1_L1` through `W5_L3`.

## 9. Player character

The protagonist is a small white square-like creature with a simple expressive face. It should be recognizable at small size and easy to animate with a compact sprite sheet.

Required visual states:
- Idle.
- Run.
- Jump/rise.
- Fall.
- Death.
- Respawn (optional short effect).

Gameplay collision dimensions should be independent from decorative sprite details.

## 10. Core feature list

### Required
- Main menu.
- Level selection or campaign progression.
- Player movement and jump.
- Gravity and collision.
- Platforms and hazards.
- Trap triggers.
- Death and respawn.
- Unlimited retry.
- Death counter.
- Pause/resume.
- Restart level.
- Level completion and unlock progression.
- Fifteen playable levels.
- Audio and visual feedback for key events.
- Windows release build.

### Optional after required scope
- Best/total death statistics.
- Hidden bonus route.
- Small cosmetic effects.
- Extra ending gag.
- Settings for volume and fullscreen/windowed mode.

## 11. Success criteria

The project is successful when:
- A new player can start and understand the controls.
- The player can complete all 15 levels.
- All deaths increment the counter exactly once.
- Restart and respawn work reliably.
- No required level is impossible due to a layout or collision bug.
- The game builds and runs on the target Windows environment.
- The code and presentation clearly demonstrate Computer Graphics concepts.
