using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Text;

namespace Chess
{
    public class Button : Sprite
    {
        public string text;
        SpriteFont font;
        public Texture2D normalTexture;
        public Texture2D clickedTexture;

        public Button(string text, Texture2D normalTexture, Texture2D clickedTexture, Vector2 position, Vector2 scale, Vector2 origin, Color color, SpriteFont font)
            :base(normalTexture, position, scale, origin, color)
        {
            this.text = text;
            this.font = font;

            this.normalTexture = normalTexture;
            this.clickedTexture = clickedTexture;
        }

        public bool isClicked(MouseState ms)
        {
            if (HitBox.Contains(ms.Position) && ms.LeftButton == ButtonState.Pressed)
            {
                return true;
            }
            return false;
        }

        public bool isClicked(MouseState ms, MouseState prevState)
        {
            if (HitBox.Contains(ms.Position) && ms.LeftButton == ButtonState.Pressed && prevState.LeftButton == ButtonState.Released)
            {
                return true;
            }
            return false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            var textSize = font.MeasureString(text);

            //logic for centering text:
            Vector2 pos = new Vector2(Position.X + ((texture.Width * scale.X) / 2) - (textSize.X / 2), Position.Y + ((texture.Height * scale.Y) / 2) - (textSize.Y / 2));

            spriteBatch.DrawString(font, text, pos, Color.Black);

        }
    }
}
