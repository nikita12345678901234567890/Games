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

        Button[] GameButtons;

        Guid[] gameIDs;

        Guid selectedGameID;

        Random random = new Random(-103);

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


            GameButtons = new Button[10];

            var color = Color.Pink;
            int xScale = 5;
            int yScale = 10;

            GameButtons[0] = new Button("Game\n   1", ButtonTexture, ClickedButtonTexture, new Vector2(150, 400), new Vector2(xScale, yScale), new Vector2(0, 0), color, font2);
            GameButtons[1] = new Button("Game\n   2", ButtonTexture, ClickedButtonTexture, new Vector2(250, 400), new Vector2(xScale, yScale), new Vector2(0, 0), color, font2);
            GameButtons[2] = new Button("Game\n   3", ButtonTexture, ClickedButtonTexture, new Vector2(350, 400), new Vector2(xScale, yScale), new Vector2(0, 0), color, font2);
            GameButtons[3] = new Button("Game\n   4", ButtonTexture, ClickedButtonTexture, new Vector2(450, 400), new Vector2(xScale, yScale), new Vector2(0, 0), color, font2);
            GameButtons[4] = new Button("Game\n   5", ButtonTexture, ClickedButtonTexture, new Vector2(550, 400), new Vector2(xScale, yScale), new Vector2(0, 0), color, font2);

            GameButtons[5] = new Button("Game\n   6", ButtonTexture, ClickedButtonTexture, new Vector2(150, 500), new Vector2(xScale, yScale), new Vector2(0, 0), color, font2);
            GameButtons[6] = new Button("Game\n   7", ButtonTexture, ClickedButtonTexture, new Vector2(250, 500), new Vector2(xScale, yScale), new Vector2(0, 0), color, font2);
            GameButtons[7] = new Button("Game\n   8", ButtonTexture, ClickedButtonTexture, new Vector2(350, 500), new Vector2(xScale, yScale), new Vector2(0, 0), color, font2);
            GameButtons[8] = new Button("Game\n   9", ButtonTexture, ClickedButtonTexture, new Vector2(450, 500), new Vector2(xScale, yScale), new Vector2(0, 0), color, font2);
            GameButtons[9] = new Button("Game\n  10", ButtonTexture, ClickedButtonTexture, new Vector2(550, 500), new Vector2(xScale, yScale), new Vector2(0, 0), color, font2);

            selectedGameID = Guid.Empty;

            gameIDs = new Guid[GameButtons.Length];

            for (int i = 0; i < gameIDs.Length; i++)
            {
                //gameIDs[i] = Guid.;
            }
        }

        public (bool moveOn, bool spectating, bool playingWhite, Guid gameID) Update(GameTime gameTime) //returns true if menu exited
        {
            for (int i = 0; i < GameButtons.Length; i++)
            {
                if (GameButtons[i].isClicked(InputManager.MouseState))
                {
                    for (int j = 0; j < GameButtons.Length; j++)
                    {
                        GameButtons[j].texture = GameButtons[i].normalTexture;
                    }
                    GameButtons[i].texture = GameButtons[i].clickedTexture;
                }
            }

            if (selectedGameID != Guid.Empty)
            {
                if (WhiteButton.isClicked(InputManager.MouseState)) return (true, false, true, selectedGameID);

                if (BlackButton.isClicked(InputManager.MouseState)) return (true, false, false, selectedGameID);

                if (SpectateButton.isClicked(InputManager.MouseState)) return (true, true, false, selectedGameID);
            }

            return (false, false, false, Guid.Empty);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            WhiteButton.Draw(spriteBatch);
            BlackButton.Draw(spriteBatch);
            SpectateButton.Draw(spriteBatch);

            for (int i = 0; i < GameButtons.Length; i++)
            {
                GameButtons[i].Draw(spriteBatch);
            }
        }
    }
}