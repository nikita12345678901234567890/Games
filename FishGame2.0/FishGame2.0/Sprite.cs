using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Text;

namespace FishGame2._0
{
    public class Sprite
    {
        public Texture2D Image;
        public Vector2 Position;
        public virtual Vector2 Origin { get; set; }
        public Vector2 Scale;
        public Color Color;
        public Rectangle? Source;
        public float Rotation;

        public virtual Rectangle HitBox => new Rectangle((Position - Origin * Scale).ToPoint(), new Point((int)(Image.Width * Scale.X), (int)(Image.Height * Scale.Y)));

        public Sprite(Texture2D image, Vector2 position, Vector2 origin, Vector2 scale, Color color)
        {
            Image = image;
            Position = position;
            Origin = origin;
            Scale = scale;
            Color = color;
            Source = null;
        }

        public virtual void Update(GameTime gameTime)
        { 
            
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Image, Position, Source, Color, Rotation, Origin, Scale, SpriteEffects.None, 0);
        }
    }
}
