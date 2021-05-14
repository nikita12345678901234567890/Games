using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System;
using System.Collections.Generic;

namespace FishGame2._0
{

    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;


        public static int LaserSpeedCoefficient = 10;
        public static int LaserSpeed = 10;

        public static Texture2D Pixel;

        public static PlayerFish Bob;
        public static PlayerFish Steve;
        List<AiFish> Fishies;
        KeyboardState lastkb;

        TimeSpan FishSpawnTime = TimeSpan.FromMilliseconds(500);
        TimeSpan ElapsedFishTime;

        public List<Rectangle> AiRectangles;
        public List<Rectangle> TankRectangles;
        public List<Rectangle> SnekRectangles;

        public static List<Laser> Lasers;

        Random random = new Random();

        public static string Text;

        bool playing = true;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 1000;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Lasers = new List<Laser>();

            Pixel = new Texture2D(GraphicsDevice, 1, 1);
            Pixel.SetData(new[] { Color.White });

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

            TankRectangles = new List<Rectangle>();

            TankRectangles.Add(new Rectangle(1, 224, 30, 32));
            TankRectangles.Add(new Rectangle(35, 224, 26, 32));
            TankRectangles.Add(new Rectangle(65, 224, 30, 32));
            TankRectangles.Add(new Rectangle(35, 224, 26, 32));

            SnekRectangles = new List<Rectangle>();

            SnekRectangles.Add(new Rectangle(105, 224, 13, 32));
            SnekRectangles.Add(new Rectangle(138, 224, 13, 32));


            Bob = new PlayerFish(BobRectangles.Select((currentAiRectangle) => new AnimationFrame(currentAiRectangle)).ToArray(), TimeSpan.FromMilliseconds(100), Content.Load<Texture2D>("FishSheet1"), Content.Load<Texture2D>("Laser"), new Vector2(50, 50), new Vector2(7.5f, 13), Vector2.One, Color.White, PlayerKeyboardLayout.ArrowKeys);
            Steve = new PlayerFish(SteveRectangles.Select((currentAiRectangle) => new AnimationFrame(currentAiRectangle)).ToArray(), TimeSpan.FromMilliseconds(100), Content.Load<Texture2D>("FishSheet1"), Content.Load<Texture2D>("Laser"), new Vector2(500, 500), new Vector2(7.5f, 13), Vector2.One, Color.White, PlayerKeyboardLayout.WASD);


