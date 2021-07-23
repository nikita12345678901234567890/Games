using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
     * Ryan did too
     * 
     * 
     */
    
    public static class Class1
    {
        public static Texture2D Pixel;

        //Specifically chess related stuff:

        public static Piece[,] PieceGrid = new Piece[8, 8];

        public static bool Whiteturn = true;

        public static Point LastMove;

        public static bool WhiteInCheck = false;

        public static bool BlackInCheck = false;

        public static void ResetBoard()
        {
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
        }

        public static bool Contains(List<(Point, MoveTypes)> list, Point pos)
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

        public static int IndexOf(List<(Point, MoveTypes)> list, Point pos)
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

        public static bool IsChecking(Piece piece, Point pieceGridPositiion, Piece[,] PieceGrid)
        {
            var movesAndMoveTypes = piece.GetMoves(PieceGrid, pieceGridPositiion);
            var moves = movesAndMoveTypes.Select((x) => x.Item1).ToList();

            foreach (Point move in moves)
            {
                if (PieceGrid[move.Y, move.X] != null && PieceGrid[move.Y, move.X].IsWhite != piece.IsWhite && PieceGrid[move.Y, move.X].PieceType == PieceTypes.King)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool UnderAttack(Point square, bool attackedByWhite, Piece[,] PieceGrid)
        {
            for (int x = 0; x < PieceGrid.GetLength(1); x++)
            {
                for (int y = 0; y < PieceGrid.GetLength(0); y++)
                {
                    Piece piece = PieceGrid[y, x];
                    if (piece != null && piece.IsWhite == attackedByWhite && piece.PieceType != PieceTypes.King && piece.GetMoves(PieceGrid, new Point(x, y)).Contains((square, MoveTypes.Normal)))
                    {
                        return true;
                    }
                    else if (piece != null && piece.IsWhite == attackedByWhite && piece.PieceType == PieceTypes.King)
                    {
                        King kingInQuestion = (King)piece;

                        if (kingInQuestion.HasMoved && kingInQuestion.GetMoves(PieceGrid, new Point(x, y)).Contains((square, MoveTypes.Normal)))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static Point[] GetMoves(Point piece)
        {
            if (PieceGrid[piece.Y, piece.X] == null || PieceGrid[piece.Y, piece.X].IsWhite != Whiteturn)
            {
                return null;
            }

            List<Point> moves = new List<Point>();

            moves.Add(piece);

            List<(Point, MoveTypes)> potentialMoves = PieceGrid[piece.Y, piece.X].GetMoves(PieceGrid, piece);

            Point position = piece; //The original location of the selected piece

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
                            var temp = PieceGrid[y1, x1].GetMoves(PieceGrid, new Point(x1, y1));
                            foreach (var move in temp)
                            {
                                if (IsChecking(PieceGrid[y1, x1], new Point(x1, y1), PieceGrid))
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
                    var surrounding = PieceGrid[potentialMove.Item1.Y, potentialMove.Item1.X].GetMoves(PieceGrid, new Point(potentialMove.Item1.X, potentialMove.Item1.Y));
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
                    moves.Add(new Point(potentialMove.Item1.X, potentialMove.Item1.Y));
                }
            }


            return moves.ToArray();
        }

        public static List<(Point, MoveTypes)> GetMovesAndTypes(Point piece)
        {
            List<(Point, MoveTypes)> moves = new List<(Point, MoveTypes)>();

            moves.Add((piece, MoveTypes.None));

            List<(Point, MoveTypes)> potentialMoves = PieceGrid[piece.Y, piece.X].GetMoves(PieceGrid, piece);

            Point position = piece; //The original location of the selected piece

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
                            var temp = PieceGrid[y1, x1].GetMoves(PieceGrid, new Point(x1, y1));
                            foreach (var move in temp)
                            {
                                if (IsChecking(PieceGrid[y1, x1], new Point(x1, y1), PieceGrid))
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
                    var surrounding = PieceGrid[potentialMove.Item1.Y, potentialMove.Item1.X].GetMoves(PieceGrid, new Point(potentialMove.Item1.X, potentialMove.Item1.Y));
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
                    moves.Add((new Point(potentialMove.Item1.X, potentialMove.Item1.Y), potentialMove.Item2));
                }
            }


            return moves;
        }

        public static void Move(Point piece, Point destination)
        {
            var moves = GetMovesAndTypes(piece);
            if (Contains(moves, destination))
            {
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
                Whiteturn = !Whiteturn;

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
            }
        }


        public static string MakeFEN(Piece[,] PieceGrid)
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
                        continue;
                    }

                    if (spaces > 0)
                    {
                        FEN += spaces;
                        spaces = 0;
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
            }

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
            else
            {
                FEN += "-";
            }

            return FEN;
        }
    }
}
