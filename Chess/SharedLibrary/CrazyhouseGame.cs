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
        public int[] WhiteAvailablePieces = {0, 0, 0, 0, 0 };
        public int[] BlackAvailablePieces = { 0, 0, 0, 0, 0 };

        public override void Move(Guid playerID, Square piece, Square destination)
        {
            if (!ValidPlayer(playerID)) return;

            var moves = GetMovesAndTypes(piece);
            if (Contains(moves, destination))
            {
                //Checks for reseting the moveCounter
                if (PieceGrid[piece.Y, piece.X].PieceType == PieceTypes.Pawn || PieceGrid[destination.Y, destination.X] != null)
                {
                    moveCounter = 0;
                }


                //Setting DidMoveTwice:
                if (moves.Count >= 3 && destination == moves[2].Item1 && PieceGrid[piece.Y, piece.X] != null && PieceGrid[piece.Y, piece.X].PieceType == PieceTypes.Pawn)
                {
                    Pawn pawn = (Pawn)PieceGrid[piece.Y, piece.X];
                    pawn.DidMoveTwice = true;
                }

                PieceGrid[destination.Y, destination.X] = PieceGrid[piece.Y, piece.X];
                PieceGrid[piece.Y, piece.X] = null;

                bool IsPromotion = false;

                switch (moves[IndexOf(moves, destination)].Item2)
                {
                    case MoveTypes.EnPassant:

                        if (PieceGrid[destination.Y, destination.X].IsWhite)
                        {
                            PieceGrid[destination.Y + 1, destination.X] = null;
                        }
                        else
                        {
                            PieceGrid[destination.Y - 1, destination.X] = null;
                        }

                        break;

                    case MoveTypes.CastleLeft:

                        PieceGrid[destination.Y, destination.X + 1] = PieceGrid[destination.Y, 0];
                        PieceGrid[destination.Y, 0] = null;

                        break;

                    case MoveTypes.CastleRight:

                        PieceGrid[destination.Y, destination.X - 1] = PieceGrid[destination.Y, PieceGrid.GetLength(1) - 1];
                        PieceGrid[destination.Y, PieceGrid.GetLength(1) - 1] = null;

                        break;

                    case MoveTypes.Promotion:

                        IsPromotion = true;

                        break;
                }

                LastMove = destination;

                //Setting HasMoved:
                var lastMovedPiece = PieceGrid[LastMove.Y, LastMove.X];
                if (lastMovedPiece.PieceType == PieceTypes.King)
                {
                    King kingMoved = (King)lastMovedPiece;
                    kingMoved.HasMoved = true;
                }
                if (lastMovedPiece.PieceType == PieceTypes.Rook)
                {
                    Rook rookMoved = (Rook)lastMovedPiece;
                    rookMoved.HasMoved = true;
                }

                if (!IsPromotion)
                {
                    Whiteturn = !Whiteturn;

                    //Checking if last move put someone in check:
                    if (IsChecking(PieceGrid[LastMove.Y, LastMove.X], LastMove, PieceGrid))
                    {
                        if (PieceGrid[LastMove.Y, LastMove.X].IsWhite)
                        {
                            BlackInCheck = true;
                        }
                        else
                        {
                            WhiteInCheck = true;
                        }
                    }
                    else
                    {
                        WhiteInCheck = false;
                        BlackInCheck = false;
                    }
                }


                moveCounter++;
            }
        }

        public virtual string MakeFEN()
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

            return FEN;
        }
    }
}