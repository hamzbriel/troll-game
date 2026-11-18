# 09 — UI/UX Specification

## 1. Screen flow

Recommended flow:

`Main Menu → Level Select → Gameplay → Pause → Gameplay`

Gameplay can also transition to:
- `Level Complete → Next Level`
- `Final Victory → Main Menu`

Death does not transition to a limited-life Game Over screen. It triggers a short death/respawn sequence and returns to active gameplay.

## 2. Main menu

Required elements:
- Game title: SKILL ISSUE.
- Start Game.
- Level Select.
- Controls / How to Play.
- Settings (may be minimal).
- Exit.

The main menu should use the project palette and show a small visual hint of the trap-based premise.

## 3. Level select

- Display 5 worlds and 15 levels.
- Unlocked levels are selectable.
- Locked levels are visibly locked and cannot be started.
- The first level is available in a new save.
- Completing a level unlocks the next level.
- Completing the final level shows campaign completion.
- Do not allow a locked level to launch through keyboard navigation or direct selection.

## 4. Gameplay HUD

Show:
- World and level identifier/name.
- Death counter, e.g. `DEATHS: 07`.
- Optional checkpoint status.

Keep the HUD anchored to screen coordinates, not world coordinates.

## 5. Pause screen

Required actions:
- Resume.
- Restart Level.
- Main Menu.

The game simulation must stop while paused. The pause screen itself remains responsive to input.

## 6. Level complete screen

Show:
- Level complete message.
- Death count for the session or level, with label clearly indicating which.
- Next Level.
- Level Select or Main Menu.

For W5_L3, replace Next Level with a final victory action/menu.

## 7. Final victory screen

Show a clear completion message and a humorous but brief tagline. Provide a way to return to the main menu.

## 8. Controls screen

Show exact controls:
- A / Left Arrow — Move Left.
- D / Right Arrow — Move Right.
- W — Jump.
- R — Restart Level.
- Escape — Pause / Resume.

Do not advertise a control that is not implemented.

## 9. Accessibility and readability

- Use readable contrast.
- Do not communicate hazard state using color alone; pair color with shape, animation, or sound.
- Keep important text within safe margins.
- Avoid rapid full-screen flashing.
- Make menu selection state obvious.
- Support keyboard-only navigation for menus.

## 10. UX acceptance checklist

- [ ] Player can start a new game.
- [ ] Controls are discoverable.
- [ ] Locked levels cannot be launched.
- [ ] HUD death counter updates correctly.
- [ ] Pause freezes gameplay.
- [ ] Resume restores gameplay.
- [ ] Restart resets current level.
- [ ] Level completion unlocks the next level.
- [ ] Final victory can return to the menu.
