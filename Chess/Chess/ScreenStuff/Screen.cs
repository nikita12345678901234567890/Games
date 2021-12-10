using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public abstract class Screen
    {
        public abstract UpdateResult Update(GameTime gameTime, bool IsActive);

        public abstract void Draw(SpriteBatch spriteBatch);
    }
}