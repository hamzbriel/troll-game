using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SKILL_ISSUE.Core;
using SKILL_ISSUE.Graphics;
using SKILL_ISSUE.Input;
using SKILL_ISSUE.Entities;

namespace SKILL_ISSUE
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private ViewportScaler _viewportScaler;
        private RenderTarget2D _logicalRenderTarget;

        // Game systems
        private InputManager _inputManager;
        private Player _player;

        // Temporary placeholder visuals for Milestone 1-2
        private Texture2D _placeholderPixel;

        // Temporary ground level for testing
        private const float GroundY = 460f;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Set window title
            Window.Title = GameConfig.WindowTitle;

            // Set initial window size (can be resized, viewport will adapt)
            _graphics.PreferredBackBufferWidth = GameConfig.LogicalWidth;
            _graphics.PreferredBackBufferHeight = GameConfig.LogicalHeight;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            // Create viewport scaler for aspect-ratio-preserving rendering
            _viewportScaler = new ViewportScaler(GraphicsDevice, GameConfig.LogicalWidth, GameConfig.LogicalHeight);

            // Create render target for logical 960×540 canvas
            _logicalRenderTarget = new RenderTarget2D(
                GraphicsDevice,
                GameConfig.LogicalWidth,
                GameConfig.LogicalHeight,
                false,
                SurfaceFormat.Color,
                DepthFormat.None,
                0,
                RenderTargetUsage.DiscardContents
            );

            // Initialize game systems
            _inputManager = new InputManager();
            _player = new Player(new Vector2(100, GroundY - GameConfig.PlayerHeight));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Create 1×1 white pixel for placeholder rendering
            _placeholderPixel = new Texture2D(GraphicsDevice, 1, 1);
            _placeholderPixel.SetData(new[] { Color.White });
        }

        protected override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update input state
            _inputManager.Update();

            // Exit on Escape (temporary - will be pause menu later)
            if (_inputManager.PausePressed())
                Exit();

            // Get player input
            float horizontalInput = _inputManager.GetHorizontalInput();
            bool jumpPressed = _inputManager.JumpPressed();

            // Update player physics
            _player.Update(deltaTime, horizontalInput, jumpPressed);

            // Temporary ground collision (Milestone 3 will have proper collision system)
            HandleTemporaryGroundCollision();

            base.Update(gameTime);
        }

        private void HandleTemporaryGroundCollision()
        {
            // Simple ground check for testing
            if (_player.Position.Y + GameConfig.PlayerHeight >= GroundY)
            {
                _player.Position = new Vector2(_player.Position.X, GroundY - GameConfig.PlayerHeight);
                _player.Land();
            }
            else
            {
                _player.IsGrounded = false;
            }

            // Temporary side boundaries
            if (_player.Position.X < 40)
            {
                _player.Position = new Vector2(40, _player.Position.Y);
                _player.StopHorizontal();
            }
            if (_player.Position.X > 896) // 960 - 40 - 24
            {
                _player.Position = new Vector2(896, _player.Position.Y);
                _player.StopHorizontal();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            // Step 1: Draw to logical 960×540 render target
            GraphicsDevice.SetRenderTarget(_logicalRenderTarget);
            GraphicsDevice.Clear(new Color(20, 12, 28)); // Dark purple background

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp); // Pixel-art sampling

            // Placeholder scene
            DrawPlaceholderScene();

            // Draw player at current position
            DrawPlayer();

            _spriteBatch.End();

            // Step 2: Draw logical canvas to screen with aspect ratio preservation
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black); // Letterbox bars

            Rectangle viewport = _viewportScaler.GetScaledViewport();

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(_logicalRenderTarget, viewport, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawPlaceholderScene()
        {
            // Floor (bottom 80px)
            _spriteBatch.Draw(_placeholderPixel, new Rectangle(0, 460, 960, 80), new Color(60, 40, 70));

            // Left wall
            _spriteBatch.Draw(_placeholderPixel, new Rectangle(0, 0, 40, 540), new Color(40, 25, 50));

            // Right wall
            _spriteBatch.Draw(_placeholderPixel, new Rectangle(920, 0, 40, 540), new Color(40, 25, 50));

            // Platform (center, elevated) - will be solid collision in Milestone 3
            _spriteBatch.Draw(_placeholderPixel, new Rectangle(380, 320, 200, 20), new Color(180, 120, 140));

            // Exit placeholder (crimson rectangle, top-right)
            _spriteBatch.Draw(_placeholderPixel, new Rectangle(860, 380, 40, 60), new Color(220, 50, 80));
        }

        private void DrawPlayer()
        {
            // Draw player as white rectangle at physics position
            Rectangle playerRect = new Rectangle(
                (int)_player.Position.X,
                (int)_player.Position.Y,
                GameConfig.PlayerWidth,
                GameConfig.PlayerHeight
            );

            // Tint slightly based on state (visual feedback)
            Color playerColor = _player.IsGrounded ? Color.White : new Color(255, 255, 200);

            _spriteBatch.Draw(_placeholderPixel, playerRect, playerColor);
        }

        protected override void UnloadContent()
        {
            _logicalRenderTarget?.Dispose();
            _placeholderPixel?.Dispose();
            base.UnloadContent();
        }
    }
}
