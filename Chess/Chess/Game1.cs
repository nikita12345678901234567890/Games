using Chess.Pieces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;

using System.Collections.Generic;

namespace Chess
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public static Texture2D Pixel;

        public static Dictionary<(PieceTypes, bool), Texture2D> Textures;

        public int squaresize;

        public MouseState Lastms;


        //Specifically chess related stuff:
        public Piece[,] PieceGrid = new Piece[8, 8];

        public List<(Point location, MoveTypes)> HighlightedSquares;

        public bool Whiteturn = true;

        public static Point LastMove;

        public static bool WhiteInCheck = false;

        public static bool BlackInCheck = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 800;
            graphics.ApplyChanges();

            squaresize = graphics.PreferredBackBufferWidth / 8;

            HighlightedSquares = new List<(Point, MoveTypes)>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Pixel = new Texture2D(GraphicsDevice, 1, 1);
            Pixel.SetData(new[] { Color.White });

            //Loading piece textures:
            Textures = new Dictionary<(PieceTypes, bool), Texture2D>();
            Textures.Add((PieceTypes.Pawn, true), Content.Load<Texture2D>("whitepawn"));
            Textures.Add((PieceTypes.Pawn, false), Content.Load<Texture2D>("blackpawn"));
            Textures.Add((PieceTypes.Rook, true), Content.Load<Texture2D>("whiterook"));
            Textures.Add((PieceTypes.Rook, false), Content.Load<Texture2D>("blackrook"));
            Textures.Add((PieceTypes.Knight, true), Content.Load<Texture2D>("whiteknight"));
            Textures.Add((PieceTypes.Knight, false), Content.Load<Texture2D>("blackknight"));
            Textures.Add((PieceTypes.Bishop, true), Content.Load<Texture2D>("whitebishop"));
            Textures.Add((PieceTypes.Bishop, false), Content.Load<Texture2D>("blackbishop"));
            Textures.Add((PieceTypes.King, true), Content.Load<Texture2D>("whiteking"));
            Textures.Add((PieceTypes.King, false), Content.Load<Texture2D>("blackking"));
            Textures.Add((PieceTypes.Queen, true), Content.Load<Texture2D>("whitequeen"));
            Textures.Add((PieceTypes.Queen, false), Content.Load<Texture2D>("blackqueen"));

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

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            MouseState ms = Mouse.GetState();

            //Checking if mouse clicked:
            if (ms.LeftButton == ButtonState.Pressed && Lastms.LeftButton == ButtonState.Released && GraphicsDevice.Viewport.Bounds.Contains(ms.Position))
            {
                var mouseCell = PositionToCell(ms.Position);
                //Moving:
                if (Contains(mouseCell) && HighlightedSquares[0].location != mouseCell)
                {
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
                    }

                    LastMove = mouseCell;
                    HighlightedSquares.Clear();
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

                //Highlighting potential moves:
                if (PieceGrid[PositionToCell(ms.Position).Y, PositionToCell(ms.Position).X] != null)
                {
                    //Checking if the piece selected is of the same color as the turn:  
                    if (PieceGrid[PositionToCell(ms.Position).Y, PositionToCell(ms.Position).X].IsWhite == Whiteturn)
                    {
                        HighlightedSquares.Clear();
                        HighlightedSquares.Add((PositionToCell(ms.Position), MoveTypes.None));
                        var moves = PieceGrid[HighlightedSquares[0].location.Y, HighlightedSquares[0].location.X].GetMoves(PieceGrid, new Point(HighlightedSquares[0].location.X, HighlightedSquares[0].location.Y));
                       
                        //Treat moves as potential 
                        //Then loop through each

                        
                        HighlightedSquares.AddRange(moves);
                    }
                }
            }

            Lastms = ms;
            base.Update(gameTime);
        }

        public static bool IsChecking(Piece piece, Point pieceGridPositiion, Piece[,] PieceGrid)
        {
            //Because kings cannot check:
            if (piece.PieceType == PieceTypes.King)
            {
                return false;
            }

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

        public static bool IsChecking(Piece piece, Point pieceGridPositiion, Piece[,] PieceGrid, Point move)
        {
            //Because kings cannot check:
            if (piece.PieceType == PieceTypes.King)
            {
                return false;
            }

            if (PieceGrid[move.Y, move.X] != null && PieceGrid[move.Y, move.X].IsWhite != piece.IsWhite && PieceGrid[move.Y, move.X].PieceType == PieceTypes.King)
            {
                return true;
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

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

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

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}