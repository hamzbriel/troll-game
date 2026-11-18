using Microsoft.Xna.Framework;
using SKILL_ISSUE.Core;

namespace SKILL_ISSUE.Entities
{
    /// <summary>
    /// Player character with physics-based movement.
    /// Handles horizontal movement, jumping, gravity, and grounded state.
    /// </summary>
    public class Player
    {
        // Position and physics
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }

        // Collision bounds
        public Rectangle Bounds => new Rectangle(
            (int)Position.X,
            (int)Position.Y,
            GameConfig.PlayerWidth,
            GameConfig.PlayerHeight
        );

        // State tracking
        public bool IsGrounded { get; set; }
        private bool _wasGrounded;

        // Jump buffering and coyote time
        private float _coyoteTimeCounter;
        private float _jumpBufferCounter;

        public Player(Vector2 startPosition)
        {
            Position = startPosition;
            Velocity = Vector2.Zero;
            IsGrounded = false;
        }

        /// <summary>
        /// Update player physics and movement.
        /// </summary>
        /// <param name="deltaTime">Elapsed seconds since last frame</param>
        /// <param name="horizontalInput">-1 (left), 0 (none), +1 (right)</param>
        /// <param name="jumpPressed">True if jump was requested this frame</param>
        public void Update(float deltaTime, float horizontalInput, bool jumpPressed)
        {
            _wasGrounded = IsGrounded;

            // Update coyote time (grace period for jumping after leaving ground)
            if (IsGrounded)
            {
                _coyoteTimeCounter = GameConfig.CoyoteTime;
            }
            else
            {
                _coyoteTimeCounter -= deltaTime;
            }

            // Update jump buffer (grace period for early jump input)
            if (jumpPressed)
            {
                _jumpBufferCounter = GameConfig.JumpBuffer;
            }
            else
            {
                _jumpBufferCounter -= deltaTime;
            }

            // Horizontal movement
            UpdateHorizontalMovement(deltaTime, horizontalInput);

            // Jump handling
            if (_jumpBufferCounter > 0f && _coyoteTimeCounter > 0f)
            {
                Jump();
                _jumpBufferCounter = 0f; // Consume jump buffer
            }

            // Apply gravity
            ApplyGravity(deltaTime);

            // Apply velocity to position
            Position += Velocity * deltaTime;
        }

        private void UpdateHorizontalMovement(float deltaTime, float horizontalInput)
        {
            float acceleration = IsGrounded ? GameConfig.PlayerGroundAccel : GameConfig.PlayerAirAccel;
            float deceleration = GameConfig.PlayerGroundDecel;

            if (horizontalInput != 0)
            {
                // Accelerate toward target speed
                float targetSpeed = horizontalInput * GameConfig.PlayerMaxSpeed;
                Velocity.X = MoveTowards(Velocity.X, targetSpeed, acceleration * deltaTime);
            }
            else if (IsGrounded)
            {
                // Decelerate to stop when grounded and no input
                Velocity.X = MoveTowards(Velocity.X, 0f, deceleration * deltaTime);
            }
            // In air with no input: maintain momentum (no air friction yet)

            // Clamp to max speed
            if (Velocity.X > GameConfig.PlayerMaxSpeed)
                Velocity.X = GameConfig.PlayerMaxSpeed;
            if (Velocity.X < -GameConfig.PlayerMaxSpeed)
                Velocity.X = -GameConfig.PlayerMaxSpeed;
        }

        private void ApplyGravity(float deltaTime)
        {
            if (!IsGrounded)
            {
                Velocity.Y += GameConfig.Gravity * deltaTime;

                // Clamp to max fall speed
                if (Velocity.Y > GameConfig.MaxFallSpeed)
                    Velocity.Y = GameConfig.MaxFallSpeed;
            }
        }

        private void Jump()
        {
            Velocity.Y = GameConfig.JumpVelocity;
            IsGrounded = false;
            _coyoteTimeCounter = 0f;
        }

        /// <summary>
        /// Called when player lands on ground. Sets grounded state and stops vertical velocity.
        /// </summary>
        public void Land()
        {
            IsGrounded = true;
            if (Velocity.Y > 0)
                Velocity.Y = 0;
        }

        /// <summary>
        /// Called when player hits ceiling. Cancels upward velocity.
        /// </summary>
        public void HitCeiling()
        {
            if (Velocity.Y < 0)
                Velocity.Y = 0;
        }

        /// <summary>
        /// Stop horizontal movement (e.g., after wall collision).
        /// </summary>
        public void StopHorizontal()
        {
            Velocity.X = 0;
        }

        /// <summary>
        /// Reset player to spawn position with zero velocity.
        /// </summary>
        public void Respawn(Vector2 spawnPosition)
        {
            Position = spawnPosition;
            Velocity = Vector2.Zero;
            IsGrounded = false;
        }

        private float MoveTowards(float current, float target, float maxDelta)
        {
            if (MathF.Abs(target - current) <= maxDelta)
                return target;

            return current + MathF.Sign(target - current) * maxDelta;
        }
    }
}
