using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SKILL_ISSUE.Core;

namespace SKILL_ISSUE.Traps
{
    public class DisappearingFloor : TrapBase
    {
        private readonly Rectangle _floorRect;
        private bool _triggered;

        public DisappearingFloor(Rectangle triggerZone, Rectangle floorRect) : base(triggerZone)
        {
            _floorRect = floorRect;
            SolidBounds = floorRect;
            HazardBounds = Rectangle.Empty;
            _triggered = false;
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
                    if (_stateTimer >= GameConfig.DisappearFloorWarningTime)
                    {
                        State = TrapState.Active;
                        _stateTimer = 0f;
                        SolidBounds = Rectangle.Empty;
                    }
                    break;

                case TrapState.Active:
                    _stateTimer += deltaTime;
                    if (_stateTimer >= GameConfig.DisappearFloorGoneTime)
                    {
                        State = TrapState.Dormant;
                        _stateTimer = 0f;
                        _triggered = false;
                        SolidBounds = _floorRect;
                    }
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Texture2D pixel)
        {
            if (State == TrapState.Active)
            {
                return;
            }

            Color floorColor;
            if (State == TrapState.Warning)
            {
                float blinkFreq = 15f;
                bool blink = (int)(_stateTimer * blinkFreq) % 2 == 0;
                floorColor = blink ? new Color(255, 200, 0) : new Color(100, 80, 70);
            }
            else
            {
                floorColor = new Color(100, 80, 70);
            }

            spriteBatch.Draw(
                pixel,
                _floorRect,
                floorColor
            );
        }

        public override void Reset()
        {
            State = TrapState.Dormant;
            _stateTimer = 0f;
            _triggered = false;
            SolidBounds = _floorRect;
            HazardBounds = Rectangle.Empty;
        }
    }
}
