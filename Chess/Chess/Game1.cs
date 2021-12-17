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
using Chess.ScreenStuff;

namespace Chess
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public MenuScreen menu;

        public MouseState Lastms;

        public ScreenManager screens;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = false;
        }

        /*
        Have one sidebar with smaller pieces
        Make the game scream
        Make settings screen
        */

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 800;
            graphics.ApplyChanges();


            screens = new ScreenManager(Content, graphics);

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

            screens.Update(gameTime, IsActive);


            InputManager.LastMouseState = InputManager.MouseState;
            base.Update(gameTime);
        }





        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Chocolate);

            spriteBatch.Begin();

            screens.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}