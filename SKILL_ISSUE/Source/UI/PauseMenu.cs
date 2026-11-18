using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SKILL_ISSUE.UI
{
    public class PauseMenu
    {
        private int _selectedOption;
        private const int OptionCount = 3;

        public PauseMenu()
        {
            _selectedOption = 0;
        }

        public void Update(bool upPressed, bool downPressed, bool selectPressed, out PauseAction action)
        {
            action = PauseAction.None;

            if (upPressed && _selectedOption > 0)
                _selectedOption--;

            if (downPressed && _selectedOption < OptionCount - 1)
                _selectedOption++;

            if (selectPressed)
            {
                action = _selectedOption switch
                {
                    0 => PauseAction.Resume,
                    1 => PauseAction.Restart,
                    2 => PauseAction.MainMenu,
                    _ => PauseAction.None
                };
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
        {
            // Dark overlay
            spriteBatch.Draw(pixel, new Rectangle(0, 0, 960, 540), new Color(0, 0, 0, 180));

            // Pause title
            UIHelper.DrawCenteredText(spriteBatch, pixel, "PAUSED", 140, new Color(220, 50, 80), 2);

            // Menu buttons
            Rectangle resumeButton = new Rectangle(330, 220, 300, 40);
            Rectangle restartButton = new Rectangle(330, 280, 300, 40);
            Rectangle mainMenuButton = new Rectangle(330, 340, 300, 40);

            UIHelper.DrawButton(spriteBatch, pixel, resumeButton, "RESUME", _selectedOption == 0,
                new Color(60, 40, 70), new Color(220, 50, 80));

            UIHelper.DrawButton(spriteBatch, pixel, restartButton, "RESTART", _selectedOption == 1,
                new Color(60, 40, 70), new Color(220, 50, 80));

            UIHelper.DrawButton(spriteBatch, pixel, mainMenuButton, "MAIN MENU", _selectedOption == 2,
                new Color(60, 40, 70), new Color(220, 50, 80));

            // Controls hint
            UIHelper.DrawCenteredText(spriteBatch, pixel, "W/S - SPACE - ESC TO RESUME", 460, new Color(150, 150, 150), 1);
        }

        public void Reset()
        {
            _selectedOption = 0;
        }
    }

    public enum PauseAction
    {
        None,
        Resume,
        Restart,
        MainMenu
    }
}
