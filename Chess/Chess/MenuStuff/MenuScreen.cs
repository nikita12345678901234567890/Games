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
        SpriteFont font2;

        Texture2D ButtonTexture;
        Texture2D ClickedButtonTexture;
        Button WhiteButton;
        Button BlackButton;
        Button SpectateButton;
        Button NewGameButton;

        bool newGame = false;

        public MenuScreen(ContentManager content, GraphicsDeviceManager graphics)
        {
            this.content = content;
            this.graphics = graphics;

            font = content.Load<SpriteFont>("SpriteFont");
            font2 = content.Load<SpriteFont>("font2");
            ButtonTexture = content.Load<Texture2D>("Button");
            ClickedButtonTexture = content.Load<Texture2D>("ButtonClicked");

            WhiteButton = new Button("Play\nWhite", ButtonTexture, ClickedButtonTexture, new Vector2(200, 100), new Vector2(10, 20), new Vector2(0, 0), Color.White, font);
            BlackButton = new Button("Play\nBlack", ButtonTexture, ClickedButtonTexture, new Vector2(400, 100), new Vector2(10, 20), new Vector2(0, 0), Color.White, font);
            SpectateButton = new Button("Spectate", ButtonTexture, ClickedButtonTexture, new Vector2(300, 300), new Vector2(10, 5), new Vector2(0, 0), Color.White, font);

            NewGameButton = new Button("Start\nnew\ngame", ButtonTexture, ClickedButtonTexture, new Vector2(300, 500), new Vector2(10, 15), new Vector2(0, 0), Color.White, font);
        }

        public (bool moveOn, bool spectating, bool playingWhite, bool newGame) Update(GameTime gameTime)
        {
            if (NewGameButton.isClicked(InputManager.MouseState, InputManager.LastMouseState))
            {
                if (NewGameButton.texture == NewGameButton.normalTexture)
                {
                    NewGameButton.texture = NewGameButton.clickedTexture;
                    newGame = true;
                }
                else
                {
                    NewGameButton.texture = NewGameButton.normalTexture;
                    newGame = false;
                }
            }


            if (WhiteButton.isClicked(InputManager.MouseState)) return (true, false, true, newGame);

            if (BlackButton.isClicked(InputManager.MouseState)) return (true, false, false, newGame);

            if (SpectateButton.isClicked(InputManager.MouseState)) return (true, true, false, newGame);

            return (false, false, false, newGame);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            WhiteButton.Draw(spriteBatch);
            BlackButton.Draw(spriteBatch);
            SpectateButton.Draw(spriteBatch);

            NewGameButton.Draw(spriteBatch);
        }
    }
}