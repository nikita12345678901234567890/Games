﻿using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary.Pieces
{
    public class Pawn : Piece
    {
        public override PieceTypes PieceType => PieceTypes.Pawn;
        public override bool IsWhite { get; set; }

        public bool DidMoveTwice { get; set; }

        public Pawn(bool isWhite)
        {
            IsWhite = isWhite;

            DidMoveTwice = false;
        }

        public override List<(Point, MoveTypes)> GetMoves(Piece[,] PieceGrid, Point position)
        {
            /*
            To-Do:
            Add functionality for turning into another piece upon reaching the other side of the board.
            */

            List<(Point, MoveTypes)> Moves = new List<(Point, MoveTypes)>();

            //Moving forward:
            if (IsWhite)
            {
                if (position.Y - 1 >= 0 && PieceGrid[position.Y - 1, position.X] == null)
                {
                    if (position.Y - 1 > 0)
                    {
                        Moves.Add((new Point(position.X, position.Y - 1), MoveTypes.Normal));
                    }
                    else
                    {
                        Moves.Add((new Point(position.X, position.Y - 1), MoveTypes.Promotion));
                    }

                    if (position.Y == 6 && PieceGrid[position.Y - 2, position.X] == null)
                    {
                        Moves.Add((new Point(position.X, position.Y - 2), MoveTypes.Normal));
                    }
                }
            }
            else
            {
                if (position.Y + 1 < PieceGrid.GetLength(0) && PieceGrid[position.Y + 1, position.X] == null)
                {
                    if (position.Y + 1 < PieceGrid.GetLength(0) - 1)
                    {
                        Moves.Add((new Point(position.X, position.Y + 1), MoveTypes.Normal));
                    }
                    else
                    {
                        Moves.Add((new Point(position.X, position.Y + 1), MoveTypes.Promotion));
                    }

                    if (position.Y == 1 && PieceGrid[position.Y + 2, position.X] == null)
                    {
                        Moves.Add((new Point(position.X, position.Y + 2), MoveTypes.Normal));
                    }
                }
            }

            //Capturing:
            if (IsWhite)
            {
                //Right
                if (position.X < 7 && position.Y - 1 >= 0 && PieceGrid[position.Y - 1, position.X + 1] != null)
                {
                    var piece = PieceGrid[position.Y - 1, position.X + 1];

                    if (piece == null || (piece != null && !piece.IsWhite))
                    {
                        if (position.Y - 1 > 0)
                        {
                            Moves.Add((new Point(position.X + 1, position.Y - 1), MoveTypes.Normal));
                        }
                        else
                        {
                            Moves.Add((new Point(position.X + 1, position.Y - 1), MoveTypes.Promotion));
                        }
                    }
                }

                //Left:
                if (position.X > 0 && position.Y - 1 >= 0 && PieceGrid[position.Y - 1, position.X - 1] != null)
                {
                    var piece = PieceGrid[position.Y - 1, position.X - 1];
                    if (piece == null || (piece != null && !piece.IsWhite))
                    {
                        if (position.Y - 1 > 0)
                        {
                            Moves.Add((new Point(position.X - 1, position.Y - 1), MoveTypes.Normal));
                        }
                        else
                        {
                            Moves.Add((new Point(position.X - 1, position.Y - 1), MoveTypes.Promotion));
                        }
                    }
                }

            }
            else
            {
                //Right
                if (position.X < 7 && position.Y + 1 < PieceGrid.GetLength(0) && PieceGrid[position.Y + 1, position.X + 1] != null)
                {
                    var piece = PieceGrid[position.Y + 1, position.X + 1];
                    if (piece == null || (piece != null && piece.IsWhite))
                    {
                        if (position.Y + 1 < PieceGrid.GetLength(0) - 1)
                        {
                            Moves.Add((new Point(position.X + 1, position.Y + 1), MoveTypes.Normal));
                        }
                        else
                        {
                            Moves.Add((new Point(position.X + 1, position.Y + 1), MoveTypes.Promotion));
                        }
                    }
                }

                //Left:
                if (position.X > 0 && position.Y + 1 < PieceGrid.GetLength(0) && PieceGrid[position.Y + 1, position.X - 1] != null)
                {
                    var piece = PieceGrid[position.Y + 1, position.X - 1];
                    if (piece == null || (piece != null && piece.IsWhite))
                    {
                        if (position.Y + 1 < PieceGrid.GetLength(0) - 1)
                        {
                            Moves.Add((new Point(position.X - 1, position.Y + 1), MoveTypes.Normal));
                        }
                        else
                        {
                            Moves.Add((new Point(position.X - 1, position.Y + 1), MoveTypes.Promotion));
                        }
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
                        Moves.Add((new Point(position.X - 1, position.Y - 1), MoveTypes.EnPassant));
                    }
                }

                if (position.X <= 6 && PieceGrid[position.Y, position.X + 1] != null && PieceGrid[position.Y, position.X + 1].PieceType == PieceTypes.Pawn && Game1.LastMove == new Point(position.X + 1, position.Y))
                {
                    Pawn pawn = (Pawn)PieceGrid[position.Y, position.X + 1];
                    if (pawn.DidMoveTwice)
                    {
                        Moves.Add((new Point(position.X + 1, position.Y - 1), MoveTypes.EnPassant));
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
                        Moves.Add((new Point(position.X - 1, position.Y + 1), MoveTypes.EnPassant));
                    }
                }

                if (position.X <= 6 && PieceGrid[position.Y, position.X + 1] != null && PieceGrid[position.Y, position.X + 1].PieceType == PieceTypes.Pawn && Game1.LastMove == new Point(position.X + 1, position.Y))
                {
                    Pawn pawn = (Pawn)PieceGrid[position.Y, position.X + 1];
                    if (pawn.DidMoveTwice)
                    {
                        Moves.Add((new Point(position.X + 1, position.Y + 1), MoveTypes.EnPassant));
                    }
                }
            }

            return Moves;
        }
    }
}