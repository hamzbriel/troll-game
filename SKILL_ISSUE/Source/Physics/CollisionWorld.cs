using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SKILL_ISSUE.Physics
{
    /// <summary>
    /// Manages solid platforms and hazard collision detection.
    /// Uses AABB (Axis-Aligned Bounding Box) collision.
    /// </summary>
    public class CollisionWorld
    {
        private readonly List<Rectangle> _solidPlatforms;
        private readonly List<Rectangle> _hazards;

        public CollisionWorld()
        {
            _solidPlatforms = new List<Rectangle>();
            _hazards = new List<Rectangle>();
        }

        /// <summary>
        /// Add a solid platform that blocks player movement.
        /// </summary>
        public void AddSolidPlatform(Rectangle platform)
        {
            _solidPlatforms.Add(platform);
        }

        /// <summary>
        /// Add a hazard zone that kills the player on overlap.
        /// </summary>
        public void AddHazard(Rectangle hazard)
        {
            _hazards.Add(hazard);
        }

        /// <summary>
        /// Clear all collision data (for level reset/load).
        /// </summary>
        public void Clear()
        {
            _solidPlatforms.Clear();
            _hazards.Clear();
        }

        /// <summary>
        /// Get all solid platforms (for trap collision integration).
        /// </summary>
        public List<Rectangle> GetSolidPlatforms()
        {
            return _solidPlatforms;
        }

        /// <summary>
        /// Check if player bounds overlap any solid platform.
        /// Returns the first overlapping platform, or null if none.
        /// </summary>
        public Rectangle? GetOverlappingSolid(Rectangle bounds)
        {
            foreach (var platform in _solidPlatforms)
            {
                if (bounds.Intersects(platform))
                    return platform;
            }
            return null;
        }

        /// <summary>
        /// Check if player bounds overlap any hazard.
        /// </summary>
        public bool IsOverlappingHazard(Rectangle bounds)
        {
            foreach (var hazard in _hazards)
            {
                if (bounds.Intersects(hazard))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Resolve horizontal collision with solids.
        /// Moves player out of collision and returns true if collision occurred.
        /// </summary>
        public bool ResolveHorizontal(ref Vector2 position, int width, int height, Vector2 velocity)
        {
            var testBounds = new Rectangle(
                (int)position.X,
                (int)position.Y,
                width,
                height
            );

            var solid = GetOverlappingSolid(testBounds);
            if (solid.HasValue)
            {
                // Push out horizontally
                if (velocity.X > 0) // Moving right, hit left side of platform
                {
                    position = new Vector2(solid.Value.Left - width, position.Y);
                }
                else if (velocity.X < 0) // Moving left, hit right side of platform
                {
                    position = new Vector2(solid.Value.Right, position.Y);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Resolve vertical collision with solids.
        /// Moves player out of collision and returns collision result.
        /// </summary>
        public VerticalCollisionResult ResolveVertical(ref Vector2 position, int width, int height, Vector2 velocity)
        {
            var testBounds = new Rectangle(
                (int)position.X,
                (int)position.Y,
                width,
                height
            );

            var solid = GetOverlappingSolid(testBounds);
            if (solid.HasValue)
            {
                if (velocity.Y > 0) // Moving down, landing on top
                {
                    position = new Vector2(position.X, solid.Value.Top - height);
                    return VerticalCollisionResult.Landing;
                }
                else if (velocity.Y < 0) // Moving up, hit ceiling
                {
                    position = new Vector2(position.X, solid.Value.Bottom);
                    return VerticalCollisionResult.HitCeiling;
                }
            }
            return VerticalCollisionResult.None;
        }

        /// <summary>
        /// Check if player is standing on ground (has solid platform directly below).
        /// Checks one pixel below player's feet.
        /// </summary>
        public bool IsGrounded(Vector2 position, int width, int height)
        {
            var testBounds = new Rectangle(
                (int)position.X + 2, // Inset slightly to avoid edge cases
                (int)position.Y + height + 1, // One pixel below feet
                width - 4,
                2
            );

            return GetOverlappingSolid(testBounds).HasValue;
        }
    }

    public enum VerticalCollisionResult
    {
        None,
        Landing,
        HitCeiling
    }
}
