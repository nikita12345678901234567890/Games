using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.Pieces
{
    class Rook : Piece
    {
        public override PieceTypes PieceType => PieceTypes.Rook;
        public override bool IsWhite { get; set; }

        public Rook(bool isWhite)
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
