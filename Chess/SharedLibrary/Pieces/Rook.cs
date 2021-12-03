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
        public override List<(Square, MoveTypes)> GetMoves(ChessGame owningGame, Square position)
        {

            List<(Square, MoveTypes)> Moves = new List<(Square, MoveTypes)>();

            Square counter = position;

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
                        if (counter.Y < owningGame.PieceGrid.GetLength(0) - 1)
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
                        if (counter.X < owningGame.PieceGrid.GetLength(1) - 1)
                        {
                            counter.X++;
                        }
                        break;
                }

                var piece = owningGame.PieceGrid[counter.Y, counter.X];
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