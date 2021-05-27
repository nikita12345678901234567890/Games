using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Text;

namespace Chess
{
    public abstract class Piece
    {
        public abstract PieceTypes PieceType { get; }
        public abstract bool IsWhite { get; set; }

        public abstract List<(Point, MoveType)> GetMoves(Piece[,] PieceGrid, Point position);
    }
}
