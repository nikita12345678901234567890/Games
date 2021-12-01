using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chess
{
    public class Sprite
    {
        public Texture2D texture { get; set; }
        public virtual Vector2 Position { get; set; }
        public Vector2 origin { get; set; }
        public Vector2 scale { get; set; }
        public Rectangle HitBox
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, (int)(texture.Width * scale.X), (int)(texture.Height * scale.Y));
            }
        }
        public float rotation { get; set; }
        public SpriteEffects effect { get; set; }
        public float layerDepth { get; set; }

        public Color color;

        public Sprite(Texture2D texture, Vector2 position, Vector2 scale, Vector2 origin, Color color)
        {
            this.texture = texture;
            this.origin = origin;

            this.Position = position;
            this.scale = scale;
            this.color = color;
        }

        public void Update()
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, color, rotation, origin, scale, effect, layerDepth);
        }
    }
}
