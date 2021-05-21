using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.Pieces
{
    public class Pawn : Piece
    {
        public override PieceTypes PieceType => PieceTypes.Pawn;
        public override bool IsWhite { get; set; }

        public Pawn(bool isWhite)
        {
            IsWhite = isWhite;
        }
        public override List<Point> GetMoves()
        {
            return null;
        }
    }
}