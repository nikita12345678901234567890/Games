using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;

namespace AnimationIntro
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        List<Rectangle> rectangles;
        Sprite fish;
        int fishy = 0;

        TimeSpan Delay = TimeSpan.FromMilliseconds(100);
        TimeSpan Elapsed;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            fish = new Sprite(Content.Load<Texture2D>("FishSheet1"), new Vector2(50, 50), Vector2.Zero, Vector2.One, Color.White);

            rectangles = new List<Rectangle>();

            rectangles.Add(new Rectangle(200, 100, 15, 26));
            rectangles.Add(new Rectangle(233, 99, 14, 27));
            rectangles.Add(new Rectangle(265, 98, 15, 25));
            rectangles.Add(new Rectangle(233, 99, 14, 27));

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if(Elapsed >= Delay)
            {
                fishy++;

                if (fishy >= rectangles.Count)
                {
                    fishy = 0;
                }

                Elapsed = TimeSpan.Zero;
            }

            fish.Update(gameTime);

            Elapsed += gameTime.ElapsedGameTime;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            fish.Source = rectangles[fishy];
            fish.Draw(spriteBatch);
            
            
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
