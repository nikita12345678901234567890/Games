using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.Pieces
{
    public class Pawn : Piece
    {
        public override PieceTypes PieceType => PieceTypes.Pawn;
        public override bool IsWhite { get; set; }

        public bool DidMoveTwice { get; set; }

        public Pawn(bool isWhite)
        {
            IsWhite = isWhite;
        }

        public override List<(Point, MoveTypes)> GetMoves(Piece[,] PieceGrid, Point position)
        {
            /*
            To-Do:
            Add functionality for turning into another piece upon reaching the other side of the board.
            */

            List<(Point, MoveTypes)> Moves = new List<(Point, MoveTypes)>();

            List<(Point, MoveTypes)> potentialMoves = new List<(Point, MoveTypes)>();

            //Moving forward:
            if (IsWhite)
            {
                if (PieceGrid[position.Y - 1, position.X] == null)
                {
                    potentialMoves.Add((new Point(position.X, position.Y - 1), MoveTypes.Normal));
                    if (position.Y == 6 && PieceGrid[position.Y - 2, position.X] == null)
                    {
                        potentialMoves.Add((new Point(position.X, position.Y - 2), MoveTypes.Normal));
                    }
                }
            }
            else
            {
                if (PieceGrid[position.Y + 1, position.X] == null)
                {
                    potentialMoves.Add((new Point(position.X, position.Y + 1), MoveTypes.Normal));
                    if (position.Y == 1 && PieceGrid[position.Y + 2, position.X] == null)
                    {
                        potentialMoves.Add((new Point(position.X, position.Y + 2), MoveTypes.Normal));
                    }
                }
            }

            //Capturing:
            if (IsWhite)
            {
                //Right
                if (position.X < 7 && PieceGrid[position.Y - 1, position.X + 1] != null)
                {
                    var piece = PieceGrid[position.Y - 1, position.X + 1];

                    if (piece == null || (piece != null && !piece.IsWhite))
                    {
                        potentialMoves.Add((new Point(position.X + 1, position.Y - 1), MoveTypes.Normal));
                    }
                }

                //Left:
                if (position.X > 0 && PieceGrid[position.Y - 1, position.X - 1] != null)
                {
                    var piece = PieceGrid[position.Y - 1, position.X - 1];
                    if (piece == null || (piece != null && !piece.IsWhite))
                    {
                        potentialMoves.Add((new Point(position.X - 1, position.Y - 1), MoveTypes.Normal));
                    }
                }

            }
            else
            {
                //Right
                if (position.X < 7 && PieceGrid[position.Y + 1, position.X + 1] != null)
                {
                    var piece = PieceGrid[position.Y + 1, position.X + 1];
                    if (piece == null || (piece != null && piece.IsWhite))
                    {
                        potentialMoves.Add((new Point(position.X + 1, position.Y + 1), MoveTypes.Normal));
                    }
                }

                //Left:
                if (position.X > 0 && PieceGrid[position.Y + 1, position.X - 1] != null)
                {
                    var piece = PieceGrid[position.Y + 1, position.X - 1];
                    if (piece == null || (piece != null && piece.IsWhite))
                    {
                        potentialMoves.Add((new Point(position.X - 1, position.Y + 1), MoveTypes.Normal));
                    }
                }
            }

            //En passant:
            if (IsWhite)
            {
                if (position.X >= 1 && PieceGrid[position.Y, position.X - 1] != null && PieceGrid[position.Y, position.X - 1].PieceType == PieceTypes.Pawn && Game1.LastMove == new Point(position.X - 1, position.Y))
                {
                    Pawn pawn = (Pawn)PieceGrid[position.Y, position.X - 1];
                    if (pawn.DidMoveTwice)
                    {
                        potentialMoves.Add((new Point(position.X - 1, position.Y - 1), MoveTypes.EnPassant));
                    }
                }

                if (position.X <= 6 && PieceGrid[position.Y, position.X + 1] != null && PieceGrid[position.Y, position.X + 1].PieceType == PieceTypes.Pawn && Game1.LastMove == new Point(position.X + 1, position.Y))
                {
                    Pawn pawn = (Pawn)PieceGrid[position.Y, position.X + 1];
                    if (pawn.DidMoveTwice)
                    {
                        potentialMoves.Add((new Point(position.X + 1, position.Y - 1), MoveTypes.EnPassant));
                    }
                }
            }
            else
            {
                if (position.X >= 1 && PieceGrid[position.Y, position.X - 1] != null && PieceGrid[position.Y, position.X - 1].PieceType == PieceTypes.Pawn && Game1.LastMove == new Point(position.X - 1, position.Y))
                {
                    Pawn pawn = (Pawn)PieceGrid[position.Y, position.X - 1];
                    if (pawn.DidMoveTwice)
                    {
                        potentialMoves.Add((new Point(position.X - 1, position.Y + 1), MoveTypes.EnPassant));
                    }
                }

                if (position.X <= 6 && PieceGrid[position.Y, position.X + 1] != null && PieceGrid[position.Y, position.X + 1].PieceType == PieceTypes.Pawn && Game1.LastMove == new Point(position.X + 1, position.Y))
                {
                    Pawn pawn = (Pawn)PieceGrid[position.Y, position.X + 1];
                    if (pawn.DidMoveTwice)
                    {
                        potentialMoves.Add((new Point(position.X + 1, position.Y + 1), MoveTypes.EnPassant));
                    }
                }
            }

            foreach (var move in potentialMoves)
            {
                bool skipMove = false;

                //Exectuing the move:
                Piece whatWasThere = PieceGrid[move.Item1.Y, move.Item1.X];
                PieceGrid[move.Item1.Y, move.Item1.X] = PieceGrid[position.Y, position.X];
                PieceGrid[position.Y, position.X] = null;

                //Checking if this pieces color is in check:
                for (int x1 = 0; x1 < 8; x1++)
                {
                    for (int y1 = 0; y1 < 8; y1++)
                    {
                        if (PieceGrid[y1, x1] != null && PieceGrid[y1, x1].IsWhite != IsWhite && PieceGrid[y1, x1].PieceType != PieceTypes.Pawn && Game1.IsChecking(PieceGrid[y1, x1], new Point(x1, y1), PieceGrid))
                        {
                            skipMove = true;
                        }
                    }
                }

                //Reversing the exectued moves:
                PieceGrid[position.Y, position.X] = PieceGrid[move.Item1.Y, move.Item1.X];
                PieceGrid[move.Item1.Y, move.Item1.X] = whatWasThere;

                if (!skipMove)
                {
                    Moves.Add((new Point(move.Item1.X, move.Item1.Y), MoveTypes.Normal));
                }
            }

            return Moves;
        }
    }
}