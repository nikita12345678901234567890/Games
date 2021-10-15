using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary
{
    public abstract class Piece
    {
        protected ChessGame owningGame;

        public abstract PieceTypes PieceType { get; }
        public abstract bool IsWhite { get; set; }

        public abstract List<(Square, MoveTypes)> GetMoves(Piece[,] PieceGrid, Square position);
    }
}