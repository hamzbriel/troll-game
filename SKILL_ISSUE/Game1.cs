using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SKILL_ISSUE.Core;
using SKILL_ISSUE.Graphics;
using SKILL_ISSUE.Input;
using SKILL_ISSUE.Entities;
using SKILL_ISSUE.Physics;
using SKILL_ISSUE.Traps;
using SKILL_ISSUE.Level;
using SKILL_ISSUE.UI;
using System.Collections.Generic;
using System.IO;

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
        private WorldProgressionManager _progression;

        // Current level
        private LevelData _currentLevel;
        private Vector2 _spawnPosition;
        private Rectangle _exitBounds;

        // Game state
        private GameState _gameState;
        private int _deathCount;
        private float _respawnAlpha;

        // UI
        private MainMenu _mainMenu;
        private LevelSelectMenu _levelSelectMenu;
        private PauseMenu _pauseMenu;
        private LevelCompleteScreen _levelCompleteScreen;

        // Temporary placeholder visuals
        private Texture2D _placeholderPixel;
        private SpriteFont _debugFont;

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
            _collisionWorld = new CollisionWorld();
            _traps = new List<TrapBase>();
            _progression = new WorldProgressionManager();
            _deathCount = 0;
            _respawnAlpha = 1f;

            // Initialize UI
            _mainMenu = new MainMenu();
            _pauseMenu = new PauseMenu();
            _levelCompleteScreen = new LevelCompleteScreen();
            _gameState = GameState.MainMenu;

            // Level select will be initialized after progression manager
            _levelSelectMenu = new LevelSelectMenu(_progression);

            base.Initialize();
        }

        private void StartGame()
        {
            LoadLevel("W1_L1");
            _player = new Player(_spawnPosition);
            _gameState = GameState.Playing;
        }

        private void LoadLevel(string levelId)
        {
            string levelPath = Path.Combine(Content.RootDirectory, "Levels", $"{levelId}.json");

            if (!File.Exists(levelPath))
            {
                throw new FileNotFoundException($"Level file not found: {levelPath}");
            }

            string jsonContent = File.ReadAllText(levelPath);
            _currentLevel = LevelLoader.LoadFromJson(jsonContent);

            _spawnPosition = _currentLevel.SpawnPosition;
            _exitBounds = _currentLevel.ExitBounds;

            _collisionWorld.Clear();
            _traps.Clear();

            foreach (var platform in _currentLevel.Platforms)
            {
                _collisionWorld.AddSolidPlatform(new Rectangle(platform.X, platform.Y, platform.Width, platform.Height));
            }

            foreach (var hazard in _currentLevel.Hazards)
            {
                _collisionWorld.AddHazard(new Rectangle(hazard.X, hazard.Y, hazard.Width, hazard.Height));
            }

            _traps = LevelLoader.CreateTrapsFromData(_currentLevel.Traps);

            _progression.CurrentLevelId = levelId;
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

            switch (_gameState)
            {
                case GameState.MainMenu:
                    UpdateMainMenu();
                    break;

                case GameState.LevelSelect:
                    UpdateLevelSelect();
                    break;

                case GameState.Playing:
                    UpdatePlaying(gameTime, deltaTime);
                    break;

                case GameState.Paused:
                    UpdatePaused();
                    break;

                case GameState.LevelComplete:
                    UpdateLevelComplete(deltaTime);
                    break;

                case GameState.GameComplete:
                    if (_inputManager.MenuSelectPressed())
                        _gameState = GameState.MainMenu;
                    break;
            }

            base.Update(gameTime);
        }

        private void UpdateMainMenu()
        {
            _mainMenu.Update(
                _inputManager.MenuUpPressed(),
                _inputManager.MenuDownPressed(),
                _inputManager.MenuSelectPressed(),
                out MenuAction action
            );

            if (action == MenuAction.StartGame)
            {
                _gameState = GameState.LevelSelect;
            }
            else if (action == MenuAction.Exit)
            {
                Exit();
            }
        }

        private void UpdateLevelSelect()
        {
            _levelSelectMenu.Update(
                _inputManager.MenuUpPressed(),
                _inputManager.MenuDownPressed(),
                _inputManager.MenuLeftPressed(),
                _inputManager.MenuRightPressed(),
                _inputManager.MenuSelectPressed(),
                _inputManager.MenuBackPressed(),
                out LevelSelectAction action,
                out string selectedLevelId
            );

            if (action == LevelSelectAction.SelectLevel)
            {
                LoadLevel(selectedLevelId);
                _player = new Player(_spawnPosition);
                _deathCount = 0;
                _respawnAlpha = 1f;
                _gameState = GameState.Playing;
            }
            else if (action == LevelSelectAction.Back)
            {
                _gameState = GameState.MainMenu;
            }
        }

        private void UpdatePlaying(GameTime gameTime, float deltaTime)
        {
            // Pause
            if (_inputManager.PausePressed())
            {
                _pauseMenu.Reset();
                _gameState = GameState.Paused;
                return;
            }

            // Handle death state
            if (_player.IsDead)
            {
                if (_player.UpdateDeathState(deltaTime))
                {
                    _player.Respawn(_spawnPosition);
                    _respawnAlpha = 0f;
                }
                return;
            }

            // Fade in after respawn
            if (_respawnAlpha < 1f)
            {
                _respawnAlpha += deltaTime / GameConfig.RespawnFadeTime;
                if (_respawnAlpha > 1f)
                    _respawnAlpha = 1f;
            }

            // Manual restart on R key
            if (_inputManager.RestartPressed())
            {
                _player.Respawn(_spawnPosition);
                _respawnAlpha = 1f;
                ResetAllTraps();
            }

            // Get player input
            float horizontalInput = _inputManager.GetHorizontalInput();
            bool jumpPressed = _inputManager.JumpPressed();

            _player.Update(deltaTime, horizontalInput, jumpPressed);

            // Update all traps
            Rectangle playerBounds = _player.Bounds;
            foreach (var trap in _traps)
            {
                trap.Update(gameTime, playerBounds);
            }

            ApplyMovementWithCollision(deltaTime);

            // Check hazard collision
            if (_collisionWorld.IsOverlappingHazard(_player.Bounds))
            {
                _player.RequestDeath();
            }

            // Check trap hazards
            foreach (var trap in _traps)
            {
                if (trap.IsHazardous(_player.Bounds))
                {
                    _player.RequestDeath();
                    break;
                }
            }

            HandleSpecialTraps();

            // Check exit collision
            if (_player.Bounds.Intersects(_exitBounds))
            {
                CompleteLevel();
            }

            // Consume death request
            if (_player.ConsumeDeathRequest())
            {
                _deathCount++;
                ResetAllTraps();
            }
        }

        private void UpdatePaused()
        {
            _pauseMenu.Update(
                _inputManager.MenuUpPressed(),
                _inputManager.MenuDownPressed(),
                _inputManager.MenuSelectPressed(),
                out PauseAction action
            );

            if (action == PauseAction.Resume || _inputManager.PausePressed())
            {
                _gameState = GameState.Playing;
            }
            else if (action == PauseAction.Restart)
            {
                _player.Respawn(_spawnPosition);
                _respawnAlpha = 1f;
                ResetAllTraps();
                _deathCount = 0;
                _gameState = GameState.Playing;
            }
            else if (action == PauseAction.MainMenu)
            {
                _gameState = GameState.MainMenu;
            }
        }

        private void UpdateLevelComplete(float deltaTime)
        {
            if (_levelCompleteScreen.Update(deltaTime))
            {
                // Proceed to next level or game complete
                string nextLevel = _progression.GetNextLevelId(_progression.CurrentLevelId);

                if (!string.IsNullOrEmpty(nextLevel) && _progression.IsLevelUnlocked(nextLevel))
                {
                    _deathCount = 0;
                    LoadLevel(nextLevel);
                    _player.Respawn(_spawnPosition);
                    _respawnAlpha = 1f;
                    _gameState = GameState.Playing;
                }
                else
                {
                    _gameState = GameState.GameComplete;
                }
            }
        }

        private void CompleteLevel()
        {
            _progression.CompleteLevel(_progression.CurrentLevelId);
            _levelCompleteScreen.Start();
            _gameState = GameState.LevelComplete;
        }

        private void HandleSpecialTraps()
        {
            foreach (var trap in _traps)
            {
                // Teleport trap
                if (trap is TeleportTrap teleport && teleport.ConsumeTeleport())
                {
                    _player.Position = teleport.Destination;
                    _player.StopVertical();
                }

                // Fake exit trap
                if (trap is FakeExit fakeExit && fakeExit.ConsumeTrigger())
                {
                    _player.Position = fakeExit.TeleportDestination;
                    _player.StopVertical();
                }

                // Bait object (triggers falling block or spikes)
                if (trap is BaitObject bait && bait.ConsumeCollection())
                {
                    // Just collected - trap triggers are already set up in level design
                }
            }
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

            // Apply moving platform carry (if standing on one)
            if (_player.IsGrounded)
            {
                ApplyMovingPlatformCarry(deltaTime);
            }
        }

        private void ApplyMovingPlatformCarry(float deltaTime)
        {
            Rectangle playerFeet = new Rectangle(
                (int)_player.Position.X,
                (int)_player.Position.Y + GameConfig.PlayerHeight + 1,
                GameConfig.PlayerWidth,
                1
            );

            foreach (var trap in _traps)
            {
                if (trap is MovingPlatform movingPlatform && movingPlatform.IsSolid())
                {
                    if (playerFeet.Intersects(movingPlatform.SolidBounds))
                    {
                        _player.Position += movingPlatform.Velocity * deltaTime;
                        break;
                    }
                }
            }
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

            switch (_gameState)
            {
                case GameState.MainMenu:
                    _mainMenu.Draw(_spriteBatch, _placeholderPixel);
                    break;

                case GameState.LevelSelect:
                    _levelSelectMenu.Draw(_spriteBatch, _placeholderPixel);
                    break;

                case GameState.Playing:
                    DrawLevel();
                    DrawTraps();
                    DrawPlayer();
                    DrawDeathCounter();
                    break;

                case GameState.Paused:
                    DrawLevel();
                    DrawTraps();
                    DrawPlayer();
                    DrawDeathCounter();
                    _pauseMenu.Draw(_spriteBatch, _placeholderPixel);
                    break;

                case GameState.LevelComplete:
                    DrawLevel();
                    DrawTraps();
                    DrawPlayer();
                    string nextLevel = _progression.GetNextLevelId(_progression.CurrentLevelId);
                    bool isLastLevel = string.IsNullOrEmpty(nextLevel) || !_progression.IsLevelUnlocked(nextLevel);
                    _levelCompleteScreen.Draw(_spriteBatch, _placeholderPixel, _currentLevel.Name, _deathCount, isLastLevel);
                    break;

                case GameState.GameComplete:
                    UIHelper.DrawCenteredText(_spriteBatch, _placeholderPixel, "CONGRATULATIONS!", 180, new Color(220, 50, 80), 2);
                    UIHelper.DrawCenteredText(_spriteBatch, _placeholderPixel, "YOU COMPLETED ALL LEVELS", 240, new Color(200, 200, 200), 1);
                    UIHelper.DrawCenteredText(_spriteBatch, _placeholderPixel, "PRESS SPACE TO RETURN", 320, new Color(150, 150, 150), 1);
                    break;
            }

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
            if (_currentLevel == null)
                return;

            // Draw platforms
            foreach (var platform in _currentLevel.Platforms)
            {
                Rectangle rect = new Rectangle(platform.X, platform.Y, platform.Width, platform.Height);
                Color platformColor = new Color(100, 80, 70);

                // Different colors for walls vs floors
                if (platform.Width < 50)
                    platformColor = new Color(40, 25, 50); // Walls
                else if (platform.Height > 50)
                    platformColor = new Color(60, 40, 70); // Floor

                _spriteBatch.Draw(_placeholderPixel, rect, platformColor);
            }

            // Draw hazard zones (semi-transparent red)
            foreach (var hazard in _currentLevel.Hazards)
            {
                Rectangle rect = new Rectangle(hazard.X, hazard.Y, hazard.Width, hazard.Height);
                _spriteBatch.Draw(_placeholderPixel, rect, new Color(220, 50, 80, 80));
            }

            // Draw exit (bright crimson)
            _spriteBatch.Draw(_placeholderPixel, _exitBounds, new Color(220, 50, 80));

            // Exit inner frame
            Rectangle exitFrame = new Rectangle(
                _exitBounds.X + 8,
                _exitBounds.Y + 8,
                _exitBounds.Width - 16,
                _exitBounds.Height - 16
            );
            _spriteBatch.Draw(_placeholderPixel, exitFrame, new Color(40, 20, 30));
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
