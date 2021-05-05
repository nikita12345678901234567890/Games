using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationIntro
{
    public class AnimatedSprite : Sprite
    {
        public Rectangle[] Frames;
        public int currentFrame;

        public TimeSpan Delay;

        public AnimatedSprite(Rectangle[] frames, TimeSpan delay, Texture2D image, Vector2 position, Vector2 origin, Vector2 scale, Color color) : base(image, position, origin, scale, color)
        {
            Frames = frames;
            Delay = delay;
        }

        public void NextFrame()
        { 

        }
    }
}
