using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SKILL_ISSUE.UI
{
    public static class UIHelper
    {
        public static void DrawText(SpriteBatch spriteBatch, Texture2D pixel, string text, int x, int y, Color color, int scale = 1)
        {
            int cursorX = x;
            foreach (char c in text.ToUpper())
            {
                DrawChar(spriteBatch, pixel, c, cursorX, y, color, scale);
                cursorX += (4 * scale);
            }
        }

        public static void DrawCenteredText(SpriteBatch spriteBatch, Texture2D pixel, string text, int y, Color color, int scale = 1, int screenWidth = 960)
        {
            int textWidth = text.Length * 4 * scale;
            int x = (screenWidth - textWidth) / 2;
            DrawText(spriteBatch, pixel, text, x, y, color, scale);
        }

        public static void DrawButton(SpriteBatch spriteBatch, Texture2D pixel, Rectangle bounds, string text, bool selected, Color normalColor, Color selectedColor)
        {
            Color bgColor = selected ? selectedColor : normalColor;
            Color textColor = selected ? Color.White : new Color(200, 200, 200);

            spriteBatch.Draw(pixel, bounds, bgColor);

            Rectangle border = new Rectangle(bounds.X + 2, bounds.Y + 2, bounds.Width - 4, bounds.Height - 4);
            spriteBatch.Draw(pixel, border, new Color(20, 12, 28));

            int textWidth = text.Length * 4;
            int textX = bounds.X + (bounds.Width - textWidth) / 2;
            int textY = bounds.Y + (bounds.Height - 5) / 2;

            DrawText(spriteBatch, pixel, text, textX, textY, textColor);
        }

        private static void DrawChar(SpriteBatch spriteBatch, Texture2D pixel, char c, int x, int y, Color color, int scale)
        {
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
                'P' => new bool[,] { {true,true,false}, {true,false,true}, {true,true,false}, {true,false,false}, {true,false,false} },
                'R' => new bool[,] { {true,true,false}, {true,false,true}, {true,true,false}, {true,false,true}, {true,false,true} },
                'S' => new bool[,] { {false,true,true}, {true,false,false}, {false,true,false}, {false,false,true}, {true,true,false} },
                'T' => new bool[,] { {true,true,true}, {false,true,false}, {false,true,false}, {false,true,false}, {false,true,false} },
                'U' => new bool[,] { {true,false,true}, {true,false,true}, {true,false,true}, {true,false,true}, {false,true,false} },
                'V' => new bool[,] { {true,false,true}, {true,false,true}, {true,false,true}, {true,false,true}, {false,true,false} },
                'W' => new bool[,] { {true,false,true}, {true,false,true}, {true,true,true}, {true,true,true}, {true,false,true} },
                'Y' => new bool[,] { {true,false,true}, {true,false,true}, {false,true,false}, {false,true,false}, {false,true,false} },
                '0' => new bool[,] { {true,true,true}, {true,false,true}, {true,false,true}, {true,false,true}, {true,true,true} },
                '1' => new bool[,] { {false,true,false}, {true,true,false}, {false,true,false}, {false,true,false}, {true,true,true} },
                '2' => new bool[,] { {true,true,true}, {false,false,true}, {true,true,true}, {true,false,false}, {true,true,true} },
                '3' => new bool[,] { {true,true,true}, {false,false,true}, {true,true,true}, {false,false,true}, {true,true,true} },
                '4' => new bool[,] { {true,false,true}, {true,false,true}, {true,true,true}, {false,false,true}, {false,false,true} },
                '5' => new bool[,] { {true,true,true}, {true,false,false}, {true,true,true}, {false,false,true}, {true,true,true} },
                '-' => new bool[,] { {false,false,false}, {false,false,false}, {true,true,true}, {false,false,false}, {false,false,false} },
                ':' => new bool[,] { {false,false,false}, {false,true,false}, {false,false,false}, {false,true,false}, {false,false,false} },
                ' ' => new bool[,] { {false,false,false}, {false,false,false}, {false,false,false}, {false,false,false}, {false,false,false} },
                _ => new bool[,] { {false,false,false}, {false,false,false}, {false,true,false}, {false,false,false}, {false,false,false} }
            };

            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    if (pixels[row, col])
                    {
                        Rectangle pixelRect = new Rectangle(
                            x + (col * scale),
                            y + (row * scale),
                            scale,
                            scale
                        );
                        spriteBatch.Draw(pixel, pixelRect, color);
                    }
                }
            }
        }
    }
}
