using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SKILL_ISSUE.Core;
using SKILL_ISSUE.Graphics;
using SKILL_ISSUE.Input;
using SKILL_ISSUE.Entities;
using SKILL_ISSUE.Physics;
using SKILL_ISSUE.Traps;
using System.Collections.Generic;

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
        private List<TrapBase> _traps;

        // Game state
        private int _deathCount;
        private float _respawnAlpha;

        // Temporary placeholder visuals
        private Texture2D _placeholderPixel;
        private SpriteFont _debugFont; // Will be null until we add a font

        // Spawn point - on left floor, closer to left platform
        private readonly Vector2 _spawnPosition = new Vector2(100, 430);

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
            _traps = new List<TrapBase>();
            _deathCount = 0;
            _respawnAlpha = 1f;

            // Build test level geometry
            BuildTestLevel();

            base.Initialize();
        }

        private void BuildTestLevel()
        {
            _collisionWorld.Clear();
            _traps.Clear();

            // Main floor - NOT full width, has GAP in middle for death testing
            _collisionWorld.AddSolidPlatform(new Rectangle(0, 460, 300, 80));     // Left floor
            _collisionWorld.AddSolidPlatform(new Rectangle(500, 460, 460, 80));   // Right floor
            // GAP at X=300-500 for easy death testing

            // Left wall
            _collisionWorld.AddSolidPlatform(new Rectangle(0, 0, 40, 540));

            // Right wall
            _collisionWorld.AddSolidPlatform(new Rectangle(920, 0, 40, 540));

            // Platform layout - ALL gaps respect MaxJumpHeight = 95px (jump increased to -550)
            // Spawn is at Y=430 (standing on floor at Y=460)

            // Left low platform - 55px above spawn - VERY EASY to reach now
            _collisionWorld.AddSolidPlatform(new Rectangle(140, 375, 120, 20));

            // Center platform - 85px above left platform (within 95px limit)
            _collisionWorld.AddSolidPlatform(new Rectangle(320, 290, 200, 20));

            // Right low platform - same height as left for symmetry
            _collisionWorld.AddSolidPlatform(new Rectangle(600, 375, 120, 20));

            // High platform - 80px above center (within 95px limit)
            _collisionWorld.AddSolidPlatform(new Rectangle(380, 210, 140, 20));

            // Hazard zone - covers GAP and bottom area
            _collisionWorld.AddHazard(new Rectangle(0, 480, 960, 100));

            // === TRAP DEMONSTRATIONS ===

            // 1. Hidden Spikes - on right floor section
            _traps.Add(new HiddenSpikes(
                triggerBounds: new Rectangle(700, 400, 100, 60),
                spikeBasePosition: new Rectangle(700, 460, 100, 10)
            ));

            // 2. Fake Platform - center platform replacement (make one section fake)
            _traps.Add(new FakePlatform(
                platformRect: new Rectangle(420, 290, 80, 20)
            ));

            // 3. Disappearing Floor - left floor section near spawn
            _traps.Add(new DisappearingFloor(
                triggerZone: new Rectangle(200, 300, 60, 100),
                floorRect: new Rectangle(200, 460, 80, 20)
            ));

            // 4. Falling Block - above high platform
            _traps.Add(new FallingBlock(
                triggerZone: new Rectangle(380, 210, 140, 20),
                blockStartPosition: new Vector2(400, 100),
                width: 40,
                height: 40
            ));
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _placeholderPixel = new Texture2D(GraphicsDevice, 1, 1);
            _placeholderPixel.SetData(new[] { Color.White });

            // Note: _debugFont stays null - we'll draw death counter with primitives for now
        }

        protected override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _inputManager.Update();

            if (_inputManager.PausePressed())
                Exit();

            // Handle death state
            if (_player.IsDead)
            {
                if (_player.UpdateDeathState(deltaTime))
                {
                    // Ready to respawn
                    _player.Respawn(_spawnPosition);
                    _respawnAlpha = 0f;
                }
                return; // Skip gameplay update while dead
            }

            // Fade in after respawn
            if (_respawnAlpha < 1f)
            {
                _respawnAlpha += deltaTime / GameConfig.RespawnFadeTime;
                if (_respawnAlpha > 1f)
                    _respawnAlpha = 1f;
            }

            // Manual restart on R key (does NOT count as death)
            if (_inputManager.RestartPressed())
            {
                _player.Respawn(_spawnPosition);
                _respawnAlpha = 1f;
                ResetAllTraps();
            }

            // Get player input
            float horizontalInput = _inputManager.GetHorizontalInput();
            bool jumpPressed = _inputManager.JumpPressed();

            // Update player physics (calculates velocity, does NOT move position yet)
            _player.Update(deltaTime, horizontalInput, jumpPressed);

            // Update all traps
            Rectangle playerBounds = _player.Bounds;
            foreach (var trap in _traps)
            {
                trap.Update(gameTime, playerBounds);
            }

            // Now apply movement with collision detection (includes trap solid bounds)
            ApplyMovementWithCollision(deltaTime);

            // Check hazard collision (death from hazard zones)
            if (_collisionWorld.IsOverlappingHazard(_player.Bounds))
            {
                _player.RequestDeath();
            }

            // Check trap hazards (death from active traps)
            foreach (var trap in _traps)
            {
                if (trap.IsHazardous(_player.Bounds))
                {
                    _player.RequestDeath();
                    break;
                }
            }

            // Consume death request and increment counter
            if (_player.ConsumeDeathRequest())
            {
                _deathCount++;
                ResetAllTraps();
            }

            base.Update(gameTime);
        }

        private void ResetAllTraps()
        {
            foreach (var trap in _traps)
            {
                trap.Reset();
            }
        }

        private void ApplyMovementWithCollision(float deltaTime)
        {
            Vector2 startPosition = _player.Position;
            Vector2 desiredPosition = startPosition + _player.Velocity * deltaTime;

            // Collect all solid bounds (platforms + solid traps)
            List<Rectangle> allSolidBounds = new List<Rectangle>(_collisionWorld.GetSolidPlatforms());
            foreach (var trap in _traps)
            {
                if (trap.IsSolid())
                {
                    allSolidBounds.Add(trap.SolidBounds);
                }
            }

            // Apply horizontal movement with collision
            Vector2 afterHorizontal = new Vector2(desiredPosition.X, startPosition.Y);
            bool hitWall = ResolveHorizontalCollision(
                ref afterHorizontal,
                allSolidBounds,
                _player.Velocity
            );

            if (hitWall)
            {
                _player.StopHorizontal();
            }

            // Apply vertical movement with collision
            Vector2 finalPosition = new Vector2(afterHorizontal.X, desiredPosition.Y);
            var verticalResult = ResolveVerticalCollision(
                ref finalPosition,
                allSolidBounds,
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

            // Update grounded state (check against all solid bounds)
            _player.IsGrounded = IsGroundedAgainstBounds(
                _player.Position,
                allSolidBounds
            );
        }

        private bool ResolveHorizontalCollision(ref Vector2 position, List<Rectangle> solidBounds, Vector2 velocity)
        {
            Rectangle playerBounds = new Rectangle(
                (int)position.X,
                (int)position.Y,
                GameConfig.PlayerWidth,
                GameConfig.PlayerHeight
            );

            foreach (var platform in solidBounds)
            {
                if (playerBounds.Intersects(platform))
                {
                    if (velocity.X > 0)
                        position.X = platform.Left - GameConfig.PlayerWidth;
                    else if (velocity.X < 0)
                        position.X = platform.Right;

                    return true;
                }
            }

            return false;
        }

        private VerticalCollisionResult ResolveVerticalCollision(ref Vector2 position, List<Rectangle> solidBounds, Vector2 velocity)
        {
            Rectangle playerBounds = new Rectangle(
                (int)position.X,
                (int)position.Y,
                GameConfig.PlayerWidth,
                GameConfig.PlayerHeight
            );

            foreach (var platform in solidBounds)
            {
                if (playerBounds.Intersects(platform))
                {
                    if (velocity.Y > 0)
                    {
                        position.Y = platform.Top - GameConfig.PlayerHeight;
                        return VerticalCollisionResult.Landing;
                    }
                    else if (velocity.Y < 0)
                    {
                        position.Y = platform.Bottom;
                        return VerticalCollisionResult.HitCeiling;
                    }
                }
            }

            return VerticalCollisionResult.None;
        }

        private bool IsGroundedAgainstBounds(Vector2 position, List<Rectangle> solidBounds)
        {
            Rectangle checkRect = new Rectangle(
                (int)position.X,
                (int)position.Y + GameConfig.PlayerHeight + 1,
                GameConfig.PlayerWidth,
                1
            );

            foreach (var platform in solidBounds)
            {
                if (checkRect.Intersects(platform))
                    return true;
            }

            return false;
        }

        protected override void Draw(GameTime gameTime)
        {
            // Draw to logical canvas
            GraphicsDevice.SetRenderTarget(_logicalRenderTarget);
            GraphicsDevice.Clear(new Color(20, 12, 28));

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            DrawLevel();
            DrawTraps();
            DrawPlayer();
            DrawDeathCounter();

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
            // Floor with GAP in middle
            _spriteBatch.Draw(_placeholderPixel, new Rectangle(0, 460, 300, 80), new Color(60, 40, 70));
            _spriteBatch.Draw(_placeholderPixel, new Rectangle(500, 460, 460, 80), new Color(60, 40, 70));

            // Walls
            _spriteBatch.Draw(_placeholderPixel, new Rectangle(0, 0, 40, 540), new Color(40, 25, 50));
            _spriteBatch.Draw(_placeholderPixel, new Rectangle(920, 0, 40, 540), new Color(40, 25, 50));

            // Platforms - heights respect 95px max jump (jump velocity increased to -550)
            _spriteBatch.Draw(_placeholderPixel, new Rectangle(140, 375, 120, 20), new Color(150, 100, 120)); // Left: 55px from spawn - EASY
            _spriteBatch.Draw(_placeholderPixel, new Rectangle(320, 290, 200, 20), new Color(180, 120, 140)); // Center: 85px from left
            _spriteBatch.Draw(_placeholderPixel, new Rectangle(600, 375, 120, 20), new Color(150, 100, 120)); // Right: same as left
            _spriteBatch.Draw(_placeholderPixel, new Rectangle(380, 210, 140, 20), new Color(200, 140, 160)); // High: 80px from center

            // Draw GAP clearly with darker background and hazard warning
            _spriteBatch.Draw(_placeholderPixel, new Rectangle(300, 400, 200, 140), new Color(10, 5, 15)); // Dark void

            // Hazard zone indicator (bright red at bottom - visible death zone)
            _spriteBatch.Draw(_placeholderPixel, new Rectangle(0, 480, 960, 60), new Color(220, 50, 80, 80));

            // Exit marker (crimson) - on right floor
            _spriteBatch.Draw(_placeholderPixel, new Rectangle(860, 380, 40, 60), new Color(220, 50, 80));
        }

        private void DrawTraps()
        {
            foreach (var trap in _traps)
            {
                trap.Draw(_spriteBatch, _placeholderPixel);
            }
        }

        private void DrawPlayer()
        {
            if (_player.IsDead)
                return; // Don't draw dead player

            Rectangle playerRect = new Rectangle(
                (int)_player.Position.X,
                (int)_player.Position.Y,
                GameConfig.PlayerWidth,
                GameConfig.PlayerHeight
            );

            // Color based on state with alpha for fade-in
            Color playerColor = _player.IsGrounded ? Color.White : new Color(255, 255, 200);
            playerColor *= _respawnAlpha;

            _spriteBatch.Draw(_placeholderPixel, playerRect, playerColor);
        }

        private void DrawDeathCounter()
        {
            // Death counter with number display and sarcastic messages
            Color textColor = new Color(220, 200, 200);
            Color bgColor = new Color(0, 0, 0, 180);

            // Background panel
            _spriteBatch.Draw(_placeholderPixel, new Rectangle(10, 10, 180, 30), bgColor);

            // Draw "DEATHS:" label using primitive rectangles as letters
            // D
            _spriteBatch.Draw(_placeholderPixel, new Rectangle(15, 15, 2, 20), textColor);
            _spriteBatch.Draw(_placeholderPixel, new Rectangle(15, 15, 8, 2), textColor);
            _spriteBatch.Draw(_placeholderPixel, new Rectangle(15, 33, 8, 2), textColor);
            _spriteBatch.Draw(_placeholderPixel, new Rectangle(21, 15, 2, 20), textColor);

            // Draw death count as small bars (visual representation)
            int barWidth = 3;
            int barSpacing = 4;
            int maxBars = 25;
            int displayBars = _deathCount > maxBars ? maxBars : _deathCount;
            int startX = 30;
            int barY = 18;

            for (int i = 0; i < displayBars; i++)
            {
                _spriteBatch.Draw(_placeholderPixel,
                    new Rectangle(startX + i * barSpacing, barY, barWidth, 14),
                    new Color(220, 50, 80));
            }

            // Draw actual NUMBER
            // Simple digit rendering - just show the count as blocks
            int numberX = 140;
            int numberY = 17;
            DrawNumber(_deathCount, numberX, numberY, textColor);

            // Sarcastic message when death count gets high
            if (_deathCount >= 10)
            {
                string message = GetSarcasticMessage(_deathCount);
                DrawSarcasticMessage(message);
            }
        }

        private void DrawNumber(int number, int x, int y, Color color)
        {
            // Very simple number rendering using rectangles
            // Just draw blocks representing digits (0-9)
            string numStr = number.ToString();
            int digitWidth = 6;
            int digitSpacing = 8;

            for (int i = 0; i < numStr.Length; i++)
            {
                int digit = numStr[i] - '0';
                int digitX = x + i * digitSpacing;
                DrawDigit(digit, digitX, y, color);
            }
        }

        private void DrawDigit(int digit, int x, int y, Color color)
        {
            // Primitive 7-segment style digits using rectangles
            // Very simplified - just draw enough to be readable
            bool[] segments = GetSegments(digit);
            int w = 5; // width
            int h = 2; // segment thickness

            // Top
            if (segments[0]) _spriteBatch.Draw(_placeholderPixel, new Rectangle(x, y, w, h), color);
            // Top-right
            if (segments[1]) _spriteBatch.Draw(_placeholderPixel, new Rectangle(x + w - h, y, h, 5), color);
            // Bottom-right
            if (segments[2]) _spriteBatch.Draw(_placeholderPixel, new Rectangle(x + w - h, y + 8, h, 5), color);
            // Bottom
            if (segments[3]) _spriteBatch.Draw(_placeholderPixel, new Rectangle(x, y + 13, w, h), color);
            // Bottom-left
            if (segments[4]) _spriteBatch.Draw(_placeholderPixel, new Rectangle(x, y + 8, h, 5), color);
            // Top-left
            if (segments[5]) _spriteBatch.Draw(_placeholderPixel, new Rectangle(x, y, h, 5), color);
            // Middle
            if (segments[6]) _spriteBatch.Draw(_placeholderPixel, new Rectangle(x, y + 7, w, h), color);
        }

        private bool[] GetSegments(int digit)
        {
            // 7-segment display: [top, top-right, bottom-right, bottom, bottom-left, top-left, middle]
            return digit switch
            {
                0 => new[] { true, true, true, true, true, true, false },
                1 => new[] { false, true, true, false, false, false, false },
                2 => new[] { true, true, false, true, true, false, true },
                3 => new[] { true, true, true, true, false, false, true },
                4 => new[] { false, true, true, false, false, true, true },
                5 => new[] { true, false, true, true, false, true, true },
                6 => new[] { true, false, true, true, true, true, true },
                7 => new[] { true, true, true, false, false, false, false },
                8 => new[] { true, true, true, true, true, true, true },
                9 => new[] { true, true, true, true, false, true, true },
                _ => new[] { false, false, false, false, false, false, false }
            };
        }

        private string GetSarcasticMessage(int deaths)
        {
            return deaths switch
            {
                >= 50 => "Maybe try a different game?",
                >= 30 => "This is embarrassing...",
                >= 20 => "Are you even trying?",
                >= 15 => "Skill Issue confirmed.",
                >= 10 => "That's a lot of deaths.",
                _ => ""
            };
        }

        private void DrawSarcasticMessage(string message)
        {
            if (string.IsNullOrEmpty(message)) return;

            // Draw message ABOVE death counter with readable text
            int msgY = 50; // Below death counter panel
            int msgX = 10;

            // Background panel - wider for text
            _spriteBatch.Draw(_placeholderPixel,
                new Rectangle(msgX, msgY, 250, 18),
                new Color(220, 50, 80, 240)); // Bright red background

            // Draw actual text using simple pixel font
            DrawSimpleText(message, msgX + 5, msgY + 5, new Color(255, 255, 255));
        }

        private void DrawSimpleText(string text, int x, int y, Color color)
        {
            // Simple 3x5 pixel font for basic readability
            int cursorX = x;
            foreach (char c in text.ToUpper())
            {
                DrawChar(c, cursorX, y, color);
                cursorX += 4; // Character width + spacing
            }
        }

        private void DrawChar(char c, int x, int y, Color color)
        {
            // Very simple 3x5 pixel font - only letters needed for messages
            bool[,] pixels = c switch
            {
                'A' => new bool[,] { {false,true,false}, {true,false,true}, {true,true,true}, {true,false,true}, {true,false,true} },
                'B' => new bool[,] { {true,true,false}, {true,false,true}, {true,true,false}, {true,false,true}, {true,true,false} },
                'C' => new bool[,] { {false,true,true}, {true,false,false}, {true,false,false}, {true,false,false}, {false,true,true} },
                'D' => new bool[,] { {true,true,false}, {true,false,true}, {true,false,true}, {true,false,true}, {true,true,false} },
                'E' => new bool[,] { {true,true,true}, {true,false,false}, {true,true,false}, {true,false,false}, {true,true,true} },
                'F' => new bool[,] { {true,true,true}, {true,false,false}, {true,true,false}, {true,false,false}, {true,false,false} },
                'G' => new bool[,] { {false,true,true}, {true,false,false}, {true,false,true}, {true,false,true}, {false,true,true} },
                'H' => new bool[,] { {true,false,true}, {true,false,true}, {true,true,true}, {true,false,true}, {true,false,true} },
                'I' => new bool[,] { {true,true,true}, {false,true,false}, {false,true,false}, {false,true,false}, {true,true,true} },
                'K' => new bool[,] { {true,false,true}, {true,false,true}, {true,true,false}, {true,false,true}, {true,false,true} },
                'L' => new bool[,] { {true,false,false}, {true,false,false}, {true,false,false}, {true,false,false}, {true,true,true} },
                'M' => new bool[,] { {true,false,true}, {true,true,true}, {true,true,true}, {true,false,true}, {true,false,true} },
                'N' => new bool[,] { {true,false,true}, {true,true,true}, {true,true,true}, {true,false,true}, {true,false,true} },
                'O' => new bool[,] { {false,true,false}, {true,false,true}, {true,false,true}, {true,false,true}, {false,true,false} },
                'R' => new bool[,] { {true,true,false}, {true,false,true}, {true,true,false}, {true,false,true}, {true,false,true} },
                'S' => new bool[,] { {false,true,true}, {true,false,false}, {false,true,false}, {false,false,true}, {true,true,false} },
                'T' => new bool[,] { {true,true,true}, {false,true,false}, {false,true,false}, {false,true,false}, {false,true,false} },
                'U' => new bool[,] { {true,false,true}, {true,false,true}, {true,false,true}, {true,false,true}, {false,true,false} },
                'V' => new bool[,] { {true,false,true}, {true,false,true}, {true,false,true}, {true,false,true}, {false,true,false} },
                'W' => new bool[,] { {true,false,true}, {true,false,true}, {true,true,true}, {true,true,true}, {true,false,true} },
                'Y' => new bool[,] { {true,false,true}, {true,false,true}, {false,true,false}, {false,true,false}, {false,true,false} },
                '?' => new bool[,] { {false,true,false}, {true,false,true}, {false,false,true}, {false,true,false}, {false,true,false} },
                '.' => new bool[,] { {false,false,false}, {false,false,false}, {false,false,false}, {false,false,false}, {false,true,false} },
                ' ' => new bool[,] { {false,false,false}, {false,false,false}, {false,false,false}, {false,false,false}, {false,false,false} },
                _ => new bool[,] { {false,false,false}, {false,false,false}, {false,true,false}, {false,false,false}, {false,false,false} } // Unknown char
            };

            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    if (pixels[row, col])
                    {
                        _spriteBatch.Draw(_placeholderPixel, new Rectangle(x + col, y + row, 1, 1), color);
                    }
                }
            }
        }

        protected override void UnloadContent()
        {
            _logicalRenderTarget?.Dispose();
            _placeholderPixel?.Dispose();
            base.UnloadContent();
        }
    }
}
