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

            for (int x = position.X - 1; x < position.X + 1; x++)
            {
                for (int y = position.Y - 1; y < position.Y + 1; y++)
                {
                    if(x >= 0 && x <= 7 && y >= 0 && y <= 7)
                    {
                        Piece piece = PieceGrid[y, x];
                        if (piece == null || (piece != null && piece.IsWhite != IsWhite))
                        {
                            if (IsWhite)
                            {
                                if (Game1.WhiteInCheck)
                                {
                                    //Exectuing the move:
                                    PieceGrid[mouseCell.Y, mouseCell.X] = PieceGrid[HighlightedSquares[0].location.Y, HighlightedSquares[0].location.X];
                                    PieceGrid[HighlightedSquares[0].location.Y, HighlightedSquares[0].location.X] = null;

                                    //Checking if that stopped the check:


                                    //Reversing the exectued move and adding it to Moves:

                                }
                            }
                            else
                            {
                                if (Game1.BlackInCheck)
                                { 
                                
                                }
                            }
                            Moves.Add((new Point(x, y), MoveTypes.Normal));
                        }
                    }
                }
            }

            return Moves;
        }
    }
}