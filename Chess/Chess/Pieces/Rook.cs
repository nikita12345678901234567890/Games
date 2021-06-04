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

            List<(Point, MoveTypes)> potentialMoves = new List<(Point, MoveTypes)>();

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
                    potentialMoves.Add((counter, MoveTypes.Normal));
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
                else if (!potentialMoves.Contains((counter, MoveTypes.Normal)))
                {
                    potentialMoves.Add((counter, MoveTypes.Normal));
                }
                else
                {
                    break;
                }
            }

            foreach (var potentialMove in potentialMoves)
            {
                bool skipMove = false;

                //Exectuing the move:
                Piece whatWasThere = PieceGrid[potentialMove.Item1.Y, potentialMove.Item1.X];
                PieceGrid[potentialMove.Item1.Y, potentialMove.Item1.X] = PieceGrid[position.Y, position.X];
                PieceGrid[position.Y, position.X] = null;

                //Checking if this pieces color is in check:
                for (int x1 = 0; x1 < 8; x1++)
                {
                    for (int y1 = 0; y1 < 8; y1++)
                    {
                        //Making sure it's not recursive:
                        if (x1 != potentialMove.Item1.Y && y1 != potentialMove.Item1.X)
                        {
                            if (PieceGrid[y1, x1] != null && PieceGrid[y1, x1].IsWhite != IsWhite && PieceGrid[y1, x1].PieceType != PieceTypes.Pawn)
                            {
                                var temp = PieceGrid[y1, x1].GetMoves(PieceGrid, new Point(x1, y1));
                                foreach (var move in temp)
                                {
                                    if (Game1.IsChecking(PieceGrid[y1, x1], new Point(x1, y1), PieceGrid, potentialMove.Item1))
                                    {
                                        skipMove = true;
                                    }
                                }
                            }
                        }
                    }
                }

                //Reversing the exectued moves:
                PieceGrid[position.Y, position.X] = PieceGrid[potentialMove.Item1.Y, potentialMove.Item1.X];
                PieceGrid[potentialMove.Item1.Y, potentialMove.Item1.X] = whatWasThere;

                if (!skipMove)
                {
                    Moves.Add((new Point(potentialMove.Item1.X, potentialMove.Item1.Y), MoveTypes.Normal));
                }
            }

            return Moves;
        }
    }
}