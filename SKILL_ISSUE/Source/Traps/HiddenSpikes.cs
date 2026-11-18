using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SKILL_ISSUE.Core;

namespace SKILL_ISSUE.Traps
{
    public class HiddenSpikes : TrapBase
    {
        private readonly Rectangle _basePosition;
        private float _spikeHeight;
        private const int MaxSpikeHeight = 20;
        private bool _triggered;

        public HiddenSpikes(Rectangle triggerBounds, Rectangle spikeBasePosition)
            : base(triggerBounds)
        {
            _basePosition = spikeBasePosition;
            _spikeHeight = 0f;
            _triggered = false;
            HazardBounds = Rectangle.Empty;
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
                    if (_stateTimer >= GameConfig.SpikeWarningTime)
                    {
                        State = TrapState.Active;
                        _stateTimer = 0f;
                    }
                    break;

                case TrapState.Active:
                    _stateTimer += deltaTime;

                    if (_stateTimer < GameConfig.SpikeExtendTime)
                    {
                        float progress = _stateTimer / GameConfig.SpikeExtendTime;
                        _spikeHeight = progress * MaxSpikeHeight;
                    }
                    else
                    {
                        _spikeHeight = MaxSpikeHeight;
                    }

                    HazardBounds = new Rectangle(
                        _basePosition.X,
                        _basePosition.Y - (int)_spikeHeight,
                        _basePosition.Width,
                        (int)_spikeHeight
                    );

                    if (_stateTimer >= GameConfig.SpikeExtendTime + GameConfig.SpikeActiveTime)
                    {
                        State = TrapState.Cooldown;
                        _stateTimer = 0f;
                    }
                    break;

                case TrapState.Cooldown:
                    _stateTimer += deltaTime;

                    if (_stateTimer < GameConfig.SpikeRetractTime)
                    {
                        float progress = 1f - (_stateTimer / GameConfig.SpikeRetractTime);
                        _spikeHeight = progress * MaxSpikeHeight;

                        HazardBounds = new Rectangle(
                            _basePosition.X,
                            _basePosition.Y - (int)_spikeHeight,
                            _basePosition.Width,
                            (int)_spikeHeight
                        );
                    }
                    else
                    {
                        _spikeHeight = 0f;
                        HazardBounds = Rectangle.Empty;
                        State = TrapState.Dormant;
                        _triggered = false;
                    }
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Texture2D pixel)
        {
            Color baseColor = new Color(60, 60, 60);
            Color warningColor = new Color(255, 200, 0);
            Color spikeColor = new Color(180, 180, 180);

            spriteBatch.Draw(
                pixel,
                _basePosition,
                State == TrapState.Warning ? warningColor : baseColor
            );

            if (_spikeHeight > 0)
            {
                Rectangle spikeRect = new Rectangle(
                    _basePosition.X,
                    _basePosition.Y - (int)_spikeHeight,
                    _basePosition.Width,
                    (int)_spikeHeight
                );

                spriteBatch.Draw(
                    pixel,
                    spikeRect,
                    spikeColor
                );
            }
        }

        public override void Reset()
        {
            State = TrapState.Dormant;
            _stateTimer = 0f;
            _spikeHeight = 0f;
            _triggered = false;
            HazardBounds = Rectangle.Empty;
        }
    }
}
