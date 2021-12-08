using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.ScreenStuff
{
    public class ScreenManager
    {
        public Stack<Screen> screens;
        private ContentManager content;
        private GraphicsDeviceManager graphics;

        public ScreenManager(ContentManager content, GraphicsDeviceManager graphics)
        {
            screens = new Stack<Screen>();

            screens.Push(new MenuScreen(content, graphics));
        }

        public void Update(GameTime gameTime, bool IsActive)
        {
            var result = screens.Peek().Update(gameTime, IsActive);

            if (result.moveOn)
            {
                if (screens.Peek() is MenuScreen menuScreen)
                {
                    screens.Push(new NormalGameScreen(content, graphics));
                }
                else
                { 
                    screens.Pop();
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            screens.Peek().Draw(spriteBatch);
        }
    }
}