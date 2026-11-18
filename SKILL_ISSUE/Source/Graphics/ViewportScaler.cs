using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SKILL_ISSUE.Graphics
{
    /// <summary>
    /// Handles aspect-ratio-preserving viewport scaling.
    /// Maps logical 960×540 game canvas to physical window with letterboxing/pillarboxing.
    /// </summary>
    public class ViewportScaler
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly int _logicalWidth;
        private readonly int _logicalHeight;

        public ViewportScaler(GraphicsDevice graphicsDevice, int logicalWidth, int logicalHeight)
        {
            _graphicsDevice = graphicsDevice;
            _logicalWidth = logicalWidth;
            _logicalHeight = logicalHeight;
        }

        /// <summary>
        /// Calculate viewport rectangle that preserves 16:9 aspect ratio.
        /// Returns destination rectangle for rendering the logical canvas.
        /// </summary>
        public Rectangle GetScaledViewport()
        {
            var backbufferWidth = _graphicsDevice.PresentationParameters.BackBufferWidth;
            var backbufferHeight = _graphicsDevice.PresentationParameters.BackBufferHeight;

            float targetAspect = (float)_logicalWidth / _logicalHeight;
            float windowAspect = (float)backbufferWidth / backbufferHeight;

            int viewportWidth, viewportHeight;
            int viewportX, viewportY;

            if (windowAspect > targetAspect)
            {
                // Window wider than target: pillarbox (black bars on sides)
                viewportHeight = backbufferHeight;
                viewportWidth = (int)(backbufferHeight * targetAspect);
                viewportX = (backbufferWidth - viewportWidth) / 2;
                viewportY = 0;
            }
            else
            {
                // Window taller than target: letterbox (black bars top/bottom)
                viewportWidth = backbufferWidth;
                viewportHeight = (int)(backbufferWidth / targetAspect);
                viewportX = 0;
                viewportY = (backbufferHeight - viewportHeight) / 2;
            }

            return new Rectangle(viewportX, viewportY, viewportWidth, viewportHeight);
        }
    }
}
