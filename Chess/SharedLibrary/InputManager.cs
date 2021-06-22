using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary
{
    public static class InputManager
    {
        public static KeyboardState LastKeyboardState;
        public static MouseState LastMouseState;

        public static KeyboardState KeyboardState;
        public static MouseState MouseState;
    }
}
