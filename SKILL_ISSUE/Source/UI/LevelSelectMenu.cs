using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SKILL_ISSUE.Level;

namespace SKILL_ISSUE.UI
{
    public class LevelSelectMenu
    {
        private int _selectedWorld;
        private int _selectedLevel;
        private const int WorldCount = 5;
        private const int LevelCount = 3;
        private WorldProgressionManager _progression;

        public LevelSelectMenu(WorldProgressionManager progression)
        {
            _progression = progression;
            _selectedWorld = 1;
            _selectedLevel = 1;
        }

        public void Update(bool upPressed, bool downPressed, bool leftPressed, bool rightPressed, bool selectPressed, bool backPressed, out LevelSelectAction action, out string selectedLevelId)
        {
            action = LevelSelectAction.None;
            selectedLevelId = "";

            if (leftPressed && _selectedWorld > 1)
                _selectedWorld--;

            if (rightPressed && _selectedWorld < WorldCount)
                _selectedWorld++;

            if (upPressed && _selectedLevel > 1)
                _selectedLevel--;

            if (downPressed && _selectedLevel < LevelCount)
                _selectedLevel++;

            if (backPressed)
            {
                action = LevelSelectAction.Back;
                return;
            }

            if (selectPressed)
            {
                selectedLevelId = $"W{_selectedWorld}_L{_selectedLevel}";
                if (_progression.IsLevelUnlocked(selectedLevelId))
                {
                    action = LevelSelectAction.SelectLevel;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
        {
            // Title
            UIHelper.DrawCenteredText(spriteBatch, pixel, "SELECT LEVEL", 60, new Color(220, 50, 80), 2);

            // World selector
            int worldY = 140;
            for (int w = 1; w <= WorldCount; w++)
            {
                int worldX = 200 + (w - 1) * 120;
                Rectangle worldButton = new Rectangle(worldX, worldY, 100, 40);
                bool selected = (w == _selectedWorld);

                UIHelper.DrawButton(spriteBatch, pixel, worldButton, $"WORLD {w}", selected,
                    new Color(60, 40, 70), new Color(220, 50, 80));
            }

            // Level buttons for selected world
            int levelY = 240;
            for (int l = 1; l <= LevelCount; l++)
            {
                string levelId = $"W{_selectedWorld}_L{l}";
                bool unlocked = _progression.IsLevelUnlocked(levelId);
                bool selected = (l == _selectedLevel);

                int levelX = 280 + (l - 1) * 140;
                Rectangle levelButton = new Rectangle(levelX, levelY, 120, 50);

                Color bgColor = unlocked
                    ? (selected ? new Color(220, 50, 80) : new Color(60, 40, 70))
                    : new Color(30, 20, 35);

                Color textColor = unlocked
                    ? (selected ? Color.White : new Color(200, 200, 200))
                    : new Color(80, 60, 70);

                spriteBatch.Draw(pixel, levelButton, bgColor);

                Rectangle border = new Rectangle(levelButton.X + 2, levelButton.Y + 2, levelButton.Width - 4, levelButton.Height - 4);
                spriteBatch.Draw(pixel, border, new Color(20, 12, 28));

                string levelText = unlocked ? $"LEVEL {l}" : "LOCKED";
                int textWidth = levelText.Length * 4;
                int textX = levelButton.X + (levelButton.Width - textWidth) / 2;
                int textY = levelButton.Y + (levelButton.Height - 5) / 2;

                UIHelper.DrawText(spriteBatch, pixel, levelText, textX, textY, textColor);
            }

            // Stats for selected level
            string selectedId = $"W{_selectedWorld}_L{_selectedLevel}";
            if (_progression.IsLevelUnlocked(selectedId))
            {
                UIHelper.DrawCenteredText(spriteBatch, pixel, selectedId, 360, new Color(200, 200, 200), 1);
            }
            else
            {
                UIHelper.DrawCenteredText(spriteBatch, pixel, "COMPLETE PREVIOUS LEVELS TO UNLOCK", 360, new Color(150, 100, 100), 1);
            }

            // Controls
            UIHelper.DrawCenteredText(spriteBatch, pixel, "ARROWS TO NAVIGATE - SPACE TO SELECT - ESC TO BACK", 460, new Color(150, 150, 150), 1);
        }
    }

    public enum LevelSelectAction
    {
        None,
        SelectLevel,
        Back
    }
}
