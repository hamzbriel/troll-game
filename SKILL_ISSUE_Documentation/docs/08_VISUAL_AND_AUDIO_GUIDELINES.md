# 08 — Visual and Audio Guidelines

## 1. Art direction

SKILL ISSUE uses a retro 16-bit-inspired pixel-art style. The presentation should feel playful, readable, mischievous, and slightly ominous.

## 2. Palette direction

Use a consistent palette. Suggested starting colors (adjust only with a documented art reason):

| Role | Color | Hex |
|---|---|---|
| Deep background | Near-black purple | `#211A2B` |
| Secondary background | Dark purple | `#38283F` |
| Main crimson | Crimson red | `#B8324A` |
| Dark crimson | Deep red | `#76263D` |
| Cream highlight | Warm cream | `#F1D6B5` |
| Main character | Off-white | `#FFF4E8` |
| Text | Near-white | `#F8F1EA` |
| Warning accent | Warm yellow | `#F2C45C` |

These are palette starting points, not a requirement to use every color in every asset.

## 3. Pixel density and rendering

- Use a consistent pixel scale across characters, tiles, and UI.
- Use nearest-neighbor sampling (`PointClamp`).
- Avoid blur, smooth filtering, and fractional scaling where possible.
- Keep silhouettes clear at gameplay size.
- Use a transparent background for sprites where appropriate.
- Keep collision boxes independent from decorative sprite pixels.

## 4. Character art

The player is a small white square-like creature with a simple face. The silhouette should remain readable against all level backgrounds.

Required animations:
- Idle: 2–4 frames.
- Run: 4–6 frames.
- Jump/rise: 1–2 frames.
- Fall: 1–2 frames.
- Death: 3–5 frames or a short visual effect.
- Respawn: optional short blink/pop-in.

The exact sprite dimensions can be selected during asset production. Keep dimensions consistent within an animation sheet and document the frame layout.

## 5. Environment art

World themes:
- World 1: grassland, warm sky, crimson-edged platforms.
- World 2: cave, dark stone, glowing warning accents.
- World 3: broken city, machinery, metal platforms.
- World 4: dark dimension, surreal silhouettes, controlled contrast.
- World 5: final gauntlet, visual motifs from previous worlds.

Gameplay surfaces must be visually distinguishable:
- Solid platform.
- Decorative background object.
- Hazard.
- Trigger hint.
- Exit.
- Checkpoint.

Do not make every surface deceptive. The player needs a visual language to learn.

## 6. UI style

- Use a pixel-style font with strong readability.
- Use consistent border thickness and spacing.
- Use crimson as an accent, not as the only text color.
- Main menu should communicate the title and start action immediately.
- HUD should not obscure hazards or the player.
- Use short humorous copy sparingly.

## 7. Animation and effects

Recommended effects:
- Small dust particles on landing.
- Brief squash/stretch on landing, if it does not alter collision.
- Flash or particles on death.
- Warning shake/blink for traps.
- Small activation sparkle for checkpoints.
- Subtle parallax background only if performance and readability remain good.

Effects must not hide lethal collision areas.

## 8. Audio direction

Use original or properly licensed sounds.
- UI confirm/cancel.
- Jump.
- Landing.
- Trap warning.
- Trap activation.
- Death.
- Respawn.
- Checkpoint activation.
- Level complete.
- Background music per world (optional).

Keep sounds short and avoid excessively loud or repetitive effects. Add simple volume settings if time permits.

## 9. Asset register

Maintain `Assets/ASSET_REGISTER.md` with:
- Asset filename.
- Type.
- Author/source.
- License/permission.
- Intended use.
- Import/content pipeline notes.

Never use ripped or extracted assets from commercial reference games.
