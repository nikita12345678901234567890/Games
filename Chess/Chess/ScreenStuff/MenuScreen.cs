using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Text;

namespace Chess
{
    public class MenuScreen : Screen
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

        public override UpdateResult Update(GameTime gameTime, bool IsActive)
        {
            UpdateResult result;
            if (IsActive)
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


                if (WhiteButton.isClicked(InputManager.MouseState))
                {
                    result.moveOn = true;
                    result.false = true;
                    result.spectating = false;
                    result.playingWhite = true;
                    result.newGame = newGame;

                    return result;
                }

                if (BlackButton.isClicked(InputManager.MouseState))
                {
                    result.moveOn = true;
                    result.spectating = false;
                    result.playingWhite = false;
                    result.newGame = newGame;

                    return result;
                }

                if (SpectateButton.isClicked(InputManager.MouseState))
                {
                    result.moveOn = true;
                    result.spectating = true;
                    result.playingWhite = false;
                    result.newGame = newGame;

                    return result;
                }
            }

            result.moveOn = false;
            result.spectating = false;
            result.playingWhite = false;
            result.newGame = newGame;

            return result;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            WhiteButton.Draw(spriteBatch);
            BlackButton.Draw(spriteBatch);
            SpectateButton.Draw(spriteBatch);

            NewGameButton.Draw(spriteBatch);
        }
    }
}