using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SKILL_ISSUE.Core;

namespace SKILL_ISSUE.Traps
{
    public class RotatingSaw : TrapBase
    {
        private readonly Vector2 _startPoint;
        private readonly Vector2 _endPoint;
        private Vector2 _currentPosition;
        private readonly int _radius;
        private float _rotation;
        private bool _movingToEnd;
        private float _moveSpeed;

        public RotatingSaw(Vector2 startPoint, Vector2 endPoint, int radius = 0)
            : base(new Rectangle(
                (int)startPoint.X - GameConfig.SawRadius,
                (int)startPoint.Y - GameConfig.SawRadius,
                GameConfig.SawRadius * 2,
                GameConfig.SawRadius * 2))
        {
            _startPoint = startPoint;
            _endPoint = endPoint;
            _currentPosition = startPoint;
            _radius = radius > 0 ? radius : GameConfig.SawRadius;
            _rotation = 0f;
            _movingToEnd = true;
            _moveSpeed = GameConfig.MovingPlatformSpeed * 0.6f;

            State = TrapState.Active;
            UpdateBounds();
        }

        public override void Update(GameTime gameTime, Rectangle playerBounds)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _rotation += GameConfig.SawRotationSpeed * deltaTime;
            if (_rotation > System.Math.PI * 2)
                _rotation -= (float)(System.Math.PI * 2);

            if (_startPoint != _endPoint)
            {
                Vector2 target = _movingToEnd ? _endPoint : _startPoint;
                Vector2 direction = target - _currentPosition;
                float distance = direction.Length();

                if (distance < 1f)
                {
                    _movingToEnd = !_movingToEnd;
                    _currentPosition = target;
                }
                else
                {
                    direction.Normalize();
                    Vector2 movement = direction * _moveSpeed * deltaTime;

                    if (movement.Length() > distance)
                    {
                        _currentPosition = target;
                    }
                    else
                    {
                        _currentPosition += movement;
                    }
                }
            }

            UpdateBounds();
        }

        private void UpdateBounds()
        {
            HazardBounds = new Rectangle(
                (int)_currentPosition.X - _radius,
                (int)_currentPosition.Y - _radius,
                _radius * 2,
                _radius * 2
            );

            TriggerBounds = HazardBounds;
            SolidBounds = Rectangle.Empty;
        }

        public override void Draw(SpriteBatch spriteBatch, Texture2D pixel)
        {
            Vector2 center = new Vector2(
                HazardBounds.X + HazardBounds.Width / 2,
                HazardBounds.Y + HazardBounds.Height / 2
            );

            spriteBatch.Draw(
                pixel,
                HazardBounds,
                new Color(80, 80, 80)
            );

            int bladeCount = 8;
            for (int i = 0; i < bladeCount; i++)
            {
                float angle = _rotation + (i * (float)System.Math.PI * 2 / bladeCount);
                Vector2 bladeEnd = center + new Vector2(
                    (float)System.Math.Cos(angle) * _radius,
                    (float)System.Math.Sin(angle) * _radius
                );

                Rectangle blade = new Rectangle(
                    (int)bladeEnd.X - 1,
                    (int)bladeEnd.Y - 1,
                    3,
                    3
                );

                spriteBatch.Draw(pixel, blade, new Color(200, 200, 200));
            }

            Rectangle centerDot = new Rectangle(
                (int)center.X - 3,
                (int)center.Y - 3,
                6,
                6
            );
            spriteBatch.Draw(pixel, centerDot, new Color(60, 60, 60));
        }

        public override void Reset()
        {
            _currentPosition = _startPoint;
            _movingToEnd = true;
            _rotation = 0f;
            UpdateBounds();
        }
    }
}
