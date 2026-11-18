using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SKILL_ISSUE.UI
{
    public class LevelCompleteScreen
    {
        private float _displayTimer;
        private const float DisplayDuration = 2.5f;

        public LevelCompleteScreen()
        {
            _displayTimer = 0f;
        }

        public void Start()
        {
            _displayTimer = 0f;
        }

        public bool Update(float deltaTime)
        {
            _displayTimer += deltaTime;
            return _displayTimer >= DisplayDuration;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D pixel, string levelName, int deathCount, bool isLastLevel)
        {
            // Semi-transparent overlay
            spriteBatch.Draw(pixel, new Rectangle(0, 0, 960, 540), new Color(0, 0, 0, 200));

            // Title
            string title = isLastLevel ? "GAME COMPLETE!" : "LEVEL COMPLETE!";
            UIHelper.DrawCenteredText(spriteBatch, pixel, title, 140, new Color(220, 50, 80), 2);

            // Level name
            UIHelper.DrawCenteredText(spriteBatch, pixel, levelName, 200, new Color(200, 200, 200), 1);

            // Death count
            string deathText = $"DEATHS: {deathCount}";
            UIHelper.DrawCenteredText(spriteBatch, pixel, deathText, 260, new Color(255, 255, 255), 1);

            // Message based on death count
            string message = deathCount switch
            {
                0 => "FLAWLESS!",
                <= 5 => "NOT BAD!",
                <= 10 => "COULD BE WORSE",
                <= 20 => "SKILL ISSUE DETECTED",
                _ => "IMPRESSIVE... IN A BAD WAY"
            };
            UIHelper.DrawCenteredText(spriteBatch, pixel, message, 300, new Color(255, 200, 0), 1);

            // Continue hint
            if (!isLastLevel)
            {
                float alpha = (float)System.Math.Sin(_displayTimer * 4) * 0.3f + 0.7f;
                Color hintColor = new Color((byte)(150 * alpha), (byte)(150 * alpha), (byte)(150 * alpha));
                UIHelper.DrawCenteredText(spriteBatch, pixel, "LOADING NEXT LEVEL...", 400, hintColor, 1);
            }
            else
            {
                UIHelper.DrawCenteredText(spriteBatch, pixel, "THANKS FOR PLAYING!", 400, new Color(150, 150, 150), 1);
            }
        }
    }
}
