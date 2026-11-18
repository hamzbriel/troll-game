namespace SKILL_ISSUE.Core
{
    /// <summary>
    /// Central configuration for game constants and tuning values.
    /// Single source of truth for resolution, physics, and gameplay parameters.
    /// </summary>
    public static class GameConfig
    {
        // Display and rendering
        public const int LogicalWidth = 960;
        public const int LogicalHeight = 540;
        public const string WindowTitle = "SKILL ISSUE";

        // Player physics tuning (initial values from 05_PLAYER_AND_PHYSICS.md)
        public const float PlayerMaxSpeed = 220f;           // px/s
        public const float PlayerGroundAccel = 1800f;       // px/s²
        public const float PlayerGroundDecel = 2200f;       // px/s²
        public const float PlayerAirAccel = 1100f;          // px/s²
        public const float Gravity = 1500f;                 // px/s²
        public const float JumpVelocity = -550f;            // px/s (negative = upward) - INCREASED from -520
        public const float MaxFallSpeed = 850f;             // px/s

        // Optional player features
        public const float CoyoteTime = 0.08f;              // seconds
        public const float JumpBuffer = 0.10f;              // seconds

        // Player collision size (initial, adjust with final sprite)
        public const int PlayerWidth = 24;
        public const int PlayerHeight = 30;

        // Death and respawn
        public const float DeathFreezeTime = 0.3f;          // seconds before respawn starts
        public const float RespawnFadeTime = 0.2f;          // seconds for fade in after respawn

        // IMPORTANT: Maximum jump height calculation
        // Formula: max_height = velocity² / (2 * gravity)
        // max_height = 550² / (2 * 1500) = 302500 / 3000 = 100.83 pixels
        // SAFE PLATFORM GAP: 95 pixels vertical (leave 5px margin for safety)
        public const int MaxJumpHeight = 95;                // Safe vertical gap between platforms

        // Trap timing (deterministic state transitions)
        public const float SpikeWarningTime = 0.4f;         // seconds before spikes extend
        public const float SpikeExtendTime = 0.15f;         // seconds to fully extend
        public const float SpikeActiveTime = 1.5f;          // seconds spikes stay extended
        public const float SpikeRetractTime = 0.15f;        // seconds to retract

        public const float FakePlatformWarningTime = 0.3f;  // seconds of shaking/cracking
        public const float FakePlatformFallTime = 0.2f;     // seconds to disappear

        public const float DisappearFloorWarningTime = 0.25f;
        public const float DisappearFloorGoneTime = 2.0f;   // seconds before reappearing

        public const float FallingBlockWarningTime = 0.4f;
        public const float FallingBlockSpeed = 600f;        // px/s

        public const float MovingPlatformSpeed = 80f;       // px/s
        public const float SawRotationSpeed = 3f;           // radians/s
        public const int SawRadius = 15;                    // pixels

        public const float TeleportCooldown = 0.5f;         // seconds before reactivation
        public const float BaitObjectPulseSpeed = 2f;       // Hz
    }
}
