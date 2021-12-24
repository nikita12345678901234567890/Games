using SharedLibrary;
using SharedLibrary.Pieces;

using System;
using System.Collections.Generic;
using System.Linq;

namespace SharedLibrary
{
    public class CrazyhouseGame : ChessGame
    {
        //number of [pawns, knights, bishops, rooks, queens] available;
        public int[] WhiteAvailablePieces = {1, 2, 3, 4, 5 };
        public int[] BlackAvailablePieces = { 0, 0, 0, 0, 0 };

        public void ResetBoard(Guid playerID)
        {
            if (playerID != whitePlayerID && playerID != blackPlayerID) return;

            for (int x = 0; x < PieceGrid.GetLength(1); x++)
            {
                for (int y = 0; y < PieceGrid.GetLength(0); y++)
                {
                    PieceGrid[y, x] = null;
                }
            }

            //Setting up grid:
            for (int x = 0; x < 8; x++)
            {
                //Black pawns:
                PieceGrid[1, x] = new Pawn(false);

                //White pawns:
                PieceGrid[6, x] = new Pawn(true);
            }
            //Rooks:
            //Black:
            PieceGrid[0, 0] = new Rook(false);
            PieceGrid[0, 7] = new Rook(false);
            //White:
            PieceGrid[7, 0] = new Rook(true);
            PieceGrid[7, 7] = new Rook(true);
            //Knights:
            //Black:
            PieceGrid[0, 1] = new Knight(false);
            PieceGrid[0, 6] = new Knight(false);
            //white:
            PieceGrid[7, 1] = new Knight(true);
            PieceGrid[7, 6] = new Knight(true);
            //Bishops:
            //Black:
            PieceGrid[0, 2] = new Bishop(false);
            PieceGrid[0, 5] = new Bishop(false);
            //White:
            PieceGrid[7, 2] = new Bishop(true);
            PieceGrid[7, 5] = new Bishop(true);
            //Kings:
            //Black:
            PieceGrid[0, 4] = new King(false);
            //White:
            PieceGrid[7, 4] = new King(true);
            //Queens:
            //Black:
            PieceGrid[0, 3] = new Queen(false);
            //White:
            PieceGrid[7, 3] = new Queen(true);

            Whiteturn = true;

            LastMove = default;

            WhiteInCheck = false;

            BlackInCheck = false;

            moveCounter = 0;

            WhiteAvailablePieces = new int[] { 0, 0, 0, 0, 0 };
            BlackAvailablePieces = new int[] { 0, 0, 0, 0, 0 };
    }

        public override string MakeFEN()
        {
            string FEN = "";
            int spaces = 0;

            for (int y = 0; y < PieceGrid.GetLength(0); y++)
            {
                for (int x = 0; x < PieceGrid.GetLength(1); x++)
                {
                    if (PieceGrid[y, x] == null)
                    {
                        spaces++;
                        if (x < 7)
                        {
                            continue;
                        }
                    }

                    if (spaces > 0)
                    {
                        FEN += spaces;
                        spaces = 0;
                    }

                    if (PieceGrid[y, x] == null)
                    {
                        continue;
                    }

                    switch (PieceGrid[y, x].PieceType)
                    {
                        case PieceTypes.Pawn:
                            if (PieceGrid[y, x].IsWhite)
                            {
                                FEN += "P";
                            }
                            else
                            {
                                FEN += "p";
                            }
                            break;

                        case PieceTypes.Knight:
                            if (PieceGrid[y, x].IsWhite)
                            {
                                FEN += "N";
                            }
                            else
                            {
                                FEN += "n";
                            }
                            break;

                        case PieceTypes.Bishop:
                            if (PieceGrid[y, x].IsWhite)
                            {
                                FEN += "B";
                            }
                            else
                            {
                                FEN += "b";
                            }
                            break;

                        case PieceTypes.Rook:
                            if (PieceGrid[y, x].IsWhite)
                            {
                                FEN += "R";
                            }
                            else
                            {
                                FEN += "r";
                            }
                            break;

                        case PieceTypes.King:
                            if (PieceGrid[y, x].IsWhite)
                            {
                                FEN += "K";
                            }
                            else
                            {
                                FEN += "k";
                            }
                            break;

                        case PieceTypes.Queen:
                            if (PieceGrid[y, x].IsWhite)
                            {
                                FEN += "Q";
                            }
                            else
                            {
                                FEN += "q";
                            }
                            break;
                    }
                }
                FEN += "/";
                spaces = 0;
            }

            FEN = FEN[0..^1];

            FEN += " ";
            if (Whiteturn)
            {
                FEN += "w";
            }
            else
            {
                FEN += "b";
            }

            FEN += " ";
            if (WhiteInCheck)
            {
                FEN += "w";
            }
            else if (BlackInCheck)
            {
                FEN += "b";
            }

            FEN += " ";
            if (choosingPromotion)
            {
                FEN += "y";
            }
            else
            {
                FEN += "n";
            }

            for (int i = 0; i < 5; i++)
            {
                FEN += " ";
                FEN += WhiteAvailablePieces[i].ToString();
            }
            for (int i = 0; i < 5; i++)
            {
                FEN += " ";
                FEN += BlackAvailablePieces[i].ToString();
            }

            return FEN;
        }

        public bool PlacePiece(Guid playerID, Piece piece, Square destination)
        {
            if (!ValidPlayer(playerID)) return false;

            //Checking if this will result in a pawn promotion:
            if (piece.PieceType == PieceTypes.Pawn && (destination.Y == 7 || destination.Y == 0)) return false;

            //Checking if there is anything on that square:
            if (PieceGrid[destination.Y, destination.X] != null) return false;

            //Checking if that would result in check:
            if (IsChecking(piece, destination, PieceGrid)) return false;

            PieceGrid[destination.Y, destination.X] = piece;
            return true;
        }
    }
}