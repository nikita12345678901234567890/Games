using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
     * 
     * Chris said not to do it this way
     * 
     * 
     * 
     */
    public class Class1
    {
        //Specifically chess related stuff:
        public int squaresize;

        public Piece[,] PieceGrid = new Piece[8, 8];

        public List<(Point location, MoveTypes)> HighlightedSquares;

        public bool Whiteturn = true;

        public static Point LastMove;

        public static bool WhiteInCheck = false;

        public static bool BlackInCheck = false;

        public bool Promotion = false;

        public Point queen;
        public Point rook;
        public Point bishop;
        public Point knight;

        public void Load()
        {
            HighlightedSquares = new List<(Point, MoveTypes)>();


        }

        public void Update(GameTime gameTime)
        {
            //Checking if mouse clicked:
            if (ms.LeftButton == ButtonState.Pressed && Lastms.LeftButton == ButtonState.Released && GraphicsDevice.Viewport.Bounds.Contains(ms.Position))
            {
                var mouseCell = PositionToCell(ms.Position);
                if (!Promotion)
                {
                    //Moving:
                    if (Contains(mouseCell) && HighlightedSquares[0].location != mouseCell)
                    {
                        //Setting DidMoveTwice:
                        if (HighlightedSquares.Count >= 3 && mouseCell == HighlightedSquares[2].location && PieceGrid[HighlightedSquares[0].location.Y, HighlightedSquares[0].location.X] != null && PieceGrid[HighlightedSquares[0].location.Y, HighlightedSquares[0].location.X].PieceType == PieceTypes.Pawn)
                        {
                            Pawn pawn = (Pawn)PieceGrid[HighlightedSquares[0].location.Y, HighlightedSquares[0].location.X];
                            pawn.DidMoveTwice = true;
                        }

                        PieceGrid[mouseCell.Y, mouseCell.X] = PieceGrid[HighlightedSquares[0].location.Y, HighlightedSquares[0].location.X];
                        PieceGrid[HighlightedSquares[0].location.Y, HighlightedSquares[0].location.X] = null;

                        switch (HighlightedSquares[IndexOf(mouseCell)].Item2)
                        {
                            case MoveTypes.EnPassant:

                                if (PieceGrid[mouseCell.Y, mouseCell.X].IsWhite)
                                {
                                    PieceGrid[mouseCell.Y + 1, mouseCell.X] = null;
                                }
                                else
                                {
                                    PieceGrid[mouseCell.Y - 1, mouseCell.X] = null;
                                }

                                break;

                            case MoveTypes.CastleLeft:

                                PieceGrid[mouseCell.Y, mouseCell.X + 1] = PieceGrid[mouseCell.Y, 0];
                                PieceGrid[mouseCell.Y, 0] = null;

                                break;

                            case MoveTypes.CastleRight:

                                PieceGrid[mouseCell.Y, mouseCell.X - 1] = PieceGrid[mouseCell.Y, PieceGrid.GetLength(1) - 1];
                                PieceGrid[mouseCell.Y, PieceGrid.GetLength(1) - 1] = null;

                                break;

                            case MoveTypes.Promotion:

                                Promotion = true;
                                if (PieceGrid[mouseCell.Y, mouseCell.X].IsWhite)
                                {
                                    queen = mouseCell;
                                    rook = new Point(mouseCell.X, mouseCell.Y + 1);
                                    bishop = new Point(mouseCell.X, mouseCell.Y + 2);
                                    knight = new Point(mouseCell.X, mouseCell.Y + 3);
                                }
                                else
                                {
                                    queen = mouseCell;
                                    rook = new Point(mouseCell.X, mouseCell.Y - 1);
                                    bishop = new Point(mouseCell.X, mouseCell.Y - 2);
                                    knight = new Point(mouseCell.X, mouseCell.Y - 3);
                                }

                                break;
                        }

                        LastMove = mouseCell;
                        HighlightedSquares.Clear();
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

                    //Highlighting potential moves:
                    if (PieceGrid[PositionToCell(ms.Position).Y, PositionToCell(ms.Position).X] != null)
                    {
                        //Checking if the piece selected is of the same color as the turn:  
                        if (PieceGrid[PositionToCell(ms.Position).Y, PositionToCell(ms.Position).X].IsWhite == Whiteturn)
                        {
                            HighlightedSquares.Clear();
                            HighlightedSquares.Add((PositionToCell(ms.Position), MoveTypes.None));
                            var moves = PieceGrid[HighlightedSquares[0].location.Y, HighlightedSquares[0].location.X].GetMoves(PieceGrid, new Point(HighlightedSquares[0].location.X, HighlightedSquares[0].location.Y));

                            Point position = HighlightedSquares[0].location; //The original location of the selected piece

                            bool IsWhite = PieceGrid[position.Y, position.X].IsWhite; //The color of the selected piece

                            //Treat moves as potential 
                            //Then loop through each
                            foreach (var potentialMove in moves)
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
                                    HighlightedSquares.Add((new Point(potentialMove.Item1.X, potentialMove.Item1.Y), potentialMove.Item2));
                                }
                            }
                        }
                    }
                }

                else
                {
                    if (mouseCell == queen)
                    {
                        PieceGrid[LastMove.Y, LastMove.X] = new Queen(PieceGrid[LastMove.Y, LastMove.X].IsWhite);
                        Promotion = false;
                    }
                    else if (mouseCell == rook)
                    {
                        PieceGrid[LastMove.Y, LastMove.X] = new Rook(PieceGrid[LastMove.Y, LastMove.X].IsWhite);
                        Promotion = false;
                    }
                    else if (mouseCell == bishop)
                    {
                        PieceGrid[LastMove.Y, LastMove.X] = new Bishop(PieceGrid[LastMove.Y, LastMove.X].IsWhite);
                        Promotion = false;
                    }
                    else if (mouseCell == knight)
                    {
                        PieceGrid[LastMove.Y, LastMove.X] = new Knight(PieceGrid[LastMove.Y, LastMove.X].IsWhite);
                        Promotion = false;
                    }

                    //Checking if switching piece put someone in check:
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
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //Drawing grid:
            Color cellColor = Color.White;
            Color color = cellColor;
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    color = cellColor;

                    spriteBatch.Draw(Pixel, new Vector2(x * squaresize, y * squaresize), null, color, 0, new Vector2(0, 0), Vector2.One * squaresize, SpriteEffects.None, 0);
                    cellColor = cellColor == Color.White ? Color.Gray : Color.White;

                    //Changing the color of the highlighted squares:
                    if (Contains(new Point(x, y)))
                    {
                        color = Color.Yellow * 0.3f;
                    }

                    //Highlighting the checked king red:
                    if (WhiteInCheck)
                    {
                        Piece piece = PieceGrid[y, x];
                        if (piece != null && piece.IsWhite && piece.PieceType == PieceTypes.King)
                        {
                            color = Color.Red * 0.3f;
                        }
                    }
                    else if (BlackInCheck)
                    {
                        Piece piece = PieceGrid[y, x];
                        if (piece != null && !piece.IsWhite && piece.PieceType == PieceTypes.King)
                        {
                            color = Color.Red * 0.3f;
                        }
                    }


                    spriteBatch.Draw(Pixel, new Vector2(x * squaresize, y * squaresize), null, color, 0, new Vector2(0, 0), Vector2.One * squaresize, SpriteEffects.None, 0);
                }

                cellColor = cellColor == Color.White ? Color.Gray : Color.White;
            }

            //Drawing pieces:
            float scale;
            for (int y = 0; y < PieceGrid.GetLength(0); y++)
            {
                for (int x = 0; x < PieceGrid.GetLength(1); x++)
                {
                    if (PieceGrid[y, x] != null)
                    {
                        if (PieceGrid[y, x].PieceType == PieceTypes.Pawn)
                        {
                            scale = 1;
                        }
                        else
                        {
                            scale = 0.5f;
                        }
                        var texture = Textures[(PieceGrid[y, x].PieceType, PieceGrid[y, x].IsWhite)];
                        spriteBatch.Draw(texture, CellCenter(new Point(x, y)), null, Color.White, 0, new Vector2(texture.Width / 2, texture.Height / 2), scale, SpriteEffects.None, 0);
                    }
                }
            }

            if (Promotion)
            {
                //Gray out whole screen:
                spriteBatch.Draw(Pixel, graphics.GraphicsDevice.Viewport.Bounds, Color.White * 0.5f);

                //Draw piece choices:
                var texture = Textures[(PieceTypes.Queen, PieceGrid[LastMove.Y, LastMove.X].IsWhite)];
                spriteBatch.Draw(texture, CellCenter(queen), null, Color.White, 0, new Vector2(texture.Width / 2, texture.Height / 2), 0.5f, SpriteEffects.None, 0);

                texture = Textures[(PieceTypes.Rook, PieceGrid[LastMove.Y, LastMove.X].IsWhite)];
                spriteBatch.Draw(texture, CellCenter(rook), null, Color.White, 0, new Vector2(texture.Width / 2, texture.Height / 2), 0.5f, SpriteEffects.None, 0);

                texture = Textures[(PieceTypes.Bishop, PieceGrid[LastMove.Y, LastMove.X].IsWhite)];
                spriteBatch.Draw(texture, CellCenter(bishop), null, Color.White, 0, new Vector2(texture.Width / 2, texture.Height / 2), 0.5f, SpriteEffects.None, 0);

                texture = Textures[(PieceTypes.Knight, PieceGrid[LastMove.Y, LastMove.X].IsWhite)];
                spriteBatch.Draw(texture, CellCenter(knight), null, Color.White, 0, new Vector2(texture.Width / 2, texture.Height / 2), 0.5f, SpriteEffects.None, 0);
            }
        }

        

        public void ResetBoard()
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

        public bool Contains(Point pos)
        {
            foreach (var square in HighlightedSquares)
            {
                if (square.location == pos)
                {
                    return true;
                }
            }
            return false;
        }

        public int IndexOf(Point pos)
        {
            for (int i = 0; i < HighlightedSquares.Count; i++)
            {
                if (HighlightedSquares[i].location == pos)
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

        public Point PositionToCell(Point position)
        {
            return new Point((position.X / squaresize), (position.Y / squaresize));
        }

        public Vector2 CellCenter(Point GridPosition)
        {
            return new Vector2((GridPosition.X * squaresize) + squaresize / 2, (GridPosition.Y * squaresize) + squaresize / 2);
        }
    }
}
