﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.Pieces
{
    class King : Piece
    {
        public override PieceTypes PieceType => PieceTypes.King;
        public override bool IsWhite { get; set; }

        public bool HasMoved { get; set; }

        public King(bool isWhite)
        {
            IsWhite = isWhite;

            HasMoved = false;
        }

        public override List<(Point, MoveTypes)> GetMoves(Piece[,] PieceGrid, Point position)
        {
            List<(Point, MoveTypes)> Moves = new List<(Point, MoveTypes)>();

            //Looping through the 3x3 of surrounding tiles:
            for (int x = position.X - 1; x <= position.X + 1; x++)
            {
                for (int y = position.Y - 1; y <= position.Y + 1; y++)
                {
                    if (x >= 0 && x < PieceGrid.GetLength(1) && y >= 0 && y < PieceGrid.GetLength(0))
                    {
                        Piece piece = PieceGrid[y, x];

                        if (piece == null || (piece != null && piece.IsWhite == !IsWhite))
                        {
                            Moves.Add((new Point(x, y), MoveTypes.Normal));
                        }
                    }
                }
            }

            //Castling:
            bool inCheck = false;
            if (IsWhite && Game1.WhiteInCheck)
            {
                inCheck = true;
            }
            if (!IsWhite && Game1.BlackInCheck)
            {
                inCheck = true;
            }

            if (!HasMoved && !inCheck)
            {
                //Checking if there's a rook of the same color in the left corner:
                var leftCorner = PieceGrid[position.Y, 0];
                if (leftCorner != null && leftCorner.IsWhite == IsWhite && leftCorner.PieceType == PieceTypes.Rook)
                {
                    //Checking if that rook hasn't moved and that the spaces between the king and the rook are empty:
                    Rook leftRook = (Rook)leftCorner;

                    if (!leftRook.HasMoved && PieceGrid[position.Y, position.X - 1] == null && PieceGrid[position.Y, position.X - 2] == null)
                    {
                        //Checking that the squares the king passes through aren't under attack:
                        if (!Game1.UnderAttack(new Point(position.X - 1, position.Y), !IsWhite, PieceGrid) && !Game1.UnderAttack(new Point(position.X - 2, position.Y), !IsWhite, PieceGrid))
                        {
                            Moves.Add((new Point(position.X - 2, position.Y), MoveTypes.CastleLeft));
                        }
                    }
                }

                //Checking if there's a rook of the same color in the right corner:
                var rightCorner = PieceGrid[position.Y, 0];
                if (rightCorner != null && rightCorner.IsWhite == IsWhite && rightCorner.PieceType == PieceTypes.Rook)
                {
                    //Checking if that rook hasn't moved and that the spaces between the king and the rook are empty:
                    Rook rightRook = (Rook)rightCorner;
                    if (!rightRook.HasMoved && PieceGrid[position.Y, position.X + 1] == null && PieceGrid[position.Y, position.X + 2] == null)
                    {
                        //Checking that the squares the king passes through aren't under attack:
                        if (!Game1.UnderAttack(new Point(position.X + 1, position.Y), !IsWhite, PieceGrid) && !Game1.UnderAttack(new Point(position.X + 2, position.Y), !IsWhite, PieceGrid))
                        {
                            Moves.Add((new Point(position.X + 2, position.Y), MoveTypes.CastleRight));
                        }
                    }
                }
            }

            return Moves;
        }
    }
}