using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Pieces
{
    public class Bishop : Piece
    {
        public override PieceTypes PieceType => PieceTypes.Bishop;
        public override bool IsWhite { get; set; }

        public Bishop(bool isWhite)
        {
            IsWhite = isWhite;
        }
        public override List<(Square, MoveTypes)> GetMoves(ChessGame owningGame, Square position)
        {
            List<(Square, MoveTypes)> Moves = new List<(Square, MoveTypes)>();

            Square counter = position;

            Directions direction = Directions.UpRight;

            while(true)
            {
                switch (direction)
                {
                    case Directions.UpRight:
                        if (counter.Y > 0 && counter.X < owningGame.PieceGrid.GetLength(1) - 1)
                        {
                            counter.Y--;
                            counter.X++;
                        }
                        else
                        {
                            direction++;
                            counter = position;
                            continue;
                        }
                        break;

                    case Directions.UpLeft:
                        if (counter.Y > 0 && counter.X > 0)
                        {
                            counter.Y--;
                            counter.X--;
                        }
                        else
                        {
                            direction++;
                            counter = position;
                            continue;
                        }
                        break;

                    case Directions.DownRight:
                        if (counter.Y < owningGame.PieceGrid.GetLength(0) - 1 && counter.X < owningGame.PieceGrid.GetLength(1) - 1)
                        {
                            counter.Y++;
                            counter.X++;
                        }
                        else
                        {
                            direction++;
                            counter = position;
                            continue;
                        }
                        break;

                    case Directions.DownLeft:
                        if (counter.Y < owningGame.PieceGrid.GetLength(0) - 1 && counter.X > 0)
                        {
                            counter.Y++;
                            counter.X--;
                        }
                        else
                        {
                            return Moves;
                        }
                        break;
                }

                var piece = owningGame.PieceGrid[counter.Y, counter.X];
                if (piece != null && piece.IsWhite == IsWhite)
                {
                    if (direction != Directions.DownLeft)
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
                    if (direction != Directions.DownLeft)
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
        }
    }
}