using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary
{
    public abstract class Piece
    {
        public abstract PieceTypes PieceType { get; }
        public abstract bool IsWhite { get; set; }

        public abstract List<(Square, MoveTypes)> GetMoves(ChessGame owningGame, Square position);
    }
}