using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SKILL_ISSUE.Core;

namespace SKILL_ISSUE.Traps
{
    public class TeleportTrap : TrapBase
    {
        private readonly Vector2 _destination;
        private bool _triggered;
        private float _cooldownTimer;

        public bool WasTriggered => _triggered;
        public Vector2 Destination => _destination;

        public TeleportTrap(Rectangle triggerZone, Vector2 destination)
            : base(triggerZone)
        {
            _destination = destination;
            _triggered = false;
            _cooldownTimer = 0f;

            State = TrapState.Dormant;
            HazardBounds = Rectangle.Empty;
            SolidBounds = Rectangle.Empty;
        }

        public override void Update(GameTime gameTime, Rectangle playerBounds)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            switch (State)
            {
                case TrapState.Dormant:
                    if (IsPlayerInTrigger(playerBounds))
                    {
                        _triggered = true;
                        State = TrapState.Cooldown;
                        _cooldownTimer = 0f;
                    }
                    break;

                case TrapState.Cooldown:
                    _cooldownTimer += deltaTime;
                    if (_cooldownTimer >= GameConfig.TeleportCooldown)
                    {
                        State = TrapState.Dormant;
                        _cooldownTimer = 0f;
                    }
                    break;
            }
        }

        public bool ConsumeTeleport()
        {
            if (_triggered)
            {
                _triggered = false;
                return true;
            }
            return false;
        }

        public override void Draw(SpriteBatch spriteBatch, Texture2D pixel)
        {
            Color portalColor = State == TrapState.Cooldown
                ? new Color(100, 50, 150, 150)
                : new Color(150, 100, 200, 200);

            spriteBatch.Draw(pixel, TriggerBounds, portalColor);

            int centerX = TriggerBounds.X + TriggerBounds.Width / 2;
            int centerY = TriggerBounds.Y + TriggerBounds.Height / 2;

            for (int i = 0; i < 3; i++)
            {
                int offset = (i - 1) * 6;
                Rectangle dot = new Rectangle(
                    centerX - 2 + offset,
                    centerY - 2,
                    4,
                    4
                );
                spriteBatch.Draw(pixel, dot, new Color(200, 150, 255));
            }
        }

        public override void Reset()
        {
            State = TrapState.Dormant;
            _triggered = false;
            _cooldownTimer = 0f;
        }
    }
}
