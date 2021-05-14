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

            //if player = 0, chase Bob
            //if ()
            //{
                //Acceleration += new Vector2(Game1.Bob);
            //}

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