using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Pieces
{
    public class Rook : Piece
    {
        public override PieceTypes PieceType => PieceTypes.Rook;
        public override bool IsWhite { get; set; }

        public bool HasMoved { get; set; }

        public Rook(bool isWhite)
        {
            IsWhite = isWhite;

            HasMoved = false;
        }
        public override List<(Point, MoveTypes)> GetMoves(Piece[,] PieceGrid, Point position)
        {

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
                        break;
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
                        break;
                    }
                }
                else if (!Moves.Contains((counter, MoveTypes.Normal)))
                {
                    Moves.Add((counter, MoveTypes.Normal));
                }
                else
                {
                    break;
                }
            }

            return Moves;
        }
    }
}