using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Text;

namespace Chess
{
    public class MenuScreen
    {
        ContentManager content;
        GraphicsDeviceManager graphics;

        SpriteFont font;

        Button WhiteButton;
        Button BlackButton;
        Button SpectateButton;

        public MenuScreen(ContentManager content, GraphicsDeviceManager graphics)
        {
            this.content = content;
            this.graphics = graphics;

            font = content.Load<SpriteFont>("SpriteFont");
            Texture2D ButtonTexture = content.Load<Texture2D>("Button");

            WhiteButton = new Button("Play\nWhite", ButtonTexture, new Vector2(100, 100), new Vector2(5, 5), new Vector2(0, 0), Color.White, font);
            BlackButton = new Button("Play\nBlack", ButtonTexture, new Vector2(300, 100), new Vector2(5, 5), new Vector2(0, 0), Color.White, font);
            SpectateButton = new Button("Spectate", ButtonTexture, new Vector2(200, 400), new Vector2(5, 5), new Vector2(0, 0), Color.White, font);
        }

        public (bool moveOn, bool spectating, bool playingWhite) Update(GameTime gameTime) //returns true if menu exited
        {
            if (WhiteButton.isClicked(InputManager.MouseState)) return (true, false, true);
            
            if (BlackButton.isClicked(InputManager.MouseState)) return (true, false, false);

            if (SpectateButton.isClicked(InputManager.MouseState)) return (true, true, false);

            return (false, false, false);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            WhiteButton.Draw(spriteBatch);
            BlackButton.Draw(spriteBatch);
            SpectateButton.Draw(spriteBatch);
        }
    }
}