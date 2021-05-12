using Microsoft.Xna.Framework.Input;

namespace FishGame2._0
{
    public struct PlayerKeyboardLayout
    {
        public Keys Forward { get; init; }
        public Keys Backward { get; init; }
        public Keys Left { get; init; }
        public Keys Right { get; init; }
        public Keys Shoot { get; init; }
        public Keys Auto { get; init; }

        public static PlayerKeyboardLayout WASD { get; }
            = new PlayerKeyboardLayout()
            {
                Forward = Keys.W,
                Backward = Keys.S,
                Left = Keys.A,
                Right = Keys.D,
                Shoot = Keys.LeftShift,
                Auto = Keys.LeftControl
            };

        public static PlayerKeyboardLayout ArrowKeys { get; }
            = new PlayerKeyboardLayout()
            {
                Forward = Keys.Up,
                Backward = Keys.Down,
                Left = Keys.Left,
                Right = Keys.Right,
                Shoot = Keys.Space,
                Auto = Keys.RightControl
            };

        public static PlayerKeyboardLayout AI { get; }
            = new PlayerKeyboardLayout()
            {
                Forward = Keys.F1,
                Backward = Keys.F2,
                Left = Keys.F3,
                Right = Keys.F4,
                Shoot = Keys.F5,
                Auto = Keys.None
            };

    }
}