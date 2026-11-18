using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SKILL_ISSUE.Core;

namespace SKILL_ISSUE.Traps
{
    public class FakePlatform : TrapBase
    {
        private readonly Rectangle _platformRect;
        private bool _triggered;
        private float _shakeOffset;

        public FakePlatform(Rectangle platformRect) : base(platformRect)
        {
            _platformRect = platformRect;
            TriggerBounds = platformRect;
            SolidBounds = platformRect;
            HazardBounds = Rectangle.Empty;
            _triggered = false;
            _shakeOffset = 0f;
        }

        public override void Update(GameTime gameTime, Rectangle playerBounds)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            switch (State)
            {
                case TrapState.Dormant:
                    if (!_triggered && IsPlayerInTrigger(playerBounds))
                    {
                        _triggered = true;
                        State = TrapState.Warning;
                        _stateTimer = 0f;
                    }
                    break;

                case TrapState.Warning:
                    _stateTimer += deltaTime;
                    _shakeOffset = (float)System.Math.Sin(_stateTimer * 40f) * 2f;

                    if (_stateTimer >= GameConfig.FakePlatformWarningTime)
                    {
                        State = TrapState.Active;
                        _stateTimer = 0f;
                        SolidBounds = Rectangle.Empty;
                    }
                    break;

                case TrapState.Active:
                    _stateTimer += deltaTime;
                    _shakeOffset = 0f;
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Texture2D pixel)
        {
            if (State == TrapState.Active)
            {
                float fadeProgress = System.Math.Min(_stateTimer / GameConfig.FakePlatformFallTime, 1f);
                byte alpha = (byte)((1f - fadeProgress) * 255);
                Color fadeColor = new Color((byte)100, (byte)80, (byte)70, alpha);

                spriteBatch.Draw(
                    pixel,
                    new Rectangle(
                        _platformRect.X,
                        _platformRect.Y + (int)(fadeProgress * 10),
                        _platformRect.Width,
                        _platformRect.Height
                    ),
                    fadeColor
                );
            }
            else
            {
                Color platformColor = State == TrapState.Warning
                    ? new Color(120, 100, 90)
                    : new Color(100, 80, 70);

                spriteBatch.Draw(
                    pixel,
                    new Rectangle(
                        _platformRect.X + (int)_shakeOffset,
                        _platformRect.Y,
                        _platformRect.Width,
                        _platformRect.Height
                    ),
                    platformColor
                );
            }
        }

        public override void Reset()
        {
            State = TrapState.Dormant;
            _stateTimer = 0f;
            _triggered = false;
            _shakeOffset = 0f;
            SolidBounds = _platformRect;
            HazardBounds = Rectangle.Empty;
        }
    }
}
