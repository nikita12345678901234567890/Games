using SharedLibrary.Pieces;
using SharedLibrary;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;

using System.Collections.Generic;
using System.Text.Json;
using System;

namespace Chess
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public static Texture2D Pixel;

        public static Dictionary<(PieceTypes, bool), Texture2D> Textures;

        public MouseState Lastms;

        public int squaresize;

        public List<Point> HighlightedSquares;

        public bool choosingPromotion = false;

        public PiecePromotion choices;

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

            HighlightedSquares = new List<Point>();

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

            Class1.ResetBoard();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Class1.CheckForCheckmate())
            {
                if (Class1.Whiteturn)
                {
                    System.Windows.Forms.MessageBox.Show("White in checkmate", "Game over", System.Windows.Forms.MessageBoxButtons.AbortRetryIgnore);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Black in checkmate", "Game over", System.Windows.Forms.MessageBoxButtons.OK);
                }
            }

            InputManager.MouseState = Mouse.GetState();

            //Checking if mouse clicked:
            if (InputManager.MouseState.LeftButton == ButtonState.Pressed && InputManager.LastMouseState.LeftButton == ButtonState.Released && GraphicsDevice.Viewport.Bounds.Contains(InputManager.MouseState.Position))
            {
                var mouseCell = PositionToCell(InputManager.MouseState.Position);

                //If choosing a piece for promotion:
                if (choosingPromotion)
                {
                    if (mouseCell == choices.Queen)
                    {
                        Class1.PieceGrid[choices.Queen.Y, choices.Queen.X] = new Queen(choices.white);
                    }

                    else if (mouseCell == choices.Rook)
                    {
                        Class1.PieceGrid[choices.Queen.Y, choices.Queen.X] = new Rook(choices.white);
                    }

                    else if (mouseCell == choices.Bishop)
                    {
                        Class1.PieceGrid[choices.Queen.Y, choices.Queen.X] = new Bishop(choices.white);
                    }

                    else if (mouseCell == choices.Knight)
                    {
                        Class1.PieceGrid[choices.Queen.Y, choices.Queen.X] = new Knight(choices.white);
                    }
                }

                //Deselecting piece:
                if (Class1.PieceGrid[mouseCell.Y, mouseCell.X] != null && HighlightedSquares.Count > 0 && mouseCell == HighlightedSquares[0])
                {
                    HighlightedSquares.Clear();
                }

                //Selecting piece:
                else if (Class1.PieceGrid[mouseCell.Y, mouseCell.X] != null && Class1.PieceGrid[mouseCell.Y, mouseCell.X].IsWhite == Class1.Whiteturn)
                {
                    HighlightedSquares.Clear();

                    Point[] moves = GetMoves(new Point(mouseCell.X, mouseCell.Y));

                    HighlightedSquares.AddRange(moves);
                }

                //Selecting move:
                else
                {
                    if (HighlightedSquares.Contains(mouseCell) && mouseCell != HighlightedSquares[0])
                    {
                        Move(HighlightedSquares[0], mouseCell);
                        HighlightedSquares.Clear();
                    }
                }
            }

            InputManager.LastMouseState = InputManager.MouseState;
            base.Update(gameTime);
        }

        public Vector2 CellCenter(Point GridPosition)
        {
            return new Vector2((GridPosition.X * squaresize) + squaresize / 2, (GridPosition.Y * squaresize) + squaresize / 2);
        }

        public Point PositionToCell(Point position)
        {
            return new Point((position.X / squaresize), (position.Y / squaresize));
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
                    if (HighlightedSquares.Contains(new Point(x, y)))
                    {
                        color = Color.Yellow * 0.3f;
                    }

                    //Highlighting the checked king red:
                    if (Class1.WhiteInCheck)
                    {
                        Piece piece = Class1.PieceGrid[y, x];
                        if (piece != null && piece.IsWhite && piece.PieceType == PieceTypes.King)
                        {
                            color = Color.Red * 0.3f;
                        }
                    }
                    else if (Class1.BlackInCheck)
                    {
                        Piece piece = Class1.PieceGrid[y, x];
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
            for (int y = 0; y < Class1.PieceGrid.GetLength(0); y++)
            {
                for (int x = 0; x < Class1.PieceGrid.GetLength(1); x++)
                {
                    if (Class1.PieceGrid[y, x] != null)
                    {
                        if (Class1.PieceGrid[y, x].PieceType == PieceTypes.Pawn)
                        {
                            scale = 1;
                        }
                        else
                        {
                            scale = 0.5f;
                        }
                        var texture = Textures[(Class1.PieceGrid[y, x].PieceType, Class1.PieceGrid[y, x].IsWhite)];
                        spriteBatch.Draw(texture, CellCenter(new Point(x, y)), null, Color.White, 0, new Vector2(texture.Width / 2, texture.Height / 2), scale, SpriteEffects.None, 0);
                    }
                }
            }


            var promotionInfo = CheckPromotion();

            choosingPromotion = promotionInfo.promotion;

            choices = new PiecePromotion(promotionInfo.IsWhite, promotionInfo.pawnLocation.X);

            if (choosingPromotion)
            {
                //Gray out whole screen:
                spriteBatch.Draw(Pixel, graphics.GraphicsDevice.Viewport.Bounds, Color.White * 0.5f);


                //Draw piece choices:
                var texture = Textures[(PieceTypes.Queen, promotionInfo.IsWhite)];
                spriteBatch.Draw(texture, CellCenter(choices.Queen), null, Color.White, 0, new Vector2(texture.Width / 2, texture.Height / 2), 0.5f, SpriteEffects.None, 0);

                texture = Textures[(PieceTypes.Rook, promotionInfo.IsWhite)];
                spriteBatch.Draw(texture, CellCenter(choices.Rook), null, Color.White, 0, new Vector2(texture.Width / 2, texture.Height / 2), 0.5f, SpriteEffects.None, 0);

                texture = Textures[(PieceTypes.Bishop, promotionInfo.IsWhite)];
                spriteBatch.Draw(texture, CellCenter(choices.Bishop), null, Color.White, 0, new Vector2(texture.Width / 2, texture.Height / 2), 0.5f, SpriteEffects.None, 0);

                texture = Textures[(PieceTypes.Knight, promotionInfo.IsWhite)];
                spriteBatch.Draw(texture, CellCenter(choices.Knight), null, Color.White, 0, new Vector2(texture.Width / 2, texture.Height / 2), 0.5f, SpriteEffects.None, 0);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public (bool promotion, bool IsWhite, Point pawnLocation) CheckPromotion()
        {
            bool promotion = false;
            bool isWhite = false;
            Point pawnLocation = new Point(0, 0);

            for (int x = 0; x < Class1.PieceGrid.GetLength(1); x++)
            {
                //Checking for a pawn in the top row:
                if (Class1.PieceGrid[0, x] != null && Class1.PieceGrid[0, x].PieceType == PieceTypes.Pawn)
                {
                    promotion = true;
                    isWhite = true;
                    pawnLocation = new Point(x, 0);
                }

                //Checking for a pawn in the bottom row:
                if (Class1.PieceGrid[Class1.PieceGrid.GetLength(0) - 1, x] != null && Class1.PieceGrid[Class1.PieceGrid.GetLength(0) - 1, x].PieceType == PieceTypes.Pawn)
                {
                    promotion = true;
                    isWhite = false;
                    pawnLocation = new Point(x, 0);
                }
            }

            return (promotion, isWhite, pawnLocation);
        }

        public Piece[,] DecodeFEN(string FEN)
        {
            Piece[,] grid = new Piece[8, 8];

            var rows = FEN.Split('/');

            var ending = rows[7].Split(' ');

            rows[7] = ending[0];

            for (int y = 0; y < rows.Length; y++)
            {
                for (int x = 0; x < rows[0].Length; x++)
                {
                    switch (rows[y][x])
                    {
                        case 'p':
                            grid[y, x] = new Pawn(false);
                            break;

                        case 'P':
                            grid[y, x] = new Pawn(true);
                            break;

                        case 'b':
                            grid[y, x] = new Bishop(false);
                            break;

                        case 'B':
                            grid[y, x] = new Bishop(true);
                            break;

                        case 'n':
                            grid[y, x] = new Knight(false);
                            break;

                        case 'N':
                            grid[y, x] = new Knight(true);
                            break;

                        case 'k':
                            grid[y, x] = new King(false);
                            break;

                        case 'K':
                            grid[y, x] = new King(true);
                            break;

                        case 'r':
                            grid[y, x] = new Rook(false);
                            break;

                        case 'R':
                            grid[y, x] = new Rook(true);
                            break;

                        case 'q':
                            grid[y, x] = new Queen(false);
                            break;

                        case 'Q':
                            grid[y, x] = new Queen(true);
                            break;

                        default:
                            x += (int)char.GetNumericValue(rows[y][x]) - 1;
                            break;
                    }
                }
            }

            //Checking whose move it is:
            /*if (ending[1] == "w")
            {
                Whiteturn = true;
            }
            else
            {
                Whiteturn = false;
            }

            //Checking whose in check:
            if (ending[2] == "w")
            {
                WhiteInCheck = true;
                BlackInCheck = false;
            }
            else if (ending[2] == "b")
            {
                WhiteInCheck = false;
                BlackInCheck = true;
            }
            else
            {
                WhiteInCheck = false;
                BlackInCheck = false;
            }*/ //add checking for checkmate;

            return grid;
        }

        public Point[] GetMoves(Point pieceLocation)
        {
            return Class1.GetMoves(pieceLocation);
        }

        public void Move(Point piece, Point destination)
        {
            Class1.Move(piece, destination);
        }
    }
}