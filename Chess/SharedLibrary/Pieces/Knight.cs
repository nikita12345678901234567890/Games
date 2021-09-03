using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Pieces
{
    public class Knight : Piece
    {
        public override PieceTypes PieceType => PieceTypes.Knight;
        public override bool IsWhite { get; set; }

        public Knight(bool isWhite)
        {
            IsWhite = isWhite;
        }
        public override List<(Square, MoveTypes)> GetMoves(Piece[,] PieceGrid, Square position)
        {
            List<(Square, MoveTypes)> Moves = new List<(Square, MoveTypes)>();

            if (position.X + 1 < PieceGrid.GetLength(1) && position.Y + 2 < PieceGrid.GetLength(0))
            {
                var piece = PieceGrid[position.Y + 2, position.X + 1];
                if (piece == null || (piece != null && piece.IsWhite == !IsWhite))
                {
                    Moves.Add((new Square(position.X + 1, position.Y + 2), MoveTypes.Normal));
                }
            }
            if (position.X + 2 < PieceGrid.GetLength(1) && position.Y + 1 < PieceGrid.GetLength(0))
            {
                var piece = PieceGrid[position.Y + 1, position.X + 2];
                if (piece == null || (piece != null && piece.IsWhite == !IsWhite))
                {
                    Moves.Add((new Square(position.X + 2, position.Y + 1), MoveTypes.Normal));
                }
            }

            if (position.X - 1 >= 0 && position.Y + 2 < PieceGrid.GetLength(0))
            {
                var piece = PieceGrid[position.Y + 2, position.X - 1];
                if (piece == null || (piece != null && piece.IsWhite == !IsWhite))
                {
                    Moves.Add((new Square(position.X - 1, position.Y + 2), MoveTypes.Normal));
                }
            }
            if (position.X - 2 >= 0 && position.Y + 1 < PieceGrid.GetLength(0))
            {
                var piece = PieceGrid[position.Y + 1, position.X - 2];
                if (piece == null || (piece != null && piece.IsWhite == !IsWhite))
                {
                    Moves.Add((new Square(position.X - 2, position.Y + 1), MoveTypes.Normal));
                }
            }

            if (position.X + 1 < PieceGrid.GetLength(1) && position.Y - 2 >= 0)
            {
                var piece = PieceGrid[position.Y - 2, position.X + 1];
                if (piece == null || (piece != null && piece.IsWhite == !IsWhite))
                {
                    Moves.Add((new Square(position.X + 1, position.Y - 2), MoveTypes.Normal));
                }
            }
            if (position.X + 2 < PieceGrid.GetLength(1) && position.Y - 1 >= 0)
            {
                var piece = PieceGrid[position.Y - 1, position.X + 2];
                if (piece == null || (piece != null && piece.IsWhite == !IsWhite))
                {
                    Moves.Add((new Square(position.X + 2, position.Y - 1), MoveTypes.Normal));
                }
            }

            if (position.X - 1 >= 0 && position.Y - 2 >= 0)
            {
                var piece = PieceGrid[position.Y - 2, position.X - 1];
                if (piece == null || (piece != null && piece.IsWhite == !IsWhite))
                {
                    Moves.Add((new Square(position.X - 1, position.Y - 2), MoveTypes.Normal));
                }
            }
            if (position.X - 2 >= 0 && position.Y - 1 >= 0)
            {
                var piece = PieceGrid[position.Y - 1, position.X - 2];
                if (piece == null || (piece != null && piece.IsWhite == !IsWhite))
                {
                    Moves.Add((new Square(position.X - 2, position.Y - 1), MoveTypes.Normal));
                }
            }

            return Moves;
        }
    }
}