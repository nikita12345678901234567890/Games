using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.Pieces
{
    class Queen : Piece
    {
        public override PieceTypes PieceType => PieceTypes.Queen;
        public override bool IsWhite { get; set; }

        public Queen(bool isWhite)
        {
            IsWhite = isWhite;
        }
        public override List<(Point, MoveType)> GetMoves(Piece[,] PieceGrid, Point position)
        {
            List<(Point, MoveType)> Moves = new List<(Point, MoveType)>();



            return Moves;
        }
    }
}
