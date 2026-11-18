using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SKILL_ISSUE.UI
{
    public class MainMenu
    {
        private int _selectedOption;
        private const int OptionCount = 2;

        public MainMenu()
        {
            _selectedOption = 0;
        }

        public void Update(bool upPressed, bool downPressed, bool selectPressed, out MenuAction action)
        {
            action = MenuAction.None;

            if (upPressed && _selectedOption > 0)
                _selectedOption--;

            if (downPressed && _selectedOption < OptionCount - 1)
                _selectedOption++;

            if (selectPressed)
            {
                action = _selectedOption switch
                {
                    0 => MenuAction.StartGame,
                    1 => MenuAction.Exit,
                    _ => MenuAction.None
                };
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
        {
            // Title
            UIHelper.DrawCenteredText(spriteBatch, pixel, "SKILL ISSUE", 120, new Color(220, 50, 80), 3);

            // Subtitle
            UIHelper.DrawCenteredText(spriteBatch, pixel, "IT'S NOT A BUG", 180, new Color(200, 200, 200), 1);
            UIHelper.DrawCenteredText(spriteBatch, pixel, "IT'S YOUR SKILL ISSUE", 200, new Color(200, 200, 200), 1);

            // Menu buttons
            Rectangle playButton = new Rectangle(330, 280, 300, 40);
            Rectangle exitButton = new Rectangle(330, 340, 300, 40);

            UIHelper.DrawButton(spriteBatch, pixel, playButton, "PLAY", _selectedOption == 0,
                new Color(60, 40, 70), new Color(220, 50, 80));

            UIHelper.DrawButton(spriteBatch, pixel, exitButton, "EXIT", _selectedOption == 1,
                new Color(60, 40, 70), new Color(220, 50, 80));

            // Controls hint
            UIHelper.DrawCenteredText(spriteBatch, pixel, "W/S TO NAVIGATE - SPACE TO SELECT", 460, new Color(150, 150, 150), 1);
        }
    }

    public enum MenuAction
    {
        None,
        StartGame,
        Exit
    }
}
