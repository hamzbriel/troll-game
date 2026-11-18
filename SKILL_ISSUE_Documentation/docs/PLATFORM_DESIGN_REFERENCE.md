# Platform Design Reference

## Maximum Jump Height

**Physics:**
- Jump Velocity: -550 px/s (upward)
- Gravity: 1500 px/s²
- Formula: max_height = velocity² / (2 × gravity)
- Calculation: 550² / (2 × 1500) = 302500 / 3000 = **100.83 pixels**

**Safe Platform Gap:** 95 pixels vertical (5px safety margin)

## Test Level Layout

```
Spawn Floor (Y=430, standing on Y=460)
    ↓ 55px jump (EASY)
Left Platform (Y=375)
    ↓ 85px jump
Center Platform (Y=290)
    ↓ 80px jump
High Platform (Y=210)
```

All gaps < 95px = All platforms guaranteed reachable.

## Rule for Future Levels

**When designing levels:** 
- Check `GameConfig.MaxJumpHeight` = 95px
- Platform gaps MUST be ≤ 95px vertical
- Add platforms as "stairs" if needed for larger heights
