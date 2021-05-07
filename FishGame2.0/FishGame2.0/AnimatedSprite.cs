using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Text;

namespace FishGame2._0
{
    public class AnimatedSprite : Sprite
    {
        public Rectangle[] Frames;
        public int current;

        public TimeSpan Delay;
        public TimeSpan Elapsed;

        public bool Wiggle;

        public AnimatedSprite(Rectangle[] frames, TimeSpan delay, Texture2D image, Vector2 position, Vector2 origin, Vector2 scale, Color color) : base(image, position, origin, scale, color)
        {
            Frames = frames;
            Delay = delay;

            Wiggle = false;
        }

        public override void Update(GameTime gameTime)
        {
            Elapsed += gameTime.ElapsedGameTime;
            if (Elapsed > Delay)
            {
                if (Wiggle)
                {
                    current++;
                }
                else
                {
                    current = 1;
                }
                if (current >= Frames.Length)
                {
                    current = 0;
                }

                Elapsed = TimeSpan.Zero;
            }

            Source = Frames[current];

            base.Update(gameTime);
        }
    }
}
