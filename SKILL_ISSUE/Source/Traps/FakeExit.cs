using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SKILL_ISSUE.Traps
{
    public class FakeExit : TrapBase
    {
        private readonly Rectangle _exitRect;
        private bool _triggered;
        private Vector2 _teleportDestination;
        private bool _hasTeleport;

        public bool WasTriggered => _triggered;
        public Vector2 TeleportDestination => _teleportDestination;

        public FakeExit(Rectangle exitRect, Vector2? teleportDestination = null)
            : base(exitRect)
        {
            _exitRect = exitRect;
            _triggered = false;
            _hasTeleport = teleportDestination.HasValue;
            _teleportDestination = teleportDestination ?? Vector2.Zero;

            TriggerBounds = exitRect;
            SolidBounds = Rectangle.Empty;
            HazardBounds = Rectangle.Empty;
            State = TrapState.Dormant;
        }

        public override void Update(GameTime gameTime, Rectangle playerBounds)
        {
            if (State == TrapState.Dormant && IsPlayerInTrigger(playerBounds))
            {
                _triggered = true;
                State = TrapState.Active;
            }
        }

        public bool ConsumeTrigger()
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
            Color exitColor = State == TrapState.Active
                ? new Color(255, 100, 100)
                : new Color(220, 50, 80);

            spriteBatch.Draw(pixel, _exitRect, exitColor);

            int doorInset = 8;
            Rectangle doorFrame = new Rectangle(
                _exitRect.X + doorInset,
                _exitRect.Y + doorInset,
                _exitRect.Width - doorInset * 2,
                _exitRect.Height - doorInset * 2
            );

            spriteBatch.Draw(pixel, doorFrame, new Color(40, 20, 30));

            if (_hasTeleport)
            {
                Rectangle warningDot = new Rectangle(
                    _exitRect.X + _exitRect.Width / 2 - 2,
                    _exitRect.Y + 5,
                    4,
                    4
                );
                spriteBatch.Draw(pixel, warningDot, new Color(255, 200, 0));
            }
        }

        public override void Reset()
        {
            State = TrapState.Dormant;
            _triggered = false;
        }
    }
}
