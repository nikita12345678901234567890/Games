using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace FishGame2._0
{
    public class Laser : Sprite
    {
        public Vector2 Velocity => (new Vector2((float)Math.Cos(Rotation + Math.PI), (float)Math.Sin(Rotation + Math.PI))) * Game1.LaserSpeed * 0.1f;

        public Laser(Texture2D image, Vector2 position, Vector2 origin, Vector2 scale, Color color)
            : base(image, position, origin, scale, color)
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            Position += Velocity;


            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {



            base.Draw(spriteBatch);
        }
    }
}