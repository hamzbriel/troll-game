using Microsoft.Xna.Framework.Input;

namespace SKILL_ISSUE.Input
{
    /// <summary>
    /// Manages keyboard input state and provides edge-detection helpers.
    /// Tracks current and previous frame states to detect key press/release events.
    /// </summary>
    public class InputManager
    {
        private KeyboardState _currentState;
        private KeyboardState _previousState;

        public InputManager()
        {
            _currentState = Keyboard.GetState();
            _previousState = _currentState;
        }

        /// <summary>
        /// Update input state. Call once per frame before reading input.
        /// </summary>
        public void Update()
        {
            _previousState = _currentState;
            _currentState = Keyboard.GetState();
        }

        /// <summary>
        /// Check if key is currently held down.
        /// </summary>
        public bool IsDown(Keys key)
        {
            return _currentState.IsKeyDown(key);
        }

        /// <summary>
        /// Check if key is currently up (not pressed).
        /// </summary>
        public bool IsUp(Keys key)
        {
            return _currentState.IsKeyUp(key);
        }

        /// <summary>
        /// Check if key was just pressed this frame (down now, up last frame).
        /// Use for edge-triggered actions like jump.
        /// </summary>
        public bool WasPressed(Keys key)
        {
            return _currentState.IsKeyDown(key) && _previousState.IsKeyUp(key);
        }

        /// <summary>
        /// Check if key was just released this frame (up now, down last frame).
        /// </summary>
        public bool WasReleased(Keys key)
        {
            return _currentState.IsKeyUp(key) && _previousState.IsKeyDown(key);
        }

        /// <summary>
        /// Get horizontal movement input (-1 left, 0 none, +1 right).
        /// Checks both WASD and arrow keys.
        /// </summary>
        public float GetHorizontalInput()
        {
            float input = 0f;

            if (IsDown(Keys.A) || IsDown(Keys.Left))
                input -= 1f;

            if (IsDown(Keys.D) || IsDown(Keys.Right))
                input += 1f;

            return input;
        }

        /// <summary>
        /// Check if jump was requested (W, Space, or Up Arrow pressed this frame).
        /// </summary>
        public bool JumpPressed()
        {
            return WasPressed(Keys.W) || WasPressed(Keys.Space) || WasPressed(Keys.Up);
        }

        /// <summary>
        /// Check if restart was requested (R pressed).
        /// </summary>
        public bool RestartPressed()
        {
            return WasPressed(Keys.R);
        }

        /// <summary>
        /// Check if pause was requested (Escape pressed).
        /// </summary>
        public bool PausePressed()
        {
            return WasPressed(Keys.Escape);
        }
    }
}
