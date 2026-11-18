using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SKILL_ISSUE.Core;

namespace SKILL_ISSUE.Traps
{
    public class MovingPlatform : TrapBase
    {
        private readonly Vector2 _startPoint;
        private readonly Vector2 _endPoint;
        private Vector2 _currentPosition;
        private readonly int _width;
        private readonly int _height;
        private bool _movingToEnd;
        private float _platformSpeed;

        public Vector2 Velocity { get; private set; }

        public MovingPlatform(Vector2 startPoint, Vector2 endPoint, int width, int height)
            : base(new Rectangle((int)startPoint.X, (int)startPoint.Y, width, height))
        {
            _startPoint = startPoint;
            _endPoint = endPoint;
            _currentPosition = startPoint;
            _width = width;
            _height = height;
            _movingToEnd = true;
            _platformSpeed = GameConfig.MovingPlatformSpeed;

            State = TrapState.Active;
            UpdateBounds();
            Velocity = Vector2.Zero;
        }

        public override void Update(GameTime gameTime, Rectangle playerBounds)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 target = _movingToEnd ? _endPoint : _startPoint;
            Vector2 direction = target - _currentPosition;
            float distance = direction.Length();

            if (distance < 1f)
            {
                _movingToEnd = !_movingToEnd;
                _currentPosition = target;
                Velocity = Vector2.Zero;
            }
            else
            {
                direction.Normalize();
                Vector2 movement = direction * _platformSpeed * deltaTime;

                if (movement.Length() > distance)
                {
                    _currentPosition = target;
                    Velocity = Vector2.Zero;
                }
                else
                {
                    _currentPosition += movement;
                    Velocity = direction * _platformSpeed;
                }
            }

            UpdateBounds();
        }

        private void UpdateBounds()
        {
            SolidBounds = new Rectangle(
                (int)_currentPosition.X,
                (int)_currentPosition.Y,
                _width,
                _height
            );

            TriggerBounds = SolidBounds;
            HazardBounds = Rectangle.Empty;
        }

        public override void Draw(SpriteBatch spriteBatch, Texture2D pixel)
        {
            spriteBatch.Draw(
                pixel,
                SolidBounds,
                new Color(120, 100, 140)
            );

            for (int i = 0; i < 3; i++)
            {
                spriteBatch.Draw(
                    pixel,
                    new Rectangle(
                        SolidBounds.X + (i * (_width / 3)),
                        SolidBounds.Y + 2,
                        2,
                        2
                    ),
                    new Color(80, 60, 100)
                );
            }
        }

        public override void Reset()
        {
            _currentPosition = _startPoint;
            _movingToEnd = true;
            Velocity = Vector2.Zero;
            UpdateBounds();
        }
    }
}
