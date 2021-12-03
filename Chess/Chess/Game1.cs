using SharedLibrary.Pieces;
using SharedLibrary;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;

using System.Collections.Generic;
using System.Text.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Reflection;
using System.Threading;
using Chess.HackyStuff;

namespace Chess
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;





        public MenuScreen menu;

        public MouseState Lastms;



        bool inMenu = true;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = false;
        }

        /*
        Make game screens
        Have one sidebar with smaller pieces
        Make the game scream
        Make settings screen
        */

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 800;
            graphics.ApplyChanges();


            menu = new MenuScreen(Content, graphics);

            //This generates a list of all exceptions:
            var exceptions = Assembly.GetAssembly(typeof(int)).GetTypes().
                             Where(x => x.IsSubclassOf(typeof(Exception))).ToArray();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);




        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.MouseState = Mouse.GetState();

            if (inMenu)
            {
                var result = menu.Update(gameTime, IsActive);
                if (result.moveOn)
                {
                    if (result.newGame)
                    {
                        if (result.spectating)
                        {
                            System.Windows.Forms.MessageBox.Show("You can't do that.", "", System.Windows.Forms.MessageBoxButtons.OK);
                        }
                        else
                        {
                            gameID = Guid.NewGuid();

                            GameIDNotifierForm.Instance.SetGameID(gameID);
                            if (!GameIDNotifierForm.Instance.Visible)
                            {
                                GameIDNotifierForm.Instance.Show();
                            }
                        }
                    }
                    else
                    {
                        GameIDEntryForm.Instance.ShowDialog();
                    }

                    playerID = Task.Run(async () => await ApiCalls.GetPlayerId(gameID)).Result;

                    if (result.spectating)
                    {
                        inMenu = false;
                        spectating = true;

                        Task.Run(async () => await GetGameState()).Wait();
                    }
                    else
                    {
                        bool? color = Task.Run(async () => await ApiCalls.GetGameColor(gameID, playerID, result.playingWhite)).Result;
                        if (color == null)
                        {
                            var BoxResult = System.Windows.Forms.MessageBox.Show("The game is full", "Press ok the spectate and cancel to go back.", System.Windows.Forms.MessageBoxButtons.OKCancel);

                            switch (BoxResult)
                            {
                                case System.Windows.Forms.DialogResult.OK:
                                    inMenu = false;
                                    spectating = true;
                                    break;

                                case System.Windows.Forms.DialogResult.Cancel:
                                    inMenu = true;
                                    spectating = false;
                                    break;
                            }
                        }
                        else
                        {
                            amWhite = (bool)color;

                            Task.Run(async () => await GetGameState()).Wait();

                            inMenu = false;
                        }
                    }
                }
            }


            InputManager.LastMouseState = InputManager.MouseState;
            base.Update(gameTime);
        }





        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}