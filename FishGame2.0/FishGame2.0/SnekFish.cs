using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace FishGame2._0
{
    public class SnekFish : AiFish
    {
        int player;
        public SnekFish(AnimationFrame[] frames, TimeSpan delay, Texture2D image, Texture2D laser, Vector2 position, Vector2 origin, Vector2 scale, Color color, PlayerKeyboardLayout layout, int startHealth = 1)
            : base(frames, delay, image, laser, position, origin, scale, color, layout, startHealth)
        {
            player = random.Next(0, 2);
        }

        public override int Update(GameTime gameTime, Vector2 screen)
        {
            //Moving
            Wiggle = true;
            double angle = 0;

            Vector2 delta = Position - Game1.Bob.Position;
            Vector2 delta2 = Position - Game1.Steve.Position;

            if (!Game1.Bob.Alive)
            {
                angle = Math.Atan2(delta2.Y, delta2.X);
            }
            else if (!Game1.Steve.Alive)
            {
                angle = Math.Atan2(delta.Y, delta.X);
            }
            else if (delta.Length() < delta2.Length())
            {
                angle = Math.Atan2(delta.Y, delta.X);
            }
            else
            {
                angle = Math.Atan2(delta2.Y, delta2.X);
            }


            Rotation = (float)(angle - 90 * Math.PI / 180);

            Acceleration += new Vector2((float)Math.Cos(Rotation - Math.PI / 2), (float)Math.Sin(Rotation - Math.PI / 2));
            Acceleration.Normalize();
            Acceleration *= 0.05f;

            Wiggle = true;

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

            //Killing:
            if (HitBox.Intersects(Game1.Bob.HitBox))
            {
                Game1.Bob.Alive = false;
            }
            if (HitBox.Intersects(Game1.Steve.HitBox))
            {
                Game1.Steve.Alive = false;
            }


            return base.Update(gameTime);
        }
    }
}