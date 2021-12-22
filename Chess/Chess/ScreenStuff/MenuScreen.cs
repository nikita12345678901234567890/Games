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
        

        Texture2D ButtonTexture;
        Texture2D ClickedButtonTexture;
        Button WhiteButton;
        Button BlackButton;
        Button SpectateButton;
        Button NewGameButton;
        Button CrazyhouseButton;

        bool newGame = false;
        bool crazyhouse = false;

        public MenuScreen(ContentManager content, GraphicsDeviceManager graphics)
        {
            this.content = content;
            this.graphics = graphics;

            font = content.Load<SpriteFont>("SpriteFont");
            ButtonTexture = content.Load<Texture2D>("Button");
            ClickedButtonTexture = content.Load<Texture2D>("ButtonClicked");

            WhiteButton = new Button("Play\nWhite", ButtonTexture, ClickedButtonTexture, new Vector2(200, 100), new Vector2(10, 20), new Vector2(0, 0), Color.White, font);
            BlackButton = new Button("Play\nBlack", ButtonTexture, ClickedButtonTexture, new Vector2(400, 100), new Vector2(10, 20), new Vector2(0, 0), Color.White, font);
            SpectateButton = new Button("Spectate", ButtonTexture, ClickedButtonTexture, new Vector2(300, 300), new Vector2(10, 5), new Vector2(0, 0), Color.White, font);

            NewGameButton = new Button("Start\nnew\ngame", ButtonTexture, ClickedButtonTexture, new Vector2(200, 400), new Vector2(10, 15), new Vector2(0, 0), Color.White, font);
            CrazyhouseButton = new Button("Crazyhouse", ButtonTexture, ClickedButtonTexture, new Vector2(400, 400), new Vector2(15, 7), new Vector2(0, 0), Color.White, font);
        }

        public override UpdateResult Update(GameTime gameTime, bool IsActive)
        {
            UpdateResult result = default;
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

                if (CrazyhouseButton.isClicked(InputManager.MouseState, InputManager.LastMouseState))
                {
                    if (CrazyhouseButton.texture == CrazyhouseButton.normalTexture)
                    {
                        CrazyhouseButton.texture = CrazyhouseButton.clickedTexture;
                        crazyhouse = true;
                    }
                    else
                    {
                        CrazyhouseButton.texture = CrazyhouseButton.normalTexture;
                        crazyhouse = false;
                    }
                }

                if (WhiteButton.isClicked(InputManager.MouseState))
                {
                    result.startPlaying = true;
                    result.startSpectating = false;
                    result.startPlayingWhite = true;
                    result.startNewGame = newGame;
                    result.startPlayingCrazyhouse = crazyhouse;

                    return result;
                }

                if (BlackButton.isClicked(InputManager.MouseState))
                {
                    result.startPlaying = true;
                    result.startSpectating = false;
                    result.startPlayingWhite = false;
                    result.startNewGame = newGame;
                    result.startPlayingCrazyhouse = crazyhouse;

                    return result;
                }

                if (SpectateButton.isClicked(InputManager.MouseState))
                {
                    result.startPlaying = true;
                    result.startSpectating = true;
                    result.startPlayingWhite = false;
                    result.startNewGame = newGame;
                    result.startPlayingCrazyhouse = crazyhouse;

                    return result;
                }
            }

            return result;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            WhiteButton.Draw(spriteBatch);
            BlackButton.Draw(spriteBatch);
            SpectateButton.Draw(spriteBatch);
            CrazyhouseButton.Draw(spriteBatch);

            NewGameButton.Draw(spriteBatch);
        }
    }
}