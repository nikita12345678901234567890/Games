using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.Pieces
{
    class Knight : Piece
    {
        public override PieceTypes PieceType => PieceTypes.Knight;
        public override bool IsWhite { get; set; }

        public Knight(bool isWhite)
        {
            IsWhite = isWhite;
        }
        public override List<(Point, MoveType)> GetMoves(Piece[,] PieceGrid, Point position)
        {
            List<(Point, MoveType)> Moves = new List<(Point, MoveType)>();

            if (position.X + 1 < PieceGrid.GetLength(1) && position.Y + 2 < PieceGrid.GetLength(0))
            {
                var piece = PieceGrid[position.Y + 2, position.X + 1];
                if (piece == null || (piece != null && piece.IsWhite == !IsWhite))
                {
                    Moves.Add((new Point(position.X + 1, position.Y + 2), MoveType.Normal));
                }
            }
            if (position.X + 2 < PieceGrid.GetLength(1) && position.Y + 1 < PieceGrid.GetLength(0))
            {
                var piece = PieceGrid[position.Y + 1, position.X + 2];
                if (piece == null || (piece != null && piece.IsWhite == !IsWhite))
                {
                    Moves.Add((new Point(position.X + 2, position.Y + 1), MoveType.Normal));
                }
            }

            if (position.X - 1 >= 0 && position.Y + 2 < PieceGrid.GetLength(0))
            {
                var piece = PieceGrid[position.Y + 2, position.X - 1];
                if (piece == null || (piece != null && piece.IsWhite == !IsWhite))
                {
                    Moves.Add((new Point(position.X - 1, position.Y + 2), MoveType.Normal));
                }
            }
            if (position.X - 2 >= 0 && position.Y + 1 < PieceGrid.GetLength(0))
            {
                var piece = PieceGrid[position.Y + 1, position.X - 2];
                if (piece == null || (piece != null && piece.IsWhite == !IsWhite))
                {
                    Moves.Add((new Point(position.X - 2, position.Y + 1), MoveType.Normal));
                }
            }

            if (position.X + 1 < PieceGrid.GetLength(1) && position.Y - 2 >= 0)
            {
                var piece = PieceGrid[position.Y - 2, position.X + 1];
                if (piece == null || (piece != null && piece.IsWhite == !IsWhite))
                {
                    Moves.Add((new Point(position.X + 1, position.Y - 2), MoveType.Normal));
                }
            }
            if (position.X + 2 < PieceGrid.GetLength(1) && position.Y - 1 >= 0)
            {
                var piece = PieceGrid[position.Y - 1, position.X + 2];
                if (piece == null || (piece != null && piece.IsWhite == !IsWhite))
                {
                    Moves.Add((new Point(position.X + 2, position.Y - 1), MoveType.Normal));
                }
            }

            if (position.X - 1 >= 0 && position.Y - 2 >= 0)
            {
                var piece = PieceGrid[position.Y - 2, position.X - 1];
                if (piece == null || (piece != null && piece.IsWhite == !IsWhite))
                {
                    Moves.Add((new Point(position.X - 1, position.Y - 2), MoveType.Normal));
                }
            }
            if (position.X - 2 >= 0 && position.Y - 1 >= 0)
            {
                var piece = PieceGrid[position.Y - 1, position.X - 2];
                if (piece == null || (piece != null && piece.IsWhite == !IsWhite))
                {
                    Moves.Add((new Point(position.X - 2, position.Y - 1), MoveType.Normal));
                }
            }

            return Moves;
        }
    }
}
