using SharedLibrary;
using SharedLibrary.Pieces;

using System;
using System.Collections.Generic;
using System.Linq;

namespace SharedLibrary
{
    /*
     * 
     * 
     * Chris said not to do it this way
     * Ryan did too, but then he changed his mind
     * 
     * 
     */

    //theAverageFlappy
    //theSuperSpeedup
    //un-speedItUp

    //"It's going to crash, but with a different error. This is progress." -Stan 10/14/2021

    //"School really interferes with learning." -Stan 12/7/2021

    public class ChessGame
    {
        //Specifically chess related stuff:

        public Piece[,] PieceGrid = new Piece[8, 8];

        public bool Whiteturn = true;

        public Square LastMove;

        public bool WhiteInCheck = false;

        public bool BlackInCheck = false;

        public int moveCounter = 0;

        public PiecePromotion choices;

        public bool choosingPromotion = false;


        protected Guid whitePlayerID = Guid.Empty;
        protected Guid blackPlayerID = Guid.Empty;

        public bool? GetGameColor(Guid playerID, bool wantsWhite)
        {
            if (wantsWhite)
            {
                if (whitePlayerID == Guid.Empty)
                {
                    whitePlayerID = playerID;
                    return true;
                }
                else if (blackPlayerID == Guid.Empty)
                {
                    blackPlayerID = playerID;
                    return false;
                }
            }
            else
            {
                if (blackPlayerID == Guid.Empty)
                {
                    blackPlayerID = playerID;
                    return false;
                }
                else if (whitePlayerID == Guid.Empty)
                {
                    whitePlayerID = playerID;
                    return true;
                }
            }
            return null;
        } //if desired color is available, gives them that, oitherwise gives them the other color. I both are taken, returns null.





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
        }

        protected bool Contains(List<(Square, MoveTypes)> list, Square pos)
        {
            foreach (var square in list)
            {
                if (square.Item1 == pos)
                {
                    return true;
                }
            }
            return false;
        }

