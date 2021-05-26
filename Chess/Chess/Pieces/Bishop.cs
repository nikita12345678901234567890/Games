using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.Pieces
{
    class Bishop : Piece
    {
        public override PieceTypes PieceType => PieceTypes.Bishop;
        public override bool IsWhite { get; set; }

        public Bishop(bool isWhite)
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
