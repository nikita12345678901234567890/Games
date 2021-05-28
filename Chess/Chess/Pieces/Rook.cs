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
            /*
            To-Do:
            Add castling
            */

            List<(Point, MoveTypes)> Moves = new List<(Point, MoveTypes)>();

            Point counter = position;

            Directions direction = Directions.Up;

            while (true)
            {
                switch (direction)
                {
                    case Directions.Up:
                        if (counter.Y > 0)
                        {
                            counter.Y--;
                        }
                        else
                        {
                            direction++;
                            counter = position;
                            continue;
                        }
                        break;

                    case Directions.Down:
                        if (counter.Y < PieceGrid.GetLength(0) - 1)
                        {
                            counter.Y++;
                        }
                        else
                        {
                            direction++;
                            counter = position;
                            continue;
                        }
                        break;

                    case Directions.Left:
                        if (counter.X > 0)
                        {
                            counter.X--;
                        }
                        else
                        {
                            direction++;
                            counter = position;
                            continue;
                        }
                        break;

                    case Directions.Right:
                        if (counter.X < PieceGrid.GetLength(1) - 1)
                        {
                            counter.X++;
                        }
                        else
                        {
                            return Moves;
                        }
                        break;
                }

                var piece = PieceGrid[counter.Y, counter.X];
                if (piece != null && piece.IsWhite == IsWhite)
                {
                    if (direction != Directions.Right)
                    {
                        direction++;
                        counter = position;
                    }
                    else
                    {
                        return Moves;
                    }
                }
                else if (piece != null && piece.IsWhite == !IsWhite)
                {
                    Moves.Add((counter, MoveTypes.Normal));
                    if (direction != Directions.Right)
                    {
                        direction++;
                        counter = position;
                    }
                    else
                    {
                        return Moves;
                    }
                }
                else
                {
                    Moves.Add((counter, MoveTypes.Normal));
                }
            }

            return Moves;
        }
    }
}