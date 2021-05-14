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
        public int Health;

        public Texture2D LaserTexture;


        public bool IsAi = false;

        public Fish(AnimationFrame[] frames, TimeSpan delay, Texture2D image, Texture2D laser, Vector2 position, Vector2 origin, Vector2 scale, Color color, int startHealth = 1)
            : base(frames, delay, image, position, origin, scale, color)
        {
            LaserTexture = laser;

            random = new Random();

            Health = startHealth;
        }
        
        public new virtual int Update(GameTime gameTime)
        {
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