using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SKILL_ISSUE.Traps
{
    public enum TrapState
    {
        Dormant,
        Warning,
        Active,
        Cooldown
    }

    public abstract class TrapBase
    {
        public TrapState State { get; protected set; }
        public Rectangle TriggerBounds { get; protected set; }
        public Rectangle HazardBounds { get; protected set; }
        public Rectangle SolidBounds { get; protected set; }

        protected float _stateTimer;

        protected TrapBase(Rectangle triggerBounds)
        {
            TriggerBounds = triggerBounds;
            State = TrapState.Dormant;
            _stateTimer = 0f;
        }

        public abstract void Update(GameTime gameTime, Rectangle playerBounds);
        public abstract void Draw(SpriteBatch spriteBatch, Texture2D pixel);
        public abstract void Reset();

        public bool IsPlayerInTrigger(Rectangle playerBounds)
        {
            return TriggerBounds.Intersects(playerBounds);
        }

        public bool IsHazardous(Rectangle playerBounds)
        {
            return State == TrapState.Active && HazardBounds.Intersects(playerBounds);
        }

        public bool IsSolid()
        {
            return !SolidBounds.IsEmpty;
        }
    }
}
