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
        public override List<(Point, MoveTypes)> GetMoves(Piece[,] PieceGrid, Point position)
        {
            List<(Point, MoveTypes)> Moves = new List<(Point, MoveTypes)>();

            

            return Moves;
        }
    }
}
