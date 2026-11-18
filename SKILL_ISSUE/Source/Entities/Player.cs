using Microsoft.Xna.Framework;
using System;
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
        public bool IsDead { get; private set; }

        private bool _wasGrounded;

        // Jump buffering and coyote time
        private float _coyoteTimeCounter;
        private float _jumpBufferCounter;

        // Death state
        private float _deathTimer;
        private bool _deathRequested;

        public Player(Vector2 startPosition)
        {
            Position = startPosition;
            Velocity = Vector2.Zero;
            IsGrounded = false;
            IsDead = false;
            _deathRequested = false;
        }

        /// <summary>
        /// Request player death. Prevents repeated death increments.
        /// </summary>
        public void RequestDeath()
        {
            if (!IsDead && !_deathRequested)
            {
                _deathRequested = true;
            }
        }

        /// <summary>
        /// Check if death was requested this frame and consume the request.
        /// Used by Game1 to increment death counter once.
        /// </summary>
        public bool ConsumeDeathRequest()
        {
            if (_deathRequested)
            {
                _deathRequested = false;
                IsDead = true;
                _deathTimer = GameConfig.DeathFreezeTime;
                Velocity = Vector2.Zero;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Update death timer. Returns true when ready to respawn.
        /// </summary>
        public bool UpdateDeathState(float deltaTime)
        {
            if (IsDead)
            {
                _deathTimer -= deltaTime;
                return _deathTimer <= 0f;
            }
            return false;
        }

        /// <summary>
        /// Update player physics and movement.
        /// Calculates velocity based on input and physics, but does NOT update position.
        /// Position update happens in Game1 with collision resolution.
        /// </summary>
        /// <param name="deltaTime">Elapsed seconds since last frame</param>
        /// <param name="horizontalInput">-1 (left), 0 (none), +1 (right)</param>
        /// <param name="jumpPressed">True if jump was requested this frame</param>
        public void Update(float deltaTime, float horizontalInput, bool jumpPressed)
        {
            if (IsDead)
                return;

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

            // NOTE: Position is NOT updated here - Game1 handles position integration with collision
        }

        private void UpdateHorizontalMovement(float deltaTime, float horizontalInput)
        {
            float acceleration = IsGrounded ? GameConfig.PlayerGroundAccel : GameConfig.PlayerAirAccel;
            float deceleration = GameConfig.PlayerGroundDecel;

            if (horizontalInput != 0)
            {
                // Accelerate toward target speed
                float targetSpeed = horizontalInput * GameConfig.PlayerMaxSpeed;
                float newVelocityX = MoveTowards(Velocity.X, targetSpeed, acceleration * deltaTime);
                Velocity = new Vector2(newVelocityX, Velocity.Y);
            }
            else if (IsGrounded)
            {
                // Decelerate to stop when grounded and no input
                float newVelocityX = MoveTowards(Velocity.X, 0f, deceleration * deltaTime);
                Velocity = new Vector2(newVelocityX, Velocity.Y);
            }
            // In air with no input: maintain momentum (no air friction yet)

            // Clamp to max speed
            if (Velocity.X > GameConfig.PlayerMaxSpeed)
                Velocity = new Vector2(GameConfig.PlayerMaxSpeed, Velocity.Y);
            if (Velocity.X < -GameConfig.PlayerMaxSpeed)
                Velocity = new Vector2(-GameConfig.PlayerMaxSpeed, Velocity.Y);
        }

        private void ApplyGravity(float deltaTime)
        {
            if (!IsGrounded)
            {
                float newVelocityY = Velocity.Y + GameConfig.Gravity * deltaTime;

                // Clamp to max fall speed
                if (newVelocityY > GameConfig.MaxFallSpeed)
                    newVelocityY = GameConfig.MaxFallSpeed;

                Velocity = new Vector2(Velocity.X, newVelocityY);
            }
        }

        private void Jump()
        {
            Velocity = new Vector2(Velocity.X, GameConfig.JumpVelocity);
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
                Velocity = new Vector2(Velocity.X, 0);
        }

        /// <summary>
        /// Called when player hits ceiling. Cancels upward velocity.
        /// </summary>
        public void HitCeiling()
        {
            if (Velocity.Y < 0)
                Velocity = new Vector2(Velocity.X, 0);
        }

        /// <summary>
        /// Stop horizontal movement (e.g., after wall collision).
        /// </summary>
        public void StopHorizontal()
        {
            Velocity = new Vector2(0, Velocity.Y);
        }

        /// <summary>
        /// Stop vertical movement (e.g., after teleport).
        /// </summary>
        public void StopVertical()
        {
            Velocity = new Vector2(Velocity.X, 0);
        }

        /// <summary>
        /// Reset player to spawn position with zero velocity.
        /// </summary>
        public void Respawn(Vector2 spawnPosition)
        {
            Position = spawnPosition;
            Velocity = Vector2.Zero;
            IsGrounded = false;
            IsDead = false;
            _deathRequested = false;
            _deathTimer = 0f;
        }

        private float MoveTowards(float current, float target, float maxDelta)
        {
            if (Math.Abs(target - current) <= maxDelta)
                return target;

            return current + Math.Sign(target - current) * maxDelta;
        }
    }
}
