using SharedLibrary;

namespace Chess
{
    public struct GameState
    {
        public bool Whiteturn;
        public bool WhiteInCheck;
        public bool BlackInCheck;
        public Piece[,] PieceGrid;
        public int moveCounter;
        public bool ChoosingPromotion;
    }
}