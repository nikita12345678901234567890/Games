namespace SharedLibrary
{
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
    public struct Square
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
    {
        public int X;
        public int Y;

        public Square(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static bool operator ==(Square a, Square b)
            => a.X == b.X && a.Y == b.Y;

        public static bool operator !=(Square a, Square b)
            => !(a == b);

    }
}