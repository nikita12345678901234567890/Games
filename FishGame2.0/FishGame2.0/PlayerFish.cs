using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Text;

namespace FishGame2._0
{
    public class PlayerFish : Fish
    {
        internal PlayerKeyboardLayout KeysLayout { get; }

        bool FullAuto = false;
        TimeSpan FullAutoDelay = TimeSpan.FromMilliseconds(50);
        TimeSpan FullAutoElapsedTime = TimeSpan.Zero;

        public PlayerFish(Rectangle[] frames, TimeSpan delay, Texture2D image, Texture2D laser, Vector2 position, Vector2 origin, Vector2 scale, Color color, PlayerKeyboardLayout layout)
            : base(frames, delay, image, laser, position, origin, scale, color)
        {
            KeysLayout = layout;
        }

        public int Update(KeyboardState kb, KeyboardState lastkb, GameTime gameTime, Vector2 screen)
        {
            //Moving
            if (kb.IsKeyDown(KeysLayout.Forward))
            {
                Acceleration += new Vector2((float)Math.Cos(Rotation - Math.PI / 2), (float)Math.Sin(Rotation - Math.PI / 2));
                Acceleration.Normalize();
                Acceleration *= 0.1f;

                Wiggle = true;
            }
            else if (kb.IsKeyDown(KeysLayout.Backward))
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
            if (kb.IsKeyDown(KeysLayout.Right))
            {
                Rotation += 0.0698132f;

                Wiggle = true;
            }
            else if (kb.IsKeyDown(KeysLayout.Left))
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
            if (kb.IsKeyDown(KeysLayout.Shoot) && lastkb.IsKeyUp(KeysLayout.Shoot))
            {
                Game1.Lasers.Add(new Laser(LaserTexture, Position, new Vector2(Origin.X - 5, Origin.Y - 12), Vector2.One * 5, Color.White));
                Game1.Lasers[Game1.Lasers.Count - 1].Rotation = (float)(Rotation + Math.PI / 2);
            }

            //Full auto:
            if (kb.IsKeyDown(KeysLayout.Auto) && lastkb.IsKeyUp(KeysLayout.Auto))
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



            return base.Update(gameTime);
        }
    }
}
