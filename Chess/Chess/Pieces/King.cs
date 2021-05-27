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
        public override List<(Point, MoveType)> GetMoves(Piece[,] PieceGrid, Point position)
        {
            List<(Point, MoveType)> Moves = new List<(Point, MoveType)>();



            return Moves;
        }
    }
}
