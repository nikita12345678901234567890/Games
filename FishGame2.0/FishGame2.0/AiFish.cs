using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Text;

namespace FishGame2._0
{
    public class AiFish : Fish
    {
        public AiFish(AnimationFrame[] frames, TimeSpan delay, Texture2D image, Texture2D laser, Vector2 position, Vector2 origin, Vector2 scale, Color color, PlayerKeyboardLayout layout, int startHealth = 1)
            : base(frames, delay, image, laser, position, origin, scale, color, startHealth)
        {

        }

        public virtual int Update(GameTime gameTime, Vector2 screen)
        {
            int choice = choice = random.Next(1, 5);

            //Moving
            if (choice == 1)
            {
                Acceleration += new Vector2((float)Math.Cos(Rotation - Math.PI / 2), (float)Math.Sin(Rotation - Math.PI / 2));
                Acceleration.Normalize();
                Acceleration *= 0.1f;

                Wiggle = true;
            }
            else if (choice == 2)
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
            if (choice == 3)
            {
                Rotation += 0.0698132f;

                Wiggle = true;
            }
            else if (choice == 4)
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


            //taking damage and dying:
            for (int i = 0; i < Game1.Lasers.Count; i++)
            {
                if (Game1.Lasers[i].HitBox.Intersects(HitBox))
                {
                    Health--;
                    Game1.Lasers.RemoveAt(i);

                    if (Health <= 0)
                    {
                        return 1;
                    }
                }
            }


            return base.Update(gameTime);
        }
    }
}
