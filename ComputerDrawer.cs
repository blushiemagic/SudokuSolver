using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sudoku
{
    public class ComputerDrawer : Drawer
    {
        private Game game;
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private SpriteFont bigFont;

        public ComputerDrawer(Game game, SpriteBatch spriteBatch)
        {
            this.game = game;
            this.spriteBatch = spriteBatch;
        }

        public override object LoadImage(string path)
        {
            return game.Content.Load<Texture2D>(path);
        }

        public override void LoadExtraContent()
        {
            font = game.Content.Load<SpriteFont>("Font");
            bigFont = game.Content.Load<SpriteFont>("BigFont");
        }

        public override void Clear(Color color)
        {
            game.GraphicsDevice.Clear(ToXnaColor(color));
        }

        public override void BeginDraw()
        {
            spriteBatch.Begin();
        }

        public override void Draw(object image, float x, float y, Color color, float alpha, float scale)
        {
            spriteBatch.Draw((Texture2D)image, new Vector2(x, y), null, ToXnaColor(color) * alpha,
                0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public override void EndDraw()
        {
            spriteBatch.End();
        }

        public override void DrawTextBox(float x, float y, float width, float height, float alpha = 1f)
        {
            Microsoft.Xna.Framework.Color color = Microsoft.Xna.Framework.Color.White * alpha;
            spriteBatch.Draw((Texture2D)textBoxTopLeft, new Vector2(x - 2f, y - 2f), color);
            spriteBatch.Draw((Texture2D)textBoxTopRight, new Vector2(x + width, y - 2f), color);
            spriteBatch.Draw((Texture2D)textBoxBottomRight, new Vector2(x + width, y + height), color);
            spriteBatch.Draw((Texture2D)textBoxBottomLeft, new Vector2(x - 2f, y + height), color);
            spriteBatch.Draw((Texture2D)textBoxTopBottom, new Rectangle((int)x, (int)(y - 2f), (int)width, 2), color);
            spriteBatch.Draw((Texture2D)textBoxTopBottom, new Rectangle((int)x, (int)(y + height), (int)width, 2), color);
            spriteBatch.Draw((Texture2D)textBoxLeftRight, new Rectangle((int)(x - 2f), (int)y, 2, (int)height), color);
            spriteBatch.Draw((Texture2D)textBoxLeftRight, new Rectangle((int)(x + width), (int)y, 2, (int)height), color);
            spriteBatch.Draw((Texture2D)textBoxCenter, new Rectangle((int)x, (int)y, (int)width, (int)height), color);
        }

        public override void DrawText(float x, float y, string text, Color color, float alpha = 1f, float scale = 1f)
        {
            spriteBatch.DrawString(font, text, new Vector2(x, y), ToXnaColor(color) * alpha,
                0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public override void DrawBigText(float x, float y, string text, Color color, float alpha = 1)
        {
            spriteBatch.DrawString(bigFont, text, new Vector2(x, y), ToXnaColor(color) * alpha);
        }

        public override float MeasureTextWidth(string text)
        {
            return font.MeasureString(text).X;
        }

        public override float MeasureTextHeight(string text)
        {
            return font.MeasureString(text).Y;
        }

        public override float MeasureBigTextWidth(string text)
        {
            return bigFont.MeasureString(text).X;
        }

        public override float MeasureBigTextHeight(string text)
        {
            return bigFont.MeasureString(text).Y;
        }

        private static Microsoft.Xna.Framework.Color ToXnaColor(Color color)
        {
            return new Microsoft.Xna.Framework.Color(color.R, color.G, color.B);
        }
    }
}
