using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SKILL_ISSUE.Core;

namespace SKILL_ISSUE.Traps
{
    public class FallingBlock : TrapBase
    {
        private readonly Vector2 _startPosition;
        private Vector2 _currentPosition;
        private readonly int _blockWidth;
        private readonly int _blockHeight;
        private bool _triggered;
        private float _fallVelocity;

        public FallingBlock(Rectangle triggerZone, Vector2 blockStartPosition, int width, int height)
            : base(triggerZone)
        {
            _startPosition = blockStartPosition;
            _currentPosition = blockStartPosition;
            _blockWidth = width;
            _blockHeight = height;
            _triggered = false;
            _fallVelocity = 0f;

            HazardBounds = new Rectangle(
                (int)_currentPosition.X,
                (int)_currentPosition.Y,
                _blockWidth,
                _blockHeight
            );
            SolidBounds = Rectangle.Empty;
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
                    if (_stateTimer >= GameConfig.FallingBlockWarningTime)
                    {
                        State = TrapState.Active;
                        _stateTimer = 0f;
                        _fallVelocity = 0f;
                    }
                    break;

                case TrapState.Active:
                    _fallVelocity += GameConfig.Gravity * deltaTime;
                    _currentPosition.Y += _fallVelocity * deltaTime;

                    HazardBounds = new Rectangle(
                        (int)_currentPosition.X,
                        (int)_currentPosition.Y,
                        _blockWidth,
                        _blockHeight
                    );

                    if (_currentPosition.Y > GameConfig.LogicalHeight)
                    {
                        State = TrapState.Cooldown;
                    }
                    break;

                case TrapState.Cooldown:
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Texture2D pixel)
        {
            Color blockColor;

            if (State == TrapState.Warning)
            {
                float blinkFreq = 10f;
                bool blink = (int)(_stateTimer * blinkFreq) % 2 == 0;
                blockColor = blink ? new Color(255, 0, 0) : new Color(80, 80, 80);
            }
            else if (State == TrapState.Cooldown)
            {
                return;
            }
            else
            {
                blockColor = new Color(80, 80, 80);
            }

            spriteBatch.Draw(
                pixel,
                new Rectangle(
                    (int)_currentPosition.X,
                    (int)_currentPosition.Y,
                    _blockWidth,
                    _blockHeight
                ),
                blockColor
            );
        }

        public override void Reset()
        {
            State = TrapState.Dormant;
            _stateTimer = 0f;
            _triggered = false;
            _fallVelocity = 0f;
            _currentPosition = _startPosition;

            HazardBounds = new Rectangle(
                (int)_currentPosition.X,
                (int)_currentPosition.Y,
                _blockWidth,
                _blockHeight
            );
        }
    }
}
