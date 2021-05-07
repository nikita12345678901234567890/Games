using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Text;

namespace FishGame2._0
{
    public class Fish : AnimatedSprite
    {
        public Random random;
        public Vector2 Velocity;
        public Vector2 Acceleration;
        public Directions Direction = Directions.None;

        public Texture2D LaserTexture;

        bool FullAuto = false;
        TimeSpan FullAutoDelay = TimeSpan.FromMilliseconds(50);
        TimeSpan FullAutoElapsedTime = TimeSpan.Zero;

        public bool IsAi = false;
        public Keys Forward;
        public Keys Backward;
        public Keys Left;
        public Keys Right;
        public Keys Shoot;
        public Keys Auto;

        public Fish(Rectangle[] frames, TimeSpan delay, Texture2D image, Texture2D laser, Vector2 position, Vector2 origin, Vector2 scale, Color color, int layout) 
            : base(frames, delay, image, position, origin, scale, color)
        {
            LaserTexture = laser;

            if (layout == 0)
            {
                Forward = Keys.Up;
                Backward = Keys.Down;
                Left = Keys.Left;
                Right = Keys.Right;
                Shoot = Keys.Space;
                Auto = Keys.RightControl;
            }
            else if (layout == 1)
            {
                Forward = Keys.W;
                Backward = Keys.S;
                Left = Keys.A;
                Right = Keys.D;
                Shoot = Keys.LeftShift;
                Auto = Keys.LeftControl;
            }
            else
            {
                random = new Random();
                IsAi = true;
                Forward = Keys.F1;
                Backward = Keys.F2;
                Left = Keys.F3;
                Right = Keys.F4;
                Shoot = Keys.F5;
            }
        }

        public int Update(KeyboardState kb, KeyboardState lastkb, GameTime gameTime, Vector2 screen)
        {
            //AI logic:
            int choice = int.MinValue;
            if (IsAi)
            {
                choice = random.Next(1, 5);
            }

            //AI dying:
            if (IsAi)
            {
                for (int i = 0; i < Game1.Lasers.Count; i++)
                {
                    if (Game1.Lasers[i].HitBox.Intersects(HitBox))
                    {
                        Game1.Lasers.RemoveAt(i);
                        return 1;
                    }
                }
            }

            //Moving
            if (kb.IsKeyDown(Forward) || choice == 1)
            {
                Acceleration += new Vector2((float)Math.Cos(Rotation - Math.PI / 2), (float)Math.Sin(Rotation - Math.PI / 2));
                Acceleration.Normalize();
                Acceleration *= 0.1f;

                Wiggle = true;
            }
            else if (kb.IsKeyDown(Backward) || choice == 2)
            {
                Acceleration -= new Vector2((float)Math.Cos(Rotation - Math.PI / 2), (float)Math.Sin(Rotation - Math.PI / 2));
                Acceleration.Normalize();
                Acceleration *= 0.1f;

                Wiggle = true;
            }
            else
            {
                Velocity *= 0.9f;

                Wiggle = false;
            }

            //Turning
            if (kb.IsKeyDown(Right) || choice == 3)
            {
                Rotation += 0.0698132f;

                Wiggle = true;
            }
            else if (kb.IsKeyDown(Left) || choice == 4)
            {
                Rotation -= 0.0698132f;

                Wiggle = true;
            }


            //stop at walls:
            if (Position.X > screen.X)
            {
                Velocity.X = 0;
                Position.X = screen.X;
            }
            if (Position.X < 0)
            {
                Velocity.X = 0;
                Position.X = 0;
            }
            if (Position.Y > screen.Y)
            {
                Velocity.Y = 0;
                Position.Y = screen.Y;
            }
            if (Position.Y < 0)
            {
                Velocity.Y = 0;
                Position.Y = 0;
            }


            //LASERS!!!!!:
            if (kb.IsKeyDown(Shoot) && lastkb.IsKeyUp(Shoot))
            {
                Game1.Lasers.Add(new Laser(LaserTexture, Position, new Vector2(Origin.X - 5, Origin.Y - 12), Vector2.One * 5, Color.White));
                Game1.Lasers[Game1.Lasers.Count - 1].Rotation = (float)(Rotation + Math.PI / 2);
            }

            //Full auto:
            if (kb.IsKeyDown(Auto) && lastkb.IsKeyUp(Auto))
            {
                FullAuto = !FullAuto;
            }
            
            FullAutoElapsedTime += gameTime.ElapsedGameTime;
            if (FullAuto)
            {
                while (FullAutoElapsedTime >= FullAutoDelay)
                {
                    Game1.Lasers.Add(new Laser(LaserTexture, Position, new Vector2(Origin.X - 5, Origin.Y - 12), Vector2.One * 5, Color.White));
                    Game1.Lasers[Game1.Lasers.Count - 1].Rotation = (float)(Rotation + Math.PI / 2);

                    FullAutoElapsedTime -= FullAutoDelay;
                }
            }

            for (int i = 0; i < Game1.Lasers.Count; i++)
            {
                Game1.Lasers[i].Update(gameTime);
                if (Game1.Lasers[i].Position.X > screen.X || Position.X < 0 || Position.Y > screen.Y || Position.Y < 0)
                {
                    Game1.Lasers.RemoveAt(i);
                    i--;
                }
            }

            Velocity += Acceleration;
            Position += Velocity;
            base.Update(gameTime);

            return 0;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (Laser laser in Game1.Lasers)
            {
                laser.Draw(spriteBatch);
            }



            base.Draw(spriteBatch);
        }
    }
}
