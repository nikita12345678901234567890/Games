using System;

namespace Chess
{
    public static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var gameID = Guid.Parse(args[0]);

            using (var game = new Game1(gameID))
            {
                game.Run();
            }
        }
    }
}