using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace FishGame2._0
{
    public class AnimatedSprite : Sprite
    {
        public AnimationFrame[] Frames;
        public int current;

        public TimeSpan Delay;
        public TimeSpan Elapsed;

        public bool Wiggle;

        public override Rectangle HitBox => new Rectangle((Position - Frames[current].Origin * Scale).ToPoint(), new Point((int)(Frames[current].Frame.Width * Scale.X), (int)(Frames[current].Frame.Height * Scale.Y)));

        public override Vector2 Origin { get => Frames[current].Origin; }
        public AnimatedSprite(AnimationFrame[] frames, TimeSpan delay, Texture2D image, Vector2 position, Vector2 origin, Vector2 scale, Color color) : base(image, position, origin, scale, color)
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

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Image, Position, Frames[current].Frame, Color, Rotation, Frames[current].Origin, Scale, SpriteEffects.None, 0);
        }
    }
}