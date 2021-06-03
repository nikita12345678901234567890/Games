using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.Pieces
{
    class King : Piece
    {
        public override PieceTypes PieceType => PieceTypes.King;
        public override bool IsWhite { get; set; }

        public King(bool isWhite)
        {
            IsWhite = isWhite;
        }

        public override List<(Point, MoveTypes)> GetMoves(Piece[,] PieceGrid, Point position)
        {
            List<(Point, MoveTypes)> Moves = new List<(Point, MoveTypes)>();

            //Looping through the 3x3 of surrounding tiles:
            for (int x = position.X - 1; x <= position.X + 1; x++)
            {
                for (int y = position.Y - 1; y <= position.Y + 1; y++)
                {
                    //Skipping the ones that are out of bounds:
                    if(x >= 0 && x <= 7 && y >= 0 && y <= 7)
                    {
                        Piece piece = PieceGrid[y, x];
                        //Skipping the ones that have a piece of the same color:
                        if (piece == null || (piece != null && piece.IsWhite != IsWhite))
                        {
                            bool skipMove = false;
                            //Checking the color to know what color to check for being in check:
                            if (IsWhite)
                            {
                                if (Game1.WhiteInCheck)
                                {
                                    //Exectuing the move:
                                    PieceGrid[y, x] = PieceGrid[position.Y, position.X];
                                    PieceGrid[position.Y, position.X] = null;

                                    //Checking if that stopped the check:
                                    for (int x1 = 0; x1 < 8; x1++)
                                    {
                                        for (int y1 = 0; y1 < 8; y1++)
                                        {
                                            if (PieceGrid[y1, x1] != null && Game1.IsChecking(PieceGrid[y1, x1], new Point(x1, y1), PieceGrid))
                                            {
                                                skipMove = true;
                                            }
                                        }
                                    }

                                    //Reversing the exectued move and adding it to Moves:
                                    PieceGrid[position.Y, position.X] = PieceGrid[y, x];
                                    PieceGrid[y, x] = null;
                                }
                            }
                            else
                            {
                                if (Game1.BlackInCheck)
                                {
                                    //Exectuing the move:
                                    PieceGrid[y, x] = PieceGrid[position.Y, position.X];
                                    PieceGrid[position.Y, position.X] = null;

                                    //Checking if that stopped the check:
                                    for (int x1 = 0; x1 < 8; x1++)
                                    {
                                        for (int y1 = 0; y1 < 8; y1++)
                                        {
                                            if (PieceGrid[y1, x1] != null && PieceGrid[y1, x1].IsWhite != IsWhite && Game1.IsChecking(PieceGrid[y1, x1], new Point(x1, y1), PieceGrid))
                                            {
                                                skipMove = true;
                                            }
                                        }
                                    }

                                    //Reversing the exectued move and adding it to Moves:
                                    PieceGrid[position.Y, position.X] = PieceGrid[y, x];
                                    PieceGrid[y, x] = null;
                                }
                            }
                            if (!skipMove)
                            {
                                Moves.Add((new Point(x, y), MoveTypes.Normal));
                            }
                        }
                    }
                }
            }

            return Moves;
        }
    }
}