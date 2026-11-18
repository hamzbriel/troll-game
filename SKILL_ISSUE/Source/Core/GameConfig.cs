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
        public const float JumpVelocity = -520f;            // px/s (negative = upward)
        public const float MaxFallSpeed = 850f;             // px/s

        // Optional player features
        public const float CoyoteTime = 0.08f;              // seconds
        public const float JumpBuffer = 0.10f;              // seconds

        // Player collision size (initial, adjust with final sprite)
        public const int PlayerWidth = 24;
        public const int PlayerHeight = 30;
    }
}
