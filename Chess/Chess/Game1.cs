﻿using SharedLibrary.Pieces;
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

        public bool WhiteInCheck = false;

        public bool BlackInCheck = false;

        public bool Whiteturn = true;

        public Piece[,] PieceGrid = new Piece[8, 8];

        


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
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            MouseState ms = Mouse.GetState();

            //Checking if mouse clicked:
            if (InputManager.MouseState.LeftButton == ButtonState.Pressed && InputManager.LastMouseState.LeftButton == ButtonState.Released && GraphicsDevice.Viewport.Bounds.Contains(InputManager.MouseState.Position))
            {
                var mouseCell = PositionToCell(InputManager.MouseState.Position);

                //Selecting piece:
                if (HighlightedSquares.Count <= 0)
                {
                    if (PieceGrid[mouseCell.Y, mouseCell.X].IsWhite = Whiteturn)
                    {
                        Point[] moves = GetMoves(PieceGrid[mouseCell.Y, mouseCell.X]);

                        HighlightedSquares.AddRange(moves);
                    }
                }
                //Selecting move:
                else
                {
                    if (HighlightedSquares.Contains(mouseCell))
                    {
                        Move(HighlightedSquares[0], mouseCell);
                    }
                }
            }

            Lastms = ms;
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


            var promotionInfo = CheckPromotion();

            if (promotionInfo.promotion)
            {
                //Gray out whole screen:
                spriteBatch.Draw(Pixel, graphics.GraphicsDevice.Viewport.Bounds, Color.White * 0.5f);


                Point queen;
                Point rook;
                Point bishop;
                Point knight;
                //Calculating the positions of the piece choices:
                if (promotionInfo.IsWhite)
                {
                    queen = promotionInfo.pawnLocation;
                    rook = new Point(promotionInfo.pawnLocation.X, promotionInfo.pawnLocation.Y - 1);
                    bishop = new Point(promotionInfo.pawnLocation.X, promotionInfo.pawnLocation.Y - 2);
                    knight = new Point(promotionInfo.pawnLocation.X, promotionInfo.pawnLocation.Y - 3);
                }
                else
                {
                    queen = promotionInfo.pawnLocation;
                    rook = new Point(promotionInfo.pawnLocation.X, promotionInfo.pawnLocation.Y + 1);
                    bishop = new Point(promotionInfo.pawnLocation.X, promotionInfo.pawnLocation.Y + 2);
                    knight = new Point(promotionInfo.pawnLocation.X, promotionInfo.pawnLocation.Y + 3);
                }

                //Draw piece choices:
                var texture = Textures[(PieceTypes.Queen, promotionInfo.IsWhite)];
                spriteBatch.Draw(texture, CellCenter(queen), null, Color.White, 0, new Vector2(texture.Width / 2, texture.Height / 2), 0.5f, SpriteEffects.None, 0);

                texture = Textures[(PieceTypes.Rook, promotionInfo.IsWhite)];
                spriteBatch.Draw(texture, CellCenter(rook), null, Color.White, 0, new Vector2(texture.Width / 2, texture.Height / 2), 0.5f, SpriteEffects.None, 0);

                texture = Textures[(PieceTypes.Bishop, promotionInfo.IsWhite)];
                spriteBatch.Draw(texture, CellCenter(bishop), null, Color.White, 0, new Vector2(texture.Width / 2, texture.Height / 2), 0.5f, SpriteEffects.None, 0);

                texture = Textures[(PieceTypes.Knight, promotionInfo.IsWhite)];
                spriteBatch.Draw(texture, CellCenter(knight), null, Color.White, 0, new Vector2(texture.Width / 2, texture.Height / 2), 0.5f, SpriteEffects.None, 0);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public (bool promotion, bool IsWhite, Point pawnLocation) CheckPromotion()
        {
            bool promotion = false;
            bool isWhite = false;
            Point pawnLocation = new Point(0, 0);

            for (int x = 0; x < PieceGrid.GetLength(1); x++)
            {
                //Checking for a pawn in the top row:
                if (PieceGrid[0, x].PieceType == PieceTypes.Pawn)
                {
                    promotion = true;
                    isWhite = true;
                    pawnLocation = new Point(x, 0);
                }

                //Checking for a pawn in the bottom row:
                if (PieceGrid[PieceGrid.GetLength(0) - 1, x].PieceType == PieceTypes.Pawn)
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

            if (ending[1] == "w")
            {
                Whiteturn = true;
            }
            else
            {
                Whiteturn = false;
            }

            throw new Exception("FEN decoder not ready yet");

            return grid;
        }

        public Point[] GetMoves(Piece piece)
        {
            //This should call the GetMoves function in Class1
            throw new Exception("This function isn't ready yet");
        }

        public void Move(Point piece, Point destination)
        {
            //This should call the Move function in Class1
            throw new Exception("This function isn't ready yet");
        }
    }
}