        protected int IndexOf(List<(Square, MoveTypes)> list, Square pos)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Item1 == pos)
                {
                    return i;
                }
            }
            return -1;
        }

        protected bool IsChecking(Piece piece, Square pieceGridPositiion, Piece[,] PieceGrid)
        {
            var movesAndMoveTypes = piece.GetMoves(this, pieceGridPositiion);
            var moves = movesAndMoveTypes.Select((x) => x.Item1).ToList();

            foreach (Square move in moves)
            {
                if (PieceGrid[move.Y, move.X] != null && PieceGrid[move.Y, move.X].IsWhite != piece.IsWhite && PieceGrid[move.Y, move.X].PieceType == PieceTypes.King)
                {
                    return true;
                }
            }

            return false;
        }

        public bool UnderAttack(Square square, bool attackedByWhite, Piece[,] PieceGrid)
        {
            for (int x = 0; x < PieceGrid.GetLength(1); x++)
            {
                for (int y = 0; y < PieceGrid.GetLength(0); y++)
                {
                    Piece piece = PieceGrid[y, x];
                    if (piece != null && piece.IsWhite == attackedByWhite && piece.PieceType != PieceTypes.King && piece.GetMoves(this, new Square(x, y)).Contains((square, MoveTypes.Normal)))
                    {
                        return true;
                    }
                    else if (piece != null && piece.IsWhite == attackedByWhite && piece.PieceType == PieceTypes.King)
                    {
                        King kingInQuestion = (King)piece;

                        if (kingInQuestion.HasMoved && kingInQuestion.GetMoves(this, new Square(x, y)).Contains((square, MoveTypes.Normal)))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public Square[] GetMoves(Square piece)
        {
            if (PieceGrid[piece.Y, piece.X] == null || PieceGrid[piece.Y, piece.X].IsWhite != Whiteturn)
            {
                return null;
            }

            List<Square> moves = new List<Square>
            {
                piece
            };

            List<(Square, MoveTypes)> potentialMoves = PieceGrid[piece.Y, piece.X].GetMoves(this, piece);

            Square position = piece; //The original location of the selected piece

            bool IsWhite = PieceGrid[position.Y, position.X].IsWhite; //The color of the selected piece

            //Treat moves as potential 
            //Then loop through each
            foreach (var potentialMove in potentialMoves)
            {
                bool skipMove = false;

                //Exectuing the move:
                Piece whatWasThere = PieceGrid[potentialMove.Item1.Y, potentialMove.Item1.X];
                PieceGrid[potentialMove.Item1.Y, potentialMove.Item1.X] = PieceGrid[position.Y, position.X];
                PieceGrid[position.Y, position.X] = null;

                //Checking if this pieces color is in check:
                for (int x1 = 0; x1 < 8; x1++)
                {
                    for (int y1 = 0; y1 < 8; y1++)
                    {
                        if (PieceGrid[y1, x1] != null && PieceGrid[y1, x1].IsWhite != IsWhite && PieceGrid[y1, x1].PieceType != PieceTypes.King)
                        {
                            var temp = PieceGrid[y1, x1].GetMoves(this, new Square(x1, y1));
                            foreach (var move in temp)
                            {
                                if (IsChecking(PieceGrid[y1, x1], new Square(x1, y1), PieceGrid))
                                {
                                    skipMove = true;
                                }
                            }
                        }
                    }
                }

                //Checking if this move results in moving a king next to a king:
                if (!skipMove && PieceGrid[potentialMove.Item1.Y, potentialMove.Item1.X].PieceType == PieceTypes.King)
                {
                    var surrounding = PieceGrid[potentialMove.Item1.Y, potentialMove.Item1.X].GetMoves(this, new Square(potentialMove.Item1.X, potentialMove.Item1.Y));
                    foreach (var move in surrounding)
                    {
                        if (PieceGrid[move.Item1.Y, move.Item1.X] != null && PieceGrid[move.Item1.Y, move.Item1.X].PieceType == PieceTypes.King)
                        {
                            skipMove = true;
                        }
                    }
                }

                //Reversing the exectued moves:
                PieceGrid[position.Y, position.X] = PieceGrid[potentialMove.Item1.Y, potentialMove.Item1.X];
                PieceGrid[potentialMove.Item1.Y, potentialMove.Item1.X] = whatWasThere;

                if (!skipMove)
                {
                    moves.Add(new Square(potentialMove.Item1.X, potentialMove.Item1.Y));
                }
            }


            return moves.ToArray();
        }

        public List<(Square, MoveTypes)> GetMovesAndTypes(Square piece)
        {
            List<(Square, MoveTypes)> moves = new List<(Square, MoveTypes)>
            {
                (piece, MoveTypes.None)
            };

            List<(Square, MoveTypes)> potentialMoves = PieceGrid[piece.Y, piece.X].GetMoves(this, piece);

            Square position = piece; //The original location of the selected piece

            bool IsWhite = PieceGrid[position.Y, position.X].IsWhite; //The color of the selected piece

            //Treat moves as potential 
            //Then loop through each
            foreach (var potentialMove in potentialMoves)
            {
                bool skipMove = false;

                //Exectuing the move:
                Piece whatWasThere = PieceGrid[potentialMove.Item1.Y, potentialMove.Item1.X];
                PieceGrid[potentialMove.Item1.Y, potentialMove.Item1.X] = PieceGrid[position.Y, position.X];
                PieceGrid[position.Y, position.X] = null;

                //Checking if this pieces color is in check:
                for (int x1 = 0; x1 < 8; x1++)
                {
                    for (int y1 = 0; y1 < 8; y1++)
                    {
                        if (PieceGrid[y1, x1] != null && PieceGrid[y1, x1].IsWhite != IsWhite && PieceGrid[y1, x1].PieceType != PieceTypes.King)
                        {
                            var temp = PieceGrid[y1, x1].GetMoves(this, new Square(x1, y1));
                            foreach (var move in temp)
                            {
                                if (IsChecking(PieceGrid[y1, x1], new Square(x1, y1), PieceGrid))
                                {
                                    skipMove = true;
                                }
                            }
                        }
                    }
                }

                //Checking if this move results in moving a king next to a king:
                if (!skipMove && PieceGrid[potentialMove.Item1.Y, potentialMove.Item1.X].PieceType == PieceTypes.King)
                {
                    var surrounding = PieceGrid[potentialMove.Item1.Y, potentialMove.Item1.X].GetMoves(this, new Square(potentialMove.Item1.X, potentialMove.Item1.Y));
                    foreach (var move in surrounding)
                    {
                        if (PieceGrid[move.Item1.Y, move.Item1.X] != null && PieceGrid[move.Item1.Y, move.Item1.X].PieceType == PieceTypes.King)
                        {
                            skipMove = true;
                        }
                    }
                }

                //Reversing the exectued moves:
                PieceGrid[position.Y, position.X] = PieceGrid[potentialMove.Item1.Y, potentialMove.Item1.X];
                PieceGrid[potentialMove.Item1.Y, potentialMove.Item1.X] = whatWasThere;

                if (!skipMove)
                {
                    moves.Add((new Square(potentialMove.Item1.X, potentialMove.Item1.Y), potentialMove.Item2));
                }
            }


            return moves;
        }

        public virtual void Move(Guid playerID, Square piece, Square destination)
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

        public bool CheckForNoMoves()
        {
            for (int x = 0; x < PieceGrid.GetLength(1); x++)
            {
                for (int y = 0; y < PieceGrid.GetLength(0); y++)
                {
                    if (PieceGrid[y, x] != null && PieceGrid[y, x].IsWhite == Whiteturn)
                    {
                        if (GetMoves(new Square(x, y)).Length > 1)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
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
            else if(BlackInCheck)
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

        public void Promote(Guid playerID, string pieceChoice)
        {
            if (!ValidPlayer(playerID)) return;

            if (choosingPromotion)
            {
                var promotionInfo = GetPromotionInfo();

                choices = new PiecePromotion(promotionInfo.IsWhite, promotionInfo.pawnLocation.X);

                switch (pieceChoice)
                {
                    case "Queen":
                        PieceGrid[promotionInfo.pawnLocation.Y, promotionInfo.pawnLocation.X] = new Queen(promotionInfo.IsWhite);
                        choosingPromotion = false;
                        Whiteturn = !Whiteturn;
                        break;

                    case "Rook":
                        PieceGrid[promotionInfo.pawnLocation.Y, promotionInfo.pawnLocation.X] = new Rook(promotionInfo.IsWhite);
                        choosingPromotion = false;
                        Whiteturn = !Whiteturn;
                        break;

                    case "Bishop":
                        PieceGrid[promotionInfo.pawnLocation.Y, promotionInfo.pawnLocation.X] = new Bishop(promotionInfo.IsWhite);
                        choosingPromotion = false;
                        Whiteturn = !Whiteturn;
                        break;

                    case "Knight":
                        PieceGrid[promotionInfo.pawnLocation.Y, promotionInfo.pawnLocation.X] = new Knight(promotionInfo.IsWhite);
                        choosingPromotion = false;
                        Whiteturn = !Whiteturn;
                        break;
                }
            }
        }

        private (bool promotion, bool IsWhite, Square pawnLocation) GetPromotionInfo()
        {
            bool promotion = false;
            bool isWhite = false;
            Square pawnLocation = new Square(0, 0);

            for (int x = 0; x < PieceGrid.GetLength(1); x++)
            {
                //Checking for a pawn in the top row:
                if (PieceGrid[0, x] != null && PieceGrid[0, x].PieceType == PieceTypes.Pawn)
                {
                    promotion = true;
                    isWhite = true;
                    pawnLocation = new Square(x, 0);
                }

                //Checking for a pawn in the bottom row:
                if (PieceGrid[PieceGrid.GetLength(0) - 1, x] != null && PieceGrid[PieceGrid.GetLength(0) - 1, x].PieceType == PieceTypes.Pawn)
                {
                    promotion = true;
                    isWhite = false;
                    pawnLocation = new Square(x, 7);
                }
            }

            return (promotion, isWhite, pawnLocation);
        }

        public void CheckPromotion()
        {
            bool promotion = false;

            for (int x = 0; x < PieceGrid.GetLength(1); x++)
            {
                //Checking for a pawn in the top row:
                if (PieceGrid[0, x] != null && PieceGrid[0, x].PieceType == PieceTypes.Pawn)
                {
                    promotion = true;
                }

                //Checking for a pawn in the bottom row:
                if (PieceGrid[PieceGrid.GetLength(0) - 1, x] != null && PieceGrid[PieceGrid.GetLength(0) - 1, x].PieceType == PieceTypes.Pawn)
                {
                    promotion = true;
                }
            }

            choosingPromotion = promotion;
        }




        protected bool ValidPlayer(Guid playerID)
        {
            if ((playerID == whitePlayerID && Whiteturn) || (playerID == blackPlayerID && !Whiteturn))
            {
                return true;
            }
            return false;
        }
    }
}