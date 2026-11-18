using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SKILL_ISSUE.Core;
using SKILL_ISSUE.Graphics;
using SKILL_ISSUE.Input;
using SKILL_ISSUE.Entities;
using SKILL_ISSUE.Physics;

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
        private CollisionWorld _collisionWorld;

        // Temporary placeholder visuals
        private Texture2D _placeholderPixel;

        // Spawn point
        private readonly Vector2 _spawnPosition = new Vector2(100, 400);

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Window.Title = GameConfig.WindowTitle;

            _graphics.PreferredBackBufferWidth = GameConfig.LogicalWidth;
            _graphics.PreferredBackBufferHeight = GameConfig.LogicalHeight;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            _viewportScaler = new ViewportScaler(GraphicsDevice, GameConfig.LogicalWidth, GameConfig.LogicalHeight);

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

            _inputManager = new InputManager();
            _player = new Player(_spawnPosition);
            _collisionWorld = new CollisionWorld();

            // Build test level geometry
            BuildTestLevel();

            base.Initialize();
        }

        private void BuildTestLevel()
        {
            _collisionWorld.Clear();

            // Floor (bottom 80px)
            _collisionWorld.AddSolidPlatform(new Rectangle(0, 460, 960, 80));

            // Left wall
            _collisionWorld.AddSolidPlatform(new Rectangle(0, 0, 40, 540));

            // Right wall
            _collisionWorld.AddSolidPlatform(new Rectangle(920, 0, 40, 540));

            // Center elevated platform
            _collisionWorld.AddSolidPlatform(new Rectangle(380, 320, 200, 20));

            // Left step platform
            _collisionWorld.AddSolidPlatform(new Rectangle(120, 380, 100, 20));

            // Right step platform
            _collisionWorld.AddSolidPlatform(new Rectangle(740, 380, 100, 20));

            // High platform (requires jump from center)
            _collisionWorld.AddSolidPlatform(new Rectangle(420, 220, 120, 20));

            // Fall boundary (death zone below screen)
            _collisionWorld.AddHazard(new Rectangle(-100, 550, 1160, 100));
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _placeholderPixel = new Texture2D(GraphicsDevice, 1, 1);
            _placeholderPixel.SetData(new[] { Color.White });
        }

        protected override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _inputManager.Update();

            if (_inputManager.PausePressed())
                Exit();

            // Restart on R key
            if (_inputManager.RestartPressed())
            {
                _player.Respawn(_spawnPosition);
            }

            // Get player input
            float horizontalInput = _inputManager.GetHorizontalInput();
            bool jumpPressed = _inputManager.JumpPressed();

            // Update player physics (calculates velocity, does NOT move position yet)
            _player.Update(deltaTime, horizontalInput, jumpPressed);

            // Now apply movement with collision detection
            ApplyMovementWithCollision(deltaTime);

            // Check hazard collision (death)
            if (_collisionWorld.IsOverlappingHazard(_player.Bounds))
            {
                _player.Respawn(_spawnPosition);
            }

            base.Update(gameTime);
        }

        private void ApplyMovementWithCollision(float deltaTime)
        {
            Vector2 startPosition = _player.Position;
            Vector2 desiredPosition = startPosition + _player.Velocity * deltaTime;

            // Apply horizontal movement with collision
            Vector2 afterHorizontal = new Vector2(desiredPosition.X, startPosition.Y);
            bool hitWall = _collisionWorld.ResolveHorizontal(
                ref afterHorizontal,
                GameConfig.PlayerWidth,
                GameConfig.PlayerHeight,
                _player.Velocity
            );

            if (hitWall)
            {
                _player.StopHorizontal();
            }

            // Apply vertical movement with collision
            Vector2 finalPosition = new Vector2(afterHorizontal.X, desiredPosition.Y);
            var verticalResult = _collisionWorld.ResolveVertical(
                ref finalPosition,
                GameConfig.PlayerWidth,
                GameConfig.PlayerHeight,
                _player.Velocity
            );

            // Update player position
            _player.Position = finalPosition;

            // Handle vertical collision results
            if (verticalResult == VerticalCollisionResult.Landing)
            {
                _player.Land();
            }
            else if (verticalResult == VerticalCollisionResult.HitCeiling)
            {
                _player.HitCeiling();
            }

            // Update grounded state
            _player.IsGrounded = _collisionWorld.IsGrounded(
                _player.Position,
                GameConfig.PlayerWidth,
                GameConfig.PlayerHeight
            );
        }

        protected override void Draw(GameTime gameTime)
        {
            // Draw to logical canvas
            GraphicsDevice.SetRenderTarget(_logicalRenderTarget);
            GraphicsDevice.Clear(new Color(20, 12, 28));

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            DrawLevel();
            DrawPlayer();

            _spriteBatch.End();

            // Draw to screen with scaling
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            Rectangle viewport = _viewportScaler.GetScaledViewport();

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(_logicalRenderTarget, viewport, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawLevel()
        {
            // Floor
            _spriteBatch.Draw(_placeholderPixel, new Rectangle(0, 460, 960, 80), new Color(60, 40, 70));

            // Walls
            _spriteBatch.Draw(_placeholderPixel, new Rectangle(0, 0, 40, 540), new Color(40, 25, 50));
            _spriteBatch.Draw(_placeholderPixel, new Rectangle(920, 0, 40, 540), new Color(40, 25, 50));

            // Platforms (color by height for visual distinction)
            _spriteBatch.Draw(_placeholderPixel, new Rectangle(380, 320, 200, 20), new Color(180, 120, 140));
            _spriteBatch.Draw(_placeholderPixel, new Rectangle(120, 380, 100, 20), new Color(150, 100, 120));
            _spriteBatch.Draw(_placeholderPixel, new Rectangle(740, 380, 100, 20), new Color(150, 100, 120));
            _spriteBatch.Draw(_placeholderPixel, new Rectangle(420, 220, 120, 20), new Color(200, 140, 160));

            // Exit marker (crimson)
            _spriteBatch.Draw(_placeholderPixel, new Rectangle(860, 380, 40, 60), new Color(220, 50, 80));
        }

        private void DrawPlayer()
        {
            Rectangle playerRect = new Rectangle(
                (int)_player.Position.X,
                (int)_player.Position.Y,
                GameConfig.PlayerWidth,
                GameConfig.PlayerHeight
            );

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
