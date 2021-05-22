using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.Pieces
{
    class King : Piece
    {
        public override PieceTypes PieceType => PieceTypes.King;
        public override bool IsWhite { get; set; }

        public King(bool isWhite)
        {
            IsWhite = isWhite;
        }
        public override List<Point> GetMoves(Piece[,] PieceGrid, Point position)
        {
            List<Point> Moves = new List<Point>();



            return Moves;
        }
    }
}
