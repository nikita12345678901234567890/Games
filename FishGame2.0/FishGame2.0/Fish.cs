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
        public Vector2 Velocity;
        public Vector2 Acceleration;
        public Directions Direction = Directions.None;

        public Fish(Rectangle[] frames, TimeSpan delay, Texture2D image, Vector2 position, Vector2 origin, Vector2 scale, Color color) 
            : base(frames, delay, image, position, origin, scale, color)
        {

        }

        public void Update(KeyboardState kb, KeyboardState lastkb, GameTime gameTime)
        {
            //Position += Velocity
            //Velocity += Acceleration
            //Add some acceleration

            if (kb.IsKeyDown(Keys.Up))
            {
                Acceleration += new Vector2((float)Math.Cos(Rotation - Math.PI / 2), (float)Math.Sin(Rotation - Math.PI / 2));
                Acceleration.Normalize();
                Acceleration *= 0.1f;

                Direction = Directions.Forward;
            }
            else if (kb.IsKeyDown(Keys.Down))
            {
                Acceleration -= new Vector2((float)Math.Cos(Rotation - Math.PI / 2), (float)Math.Sin(Rotation - Math.PI / 2));
                Acceleration.Normalize();
                Acceleration *= 0.1f;

                Direction = Directions.Backward;
            }
            else if(Direction != Directions.None)
            {
                if (Direction == Directions.Backward)
                {
                    //This should account for direction and we need to add it for the x-axis
                    Acceleration.Y -= 0.03f;
                    if (Velocity.X < 0)
                    {
                        Velocity.X = 0;
                        Acceleration.X = 0;
                    }
                    if (Velocity.Y < 0)
                    {
                        Velocity.Y = 0;
                        Acceleration.Y = 0;
                    }

                    if(Velocity.X <= 0 && Velocity.Y <= 0)
                    {
                        Direction = Directions.None;
                    }
                }
                else//also, add rotation
                {
                    //This should account for direction and we need to add it for the x-axis
                    Acceleration.Y += 0.03f;
                    if (Velocity.X > 0)
                    {
                        Velocity.X = 0;
                        Acceleration.X = 0;
                    }
                    if (Velocity.Y > 0)
                    {
                        Velocity.Y = 0;
                        Acceleration.Y = 0;
                    }

                    if (Velocity.X >= 0 && Velocity.Y >= 0)
                    {
                        Direction = Directions.None;
                    }
                }
            }

            Game1.Text = $"Velocity X:{Velocity.X}, Velocity Y:{Velocity.Y}";
            

            Velocity += Acceleration;
            Position += Velocity;
            base.Update(gameTime);
        }
    }
}
