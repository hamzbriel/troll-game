using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SKILL_ISSUE.Core;

namespace SKILL_ISSUE.Traps
{
    public class BaitObject : TrapBase
    {
        private readonly Vector2 _position;
        private readonly int _size;
        private bool _collected;
        private float _pulseTimer;

        public bool WasCollected => _collected;

        public BaitObject(Vector2 position, int size = 16)
            : base(new Rectangle((int)position.X, (int)position.Y, size, size))
        {
            _position = position;
            _size = size;
            _collected = false;
            _pulseTimer = 0f;

            State = TrapState.Dormant;
            HazardBounds = Rectangle.Empty;
            SolidBounds = Rectangle.Empty;
        }

        public override void Update(GameTime gameTime, Rectangle playerBounds)
        {
            if (_collected)
                return;

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _pulseTimer += deltaTime * GameConfig.BaitObjectPulseSpeed;

            if (IsPlayerInTrigger(playerBounds))
            {
                _collected = true;
                State = TrapState.Active;
            }
        }

        public bool ConsumeCollection()
        {
            if (_collected)
            {
                return true;
            }
            return false;
        }

        public override void Draw(SpriteBatch spriteBatch, Texture2D pixel)
        {
            if (_collected)
                return;

            float pulse = (float)System.Math.Sin(_pulseTimer * System.Math.PI * 2) * 0.3f + 0.7f;
            Color baitColor = new Color(
                (byte)(255 * pulse),
                (byte)(220 * pulse),
                (byte)(0)
            );

            Rectangle baitRect = new Rectangle(
                (int)_position.X,
                (int)_position.Y,
                _size,
                _size
            );

            spriteBatch.Draw(pixel, baitRect, baitColor);

            int innerSize = _size - 4;
            Rectangle innerRect = new Rectangle(
                (int)_position.X + 2,
                (int)_position.Y + 2,
                innerSize,
                innerSize
            );

            spriteBatch.Draw(pixel, innerRect, new Color(255, 255, 200));
        }

        public override void Reset()
        {
            _collected = false;
            _pulseTimer = 0f;
            State = TrapState.Dormant;
        }
    }
}
