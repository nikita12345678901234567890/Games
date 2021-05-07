using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;

namespace FishGame2._0
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;


        public static int LaserSpeed = 10;


        Fish Bob;
        Fish Steve;
        List<Fish> Fishies;
        KeyboardState lastkb;

        TimeSpan FishSpawnTime = TimeSpan.FromMilliseconds(500);
        TimeSpan ElapsedFishTime;

        public List<Rectangle> AiRectangles;

        public static List<Laser> Lasers;

        Random random = new Random();

        public static string Text;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 1000;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Lasers = new List<Laser>();

            spriteBatch = new SpriteBatch(GraphicsDevice);

            List<Rectangle> BobRectangles = new List<Rectangle>();

            BobRectangles.Add(new Rectangle(200, 100, 15, 26));
            BobRectangles.Add(new Rectangle(233, 99, 14, 27));
            BobRectangles.Add(new Rectangle(265, 98, 15, 25));
            BobRectangles.Add(new Rectangle(233, 99, 14, 27));

            List<Rectangle> SteveRectangles = new List<Rectangle>();

            SteveRectangles.Add(new Rectangle(8, 100, 15, 26));
            SteveRectangles.Add(new Rectangle(41, 99, 14, 27));
            SteveRectangles.Add(new Rectangle(73, 98, 15, 25));
            SteveRectangles.Add(new Rectangle(41, 99, 14, 27));

            AiRectangles = new List<Rectangle>();

            AiRectangles.Add(new Rectangle(8, 228, 15, 26));
            AiRectangles.Add(new Rectangle(41, 227, 14, 27));
            AiRectangles.Add(new Rectangle(73, 226, 15, 25));
            AiRectangles.Add(new Rectangle(41, 227, 14, 27));

            Bob = new Fish(BobRectangles.ToArray(), TimeSpan.FromMilliseconds(100), Content.Load<Texture2D>("FishSheet1"), Content.Load<Texture2D>("Laser"), new Vector2(50, 50), new Vector2(7.5f, 13), Vector2.One, Color.White, 0);
            Steve = new Fish(SteveRectangles.ToArray(), TimeSpan.FromMilliseconds(100), Content.Load<Texture2D>("FishSheet1"), Content.Load<Texture2D>("Laser"), new Vector2(500, 500), new Vector2(7.5f, 13), Vector2.One, Color.White, 1);


            Fishies = new List<Fish>();
            for (int i = 0; i < 10; i++)
            {
                Fishies.Add(new Fish(AiRectangles.ToArray(), TimeSpan.FromMilliseconds(100), Content.Load<Texture2D>("FishSheet1"), Content.Load<Texture2D>("Laser"), new Vector2(random.Next(50, graphics.PreferredBackBufferWidth - 50), random.Next(50, graphics.PreferredBackBufferHeight - 50)), new Vector2(7.5f, 13), Vector2.One, Color.White, 5));
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.NumPad0)) LaserSpeed = 10;
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad1)) LaserSpeed = 1;
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad2)) LaserSpeed = 2;
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad3)) LaserSpeed = 3;
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad4)) LaserSpeed = 4;
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad5)) LaserSpeed = 5;
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad6)) LaserSpeed = 6;
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad7)) LaserSpeed = 7;
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad8)) LaserSpeed = 8;
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad9)) LaserSpeed = 9;

            Bob.Update(Keyboard.GetState(), lastkb, gameTime, new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight));

            Steve.Update(Keyboard.GetState(), lastkb, gameTime, new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight));


            if (ElapsedFishTime >= FishSpawnTime)
            {
                ElapsedFishTime = TimeSpan.Zero;

                Fishies.Add(new Fish(AiRectangles.ToArray(), TimeSpan.FromMilliseconds(100), Content.Load<Texture2D>("FishSheet1"), Content.Load<Texture2D>("Laser"), new Vector2(random.Next(50, graphics.PreferredBackBufferWidth - 50), random.Next(50, graphics.PreferredBackBufferHeight - 50)), new Vector2(7.5f, 13), Vector2.One, Color.White, 5));
            }


            for (int i = 0; i < Fishies.Count; i++)
            {
                int value = Fishies[i].Update(Keyboard.GetState(), lastkb, gameTime, new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight));
                if (value == 1)
                {
                    Fishies.RemoveAt(i);
                    i--;
                }
            }


            ElapsedFishTime += gameTime.ElapsedGameTime;

            lastkb = Keyboard.GetState();

            Window.Title = Text;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            Bob.Draw(spriteBatch);

            Steve.Draw(spriteBatch);


            foreach (Fish fish in Fishies)
            {
                fish.Draw(spriteBatch);
            }



            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}