            Fishies = new List<AiFish>();
            for (int i = 0; i < 10; i++)
            {
                int thing = random.Next(0, 10);
                //Generating a tank every 1 in 10 fish:
                if (thing == 9)
                {
                    //Add tankFish;
                    Fishies.Add(new AiFish(TankRectangles.Select((currentAiRectangle) => new AnimationFrame(currentAiRectangle)).ToArray(), TimeSpan.FromMilliseconds(100), Content.Load<Texture2D>("FishSheet2"), Content.Load<Texture2D>("Laser"), new Vector2(random.Next(50, graphics.PreferredBackBufferWidth - 50), random.Next(50, graphics.PreferredBackBufferHeight - 50)), new Vector2(7.5f, 13), Vector2.One, Color.White, PlayerKeyboardLayout.AI, 3));
                }
                else if (thing == 8)
                {
                    //Add SnekFish:
                    Fishies.Add(new SnekFish(SnekRectangles.Select((currentAiRectangle) => new AnimationFrame(currentAiRectangle)).ToArray(), TimeSpan.FromMilliseconds(100), Content.Load<Texture2D>("FishSheet2"), Content.Load<Texture2D>("Laser"), new Vector2(random.Next(50, graphics.PreferredBackBufferWidth - 50), random.Next(50, graphics.PreferredBackBufferHeight - 50)), new Vector2(7.5f, 13), Vector2.One, Color.White, PlayerKeyboardLayout.AI));
                }
                else
                {
                    //Add normal AiFish:
                    Fishies.Add(new AiFish(AiRectangles.Select((currentAiRectangle) => new AnimationFrame(currentAiRectangle)).ToArray(), TimeSpan.FromMilliseconds(100), Content.Load<Texture2D>("FishSheet1"), Content.Load<Texture2D>("Laser"), new Vector2(random.Next(50, graphics.PreferredBackBufferWidth - 50), random.Next(50, graphics.PreferredBackBufferHeight - 50)), new Vector2(7.5f, 13), Vector2.One, Color.White, PlayerKeyboardLayout.AI));
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (playing)
            {
                //Winning:
                if (Fishies.Count == 0)
                {
                    playing = false;
                    MessageBox.Show("You won!", "Good job", new string[]{"Ok"});
                }

                if (Keyboard.GetState().IsKeyDown(Keys.NumPad0)) LaserSpeed = LaserSpeedCoefficient * 10;
                if (Keyboard.GetState().IsKeyDown(Keys.NumPad1)) LaserSpeed = LaserSpeedCoefficient * 1;
                if (Keyboard.GetState().IsKeyDown(Keys.NumPad2)) LaserSpeed = LaserSpeedCoefficient * 2;
                if (Keyboard.GetState().IsKeyDown(Keys.NumPad3)) LaserSpeed = LaserSpeedCoefficient * 3;
                if (Keyboard.GetState().IsKeyDown(Keys.NumPad4)) LaserSpeed = LaserSpeedCoefficient * 4;
                if (Keyboard.GetState().IsKeyDown(Keys.NumPad5)) LaserSpeed = LaserSpeedCoefficient * 5;
                if (Keyboard.GetState().IsKeyDown(Keys.NumPad6)) LaserSpeed = LaserSpeedCoefficient * 6;
                if (Keyboard.GetState().IsKeyDown(Keys.NumPad7)) LaserSpeed = LaserSpeedCoefficient * 7;
                if (Keyboard.GetState().IsKeyDown(Keys.NumPad8)) LaserSpeed = LaserSpeedCoefficient * 8;
                if (Keyboard.GetState().IsKeyDown(Keys.NumPad9)) LaserSpeed = LaserSpeedCoefficient * 9;

                Bob.Update(Keyboard.GetState(), lastkb, gameTime, new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight));

                Steve.Update(Keyboard.GetState(), lastkb, gameTime, new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight));


                if (ElapsedFishTime >= FishSpawnTime)
                {
                    ElapsedFishTime = TimeSpan.Zero;

                    int thing = random.Next(0, 10);
                    //Generating a tank every 1 in 10 fish:
                    if (thing == 9)
                    {
                        //Add tankFish;
                        Fishies.Add(new AiFish(TankRectangles.Select((currentAiRectangle) => new AnimationFrame(currentAiRectangle)).ToArray(), TimeSpan.FromMilliseconds(100), Content.Load<Texture2D>("FishSheet2"), Content.Load<Texture2D>("Laser"), new Vector2(random.Next(50, graphics.PreferredBackBufferWidth - 50), random.Next(50, graphics.PreferredBackBufferHeight - 50)), new Vector2(7.5f, 13), Vector2.One, Color.White, PlayerKeyboardLayout.AI, 3));
                    }
                    else if (thing == 8)
                    {
                        //Add Snekfish:
                        Fishies.Add(new SnekFish(SnekRectangles.Select((currentAiRectangle) => new AnimationFrame(currentAiRectangle)).ToArray(), TimeSpan.FromMilliseconds(100), Content.Load<Texture2D>("FishSheet2"), Content.Load<Texture2D>("Laser"), new Vector2(random.Next(50, graphics.PreferredBackBufferWidth - 50), random.Next(50, graphics.PreferredBackBufferHeight - 50)), new Vector2(7.5f, 13), Vector2.One, Color.White, PlayerKeyboardLayout.AI));
                    }
                    else
                    {
                        //Add normal AiFish:
                        Fishies.Add(new AiFish(AiRectangles.Select((currentAiRectangle) => new AnimationFrame(currentAiRectangle)).ToArray(), TimeSpan.FromMilliseconds(100), Content.Load<Texture2D>("FishSheet1"), Content.Load<Texture2D>("Laser"), new Vector2(random.Next(50, graphics.PreferredBackBufferWidth - 50), random.Next(50, graphics.PreferredBackBufferHeight - 50)), new Vector2(7.5f, 13), Vector2.One, Color.White, PlayerKeyboardLayout.AI));
                    }
                }


                for (int i = 0; i < Fishies.Count; i++)
                {
                    int value = Fishies[i].Update(gameTime, new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight));
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