using Microsoft.Xna.Framework;
using SharedLibrary;

namespace Chess
{
    public static class Extensions
    {
        public static Point ToPoint(this Square square)
            => new Point(square.X, square.Y);
    }
}