using SharedLibrary;
using SharedLibrary.Pieces;

using System;
using System.Collections.Generic;
using System.Linq;

namespace SharedLibrary
{
    public class CrazyhouseGame : ChessGame
    {
        public Dictionary<bool, Dictionary<PieceTypes, Stack<Piece>>> AvailablePieces = new Dictionary<bool, Dictionary<PieceTypes, Stack<Piece>>>();

        public override void ResetBoard(Guid playerID)
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

            
            //Initializing the Dictionary of Dictionary of Stack of Piece:
            if (AvailablePieces.Keys.Count == 0)
            {
                AvailablePieces.Add(true, new Dictionary<PieceTypes, Stack<Piece>>());
                AvailablePieces.Add(false, new Dictionary<PieceTypes, Stack<Piece>>());


                AvailablePieces[true].Add(PieceTypes.Pawn, new Stack<Piece>());
                AvailablePieces[true].Add(PieceTypes.Knight, new Stack<Piece>());
                AvailablePieces[true].Add(PieceTypes.Bishop, new Stack<Piece>());
                AvailablePieces[true].Add(PieceTypes.Rook, new Stack<Piece>());
                AvailablePieces[true].Add(PieceTypes.Queen, new Stack<Piece>());

                AvailablePieces[false].Add(PieceTypes.Pawn, new Stack<Piece>());
                AvailablePieces[false].Add(PieceTypes.Knight, new Stack<Piece>());
                AvailablePieces[false].Add(PieceTypes.Bishop, new Stack<Piece>());
                AvailablePieces[false].Add(PieceTypes.Rook, new Stack<Piece>());
                AvailablePieces[false].Add(PieceTypes.Queen, new Stack<Piece>());
            }
        }

        public override void Move(Guid playerID, Square piece, Square destination)
        {
            if (!ValidPlayer(playerID)) return;

            var moves = GetMovesAndTypes(piece);
            if (Contains(moves, destination))
            {
                Piece capturedPiece = null;

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

                //Saving the piece that was there:
                if (PieceGrid[destination.Y, destination.X] != null)
                {
                    capturedPiece = PieceGrid[destination.Y, destination.X];
                }

                //Actually doing the move:
                PieceGrid[destination.Y, destination.X] = PieceGrid[piece.Y, piece.X];
                PieceGrid[piece.Y, piece.X] = null;


                bool IsPromotion = false;

                switch (moves[IndexOf(moves, destination)].Item2)
                {
                    case MoveTypes.EnPassant:

                        if (PieceGrid[destination.Y, destination.X].IsWhite)
                        {
                            if (PieceGrid[destination.Y + 1, destination.X] != null)
                            {
                                capturedPiece = PieceGrid[destination.Y + 1, destination.X];
                                PieceGrid[destination.Y + 1, destination.X] = null;
                            }
                        }
                        else
                        {
                            if (PieceGrid[destination.Y - 1, destination.X] != null)
                            {
                                capturedPiece = PieceGrid[destination.Y - 1, destination.X];
                                PieceGrid[destination.Y - 1, destination.X] = null;
                            }
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

                if (capturedPiece != null)
                {
                    capturedPiece.IsWhite = !capturedPiece.IsWhite;
                    AvailablePieces[Whiteturn][capturedPiece.PieceType].Push(capturedPiece);
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

            FEN += " ";
            FEN += AvailablePieces[true][PieceTypes.Pawn].Count();
            FEN += ",";
            FEN += AvailablePieces[true][PieceTypes.Knight].Count();
            FEN += ",";
            FEN += AvailablePieces[true][PieceTypes.Bishop].Count();
            FEN += ",";
            FEN += AvailablePieces[true][PieceTypes.Rook].Count();
            FEN += ",";
            FEN += AvailablePieces[true][PieceTypes.Queen].Count();
            FEN += ",";

            FEN += AvailablePieces[false][PieceTypes.Pawn].Count();
            FEN += ",";
            FEN += AvailablePieces[false][PieceTypes.Knight].Count();
            FEN += ",";
            FEN += AvailablePieces[false][PieceTypes.Bishop].Count();
            FEN += ",";
            FEN += AvailablePieces[false][PieceTypes.Rook].Count();
            FEN += ",";
            FEN += AvailablePieces[false][PieceTypes.Queen].Count();
            FEN += ",";

            return FEN;
        }

        public bool PlacePiece(Guid playerID, PieceTypes piece, Square destination)
        {
            if (!ValidPlayer(playerID)) return false;

            if (AvailablePieces[Whiteturn][piece].Count == 0) return false;

            //Checking if this will result in a pawn promotion:
            if (piece == PieceTypes.Pawn && (destination.Y == 7 || destination.Y == 0)) return false;

            //Checking if there is anything on that square:
            if (PieceGrid[destination.Y, destination.X] != null) return false;

            PieceGrid[destination.Y, destination.X] = AvailablePieces[Whiteturn][piece].Pop();

            //Looping through the whole board to find the king of the side that just moved:
            for (int x = 0; x < PieceGrid.GetLength(1); x++)
            {
                for (int y = 0; y < PieceGrid.GetLength(0); y++)
                {
                    Piece Piece = PieceGrid[y, x];
                    if (Piece != null && Piece.IsWhite == Whiteturn && Piece.PieceType == PieceTypes.King)
                    {
                        if (UnderAttack(new Square(x, y), !Whiteturn, PieceGrid))
                        {
                            AvailablePieces[Whiteturn][piece].Push(PieceGrid[destination.Y, destination.X]);
                            PieceGrid[destination.Y, destination.X] = null;
                            return false;
                        }
                        if (Whiteturn)
                        {
                            WhiteInCheck = false;
                        }
                        else
                        {
                            BlackInCheck = false;
                        }
                    }

                    if (Piece != null && Piece.IsWhite == !Whiteturn && Piece.PieceType == PieceTypes.King)
                    {
                        if (UnderAttack(new Square(x, y), Whiteturn, PieceGrid))
                        {
                            if (Whiteturn)
                            {
                                BlackInCheck = true;
                            }
                            else
                            {
                                WhiteInCheck = true;
                            }
                        }
                    }
                }
            }

            LastMove = new Square(destination.X, destination.Y);
            Whiteturn = !Whiteturn;
            return true;
        }
    }
}