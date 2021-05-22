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

        public Pawn(bool isWhite)
        {
            IsWhite = isWhite;
        }
        public override List<Point> GetMoves(Piece[,] PieceGrid, Point position)
        {
            /*
            To-Do:
            Add en passant(or however you spell that)
            Add functionality for turning into another piece upon reaching the other side of the board.
            */

            List<Point> Moves = new List<Point>();

            //Moving forward:
            if (IsWhite)
            {
                if (PieceGrid[position.Y - 1, position.X] == null)
                {
                    Moves.Add(new Point(position.X, position.Y - 1));
                    if (position.Y == 6 && PieceGrid[position.Y - 2, position.X] == null)
                    {
                        Moves.Add(new Point(position.X, position.Y - 2));
                    }
                }
            }
            else
            {
                if (PieceGrid[position.Y + 1, position.X] == null)
                {
                    Moves.Add(new Point(position.X, position.Y + 1));
                    if (position.Y == 1 && PieceGrid[position.Y + 2, position.X] == null)
                    {
                        Moves.Add(new Point(position.X, position.Y + 2));
                    }
                }
            }

            //Capturing:
            if (IsWhite)
            {
                //Right
                if (position.X < 7 && PieceGrid[position.Y - 1, position.X + 1] != null)
                {
                    Moves.Add(new Point(position.X + 1, position.Y - 1));
                }

                //Left:
                if (position.X > 0 && PieceGrid[position.Y - 1, position.X - 1] != null)
                {
                    Moves.Add(new Point(position.X - 1, position.Y - 1));
                }

            }
            else
            {
                //Right
                if (position.X < 7 && PieceGrid[position.Y + 1, position.X + 1] != null)
                {
                    Moves.Add(new Point(position.X + 1, position.Y + 1));
                }

                //Left:
                if (position.X > 0 && PieceGrid[position.Y + 1, position.X - 1] != null)
                {
                    Moves.Add(new Point(position.X - 1, position.Y + 1));
                }
            }

            return Moves;
        }
    }
}