using SharedLibrary.Pieces;
using SharedLibrary;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;

using System.Collections.Generic;
using System.Text.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Reflection;
using System.Threading;

namespace Chess
{
    public class Game1 : Game
    {
        private ChessGame chessGame;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public static Texture2D Pixel;

        public static Dictionary<(PieceTypes, bool), Texture2D> Textures;

        public MenuScreen menu;

        public MouseState Lastms;

        public int squaresize;

        public List<Point> HighlightedSquares;

        bool spectating = false;

        PiecePromotion choices;

        GameState currentGameState;


        bool inMenu = true;

        Guid playerID;

        Guid gameID;

        bool amWhite;

        const int delayMillis = 250;

        TimeSpan prevTime;

        string lastFEN = String.Empty;

        public Game1(Guid gameID)
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            this.gameID = gameID;

        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 800;
            graphics.ApplyChanges();

            squaresize = graphics.PreferredBackBufferWidth / 8;

            HighlightedSquares = new List<Point>();

            menu = new MenuScreen(Content, graphics);

            //This generates a list of all exceptions:
            var exceptions = Assembly.GetAssembly(typeof(int)).GetTypes().
                             Where(x => x.IsSubclassOf(typeof(Exception))).ToArray();

            chessGame = new ChessGame();
            
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


            playerID = Task.Run(async () => await ApiCalls.GetPlayerId(gameID)).Result; //No connection could be made because the target machine actively refused it. (localhost:5001)
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.MouseState = Mouse.GetState();

            prevTime += gameTime.ElapsedGameTime;
            if (prevTime.TotalMilliseconds >= delayMillis)
            {
                Task.Run(async () => await GetGameState()).Wait();
            }


            if (inMenu)
            {
                var result = menu.Update(gameTime);
                if (result.moveOn)
                {
                    if (result.spectating)
                    {
                        inMenu = false;
                        spectating = true;
                    }
                    else
                    {
                        bool? color = Task.Run(async () => await ApiCalls.GetGameColor(gameID, playerID, result.playingWhite)).Result;
                        if (color == null)
                        {
                            inMenu = false;
                            spectating = true;
                        }
                        else
                        {
                            amWhite = (bool)color;

                            Task.Run(async () => await ApiCalls.ResetBoard(gameID, playerID)).Wait();
                            Task.Run(async () => await GetGameState()).Wait();

                            inMenu = false;
                        }
                    }
                }
            }

            else if (spectating)
            {

            }


            //Checking if mouse clicked:
            else if (InputManager.MouseState.LeftButton == ButtonState.Pressed && InputManager.LastMouseState.LeftButton == ButtonState.Released && GraphicsDevice.Viewport.Bounds.Contains(InputManager.MouseState.Position) && currentGameState.Whiteturn == amWhite && IsActive)
            {
                var mouseCell = PositionToCell(InputManager.MouseState.Position);

                //If choosing a piece for promotion:
                if (currentGameState.ChoosingPromotion)
                {
                    var promotionInfo = CheckPromotion();
                    choices = new PiecePromotion(amWhite, promotionInfo.pawnLocation.X);

                    if (mouseCell == choices.Queen.ToPoint())
                    {
                        Task.Run(async () => await ApiCalls.Promote(gameID, playerID, "Queen")).Wait();
                    }

                    else if (mouseCell == choices.Rook.ToPoint())
                    {
                        Task.Run(async () => await ApiCalls.Promote(gameID, playerID, "Rook")).Wait();
                    }

                    else if (mouseCell == choices.Bishop.ToPoint())
                    {
                        Task.Run(async () => await ApiCalls.Promote(gameID, playerID, "Bishop")).Wait();
                    }

                    else if (mouseCell == choices.Knight.ToPoint())
                    {
                        Task.Run(async () => await ApiCalls.Promote(gameID, playerID, "Knight")).Wait();
                    }

                    Task.Run(async () => await GetGameState()).Wait();
                    CheckIfGameOver(gameTime);
                }

                //Deselecting piece:
                else if (currentGameState.PieceGrid[mouseCell.Y, mouseCell.X] != null && HighlightedSquares.Count > 0 && mouseCell == HighlightedSquares[0])
                {
                    HighlightedSquares.Clear();
                }

                //Selecting piece:
                else if (currentGameState.PieceGrid[mouseCell.Y, mouseCell.X] != null && currentGameState.PieceGrid[mouseCell.Y, mouseCell.X].IsWhite == amWhite)
                {
                    HighlightedSquares.Clear();

                    Point[] moves;
                    if (amWhite)
                    {
                        moves = Task.Run(async () => await ApiCalls.GetMoves(gameID, mouseCell)).Result;

                        HighlightedSquares.AddRange(moves);
                    }
                    else
                    {
                        moves = Task.Run(async () => await ApiCalls.GetMoves(gameID, Flip(mouseCell))).Result;

                        HighlightedSquares.AddRange(FlipSquares(moves));
                    }

                    
                }
                //Selecting move:
                else
                {
                    if (HighlightedSquares.Contains(mouseCell) && mouseCell != HighlightedSquares[0])
                    {
                        if (amWhite)
                        {
                            Task.Run(async () => await ApiCalls.Move(gameID, playerID, HighlightedSquares[0], mouseCell)).Wait();
                        }
                        else
                        {
                            Task.Run(async () => await ApiCalls.Move(gameID, playerID, Flip(HighlightedSquares[0]), Flip(mouseCell))).Wait();
                        }

                        HighlightedSquares.Clear();

                        Task.Run(async () => await ApiCalls.CheckPromotion(gameID)).Wait();

                        Task.Run(async () => await GetGameState()).Wait();

                        var promotionInfo = CheckPromotion();
                        if (promotionInfo.promotion)
                        {
                            choices = new PiecePromotion(amWhite, promotionInfo.pawnLocation.X);
                        }


                        CheckIfGameOver(gameTime);
                    }
                }
            }


            InputManager.LastMouseState = InputManager.MouseState;
            base.Update(gameTime);
        }

        void CheckIfGameOver(GameTime gameTime)
        {
            System.Windows.Forms.DialogResult result = System.Windows.Forms.DialogResult.None;


            System.Windows.Forms.MessageBox.Show();//pop up a messagebox on launch that asks for gameID.

            var checkForMoveResults = Task.Run(async () => await ApiCalls.CheckForNoMoves(gameID)).Result;
            if (checkForMoveResults)
            {
                Draw(gameTime);
                if (currentGameState.WhiteInCheck)
                {
                    result = System.Windows.Forms.MessageBox.Show("White in checkmate", "Game over", System.Windows.Forms.MessageBoxButtons.AbortRetryIgnore);
                }
                else if (currentGameState.BlackInCheck)
                {
                    result = System.Windows.Forms.MessageBox.Show("Black in checkmate", "Game over", System.Windows.Forms.MessageBoxButtons.AbortRetryIgnore);
                }
                else
                {
                    result = System.Windows.Forms.MessageBox.Show("Stalemate", "Game over", System.Windows.Forms.MessageBoxButtons.AbortRetryIgnore);
                }
            }

            else if (currentGameState.moveCounter >= 50)
            {
                Draw(gameTime);
                result = System.Windows.Forms.MessageBox.Show("There have been 50 moves and nothing has happened", "Game over", System.Windows.Forms.MessageBoxButtons.AbortRetryIgnore);
            }

            switch (result)
            {
                case System.Windows.Forms.DialogResult.Abort:
                    this.Exit();
                    break;

                case System.Windows.Forms.DialogResult.Retry:
                    Task.Run(async () => await ApiCalls.ResetBoard(gameID, playerID)).Wait();
                    break;

                case System.Windows.Forms.DialogResult.Ignore:
                    spectating = true;
                    break;
            }
        }

        public Vector2 CellCenter(Point GridPosition)
        {
            return new Vector2((GridPosition.X * squaresize) + squaresize / 2, (GridPosition.Y * squaresize) + squaresize / 2);
        }

        public Point PositionToCell(Point position)
        {
            return new Point((position.X / squaresize), (position.Y / squaresize));
        }

        public Point Flip(Point square)
        {
            return new Point(7 - square.X, 7 - square.Y);
        }
        public Point[] FlipSquares(Point[] squares)
        {
            Point[] flipped = new Point[squares.Length];

            for (int i = 0; i < squares.Length; i++)
            {
                flipped[i] = new Point(7 - squares[i].X, 7 - squares[i].Y);
            }

            return flipped;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            if (inMenu)
            {
                menu.Draw(spriteBatch);
            }

            else
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
                        if (HighlightedSquares.Contains(new Point(x, y)))
                        {
                            color = Color.Yellow * 0.3f;
                        }

                        //Highlighting the checked king red:
                        if (currentGameState.WhiteInCheck)
                        {
                            Piece piece = currentGameState.PieceGrid[y, x];
                            if (piece != null && piece.IsWhite && piece.PieceType == PieceTypes.King)
                            {
                                color = Color.Red * 0.3f;
                            }
                        }
                        else if (currentGameState.BlackInCheck)
                        {
                            Piece piece = currentGameState.PieceGrid[y, x];
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
                for (int y = 0; y < currentGameState.PieceGrid.GetLength(0); y++)
                {
                    for (int x = 0; x < currentGameState.PieceGrid.GetLength(1); x++)
                    {
                        if (currentGameState.PieceGrid[y, x] != null)
                        {
                            if (currentGameState.PieceGrid[y, x].PieceType == PieceTypes.Pawn)
                            {
                                scale = 1;
                            }
                            else
                            {
                                scale = 0.5f;
                            }
                            var texture = Textures[(currentGameState.PieceGrid[y, x].PieceType, currentGameState.PieceGrid[y, x].IsWhite)];
                            spriteBatch.Draw(texture, CellCenter(new Point(x, y)), null, Color.White, 0, new Vector2(texture.Width / 2, texture.Height / 2), scale, SpriteEffects.None, 0);
                        }
                    }
                }





                if (currentGameState.ChoosingPromotion && currentGameState.Whiteturn == amWhite)
                {
                    //Gray out whole screen:
                    spriteBatch.Draw(Pixel, graphics.GraphicsDevice.Viewport.Bounds, Color.White * 0.5f);


                    //Draw piece choices:
                    var texture = Textures[(PieceTypes.Queen, amWhite)];
                    spriteBatch.Draw(texture, CellCenter(choices.Queen.ToPoint()), null, Color.White, 0, new Vector2(texture.Width / 2, texture.Height / 2), 0.5f, SpriteEffects.None, 0);

                    texture = Textures[(PieceTypes.Rook, amWhite)];
                    spriteBatch.Draw(texture, CellCenter(choices.Rook.ToPoint()), null, Color.White, 0, new Vector2(texture.Width / 2, texture.Height / 2), 0.5f, SpriteEffects.None, 0);

                    texture = Textures[(PieceTypes.Bishop, amWhite)];
                    spriteBatch.Draw(texture, CellCenter(choices.Bishop.ToPoint()), null, Color.White, 0, new Vector2(texture.Width / 2, texture.Height / 2), 0.5f, SpriteEffects.None, 0);

                    texture = Textures[(PieceTypes.Knight, amWhite)];
                    spriteBatch.Draw(texture, CellCenter(choices.Knight.ToPoint()), null, Color.White, 0, new Vector2(texture.Width / 2, texture.Height / 2), 0.5f, SpriteEffects.None, 0);
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public (bool promotion, bool IsWhite, Point pawnLocation) CheckPromotion()
        {
            bool promotion = false;
            bool isWhite = false;
            Point pawnLocation = new Point(0, 0);

            for (int x = 0; x < currentGameState.PieceGrid.GetLength(1); x++)
            {
                //Checking for a pawn in the top row:
                if (currentGameState.PieceGrid[0, x] != null && currentGameState.PieceGrid[0, x].PieceType == PieceTypes.Pawn)
                {
                    promotion = true;
                    isWhite = amWhite;
                    pawnLocation = new Point(x, 0);
                }
            }

            return (promotion, isWhite, pawnLocation);
        }


        public async Task GetGameState()
        {
            string newFEN = await ApiCalls.MakeFEN(gameID);
            if (newFEN == lastFEN)
            {
                return;
            }
            currentGameState = DecodeFEN(newFEN);
            lastFEN = newFEN;
        }

        public GameState DecodeFEN(string FEN)
        {
            GameState gamestate = new GameState();

            gamestate.PieceGrid = new Piece[8, 8];

            var rows = FEN.Split('/');

            var ending = rows[7].Split(' ');

            rows[7] = ending[0];

            for (int y = 0; y < rows.Length; y++)
            {
                int x = 0;
                for (int i = 0; i < rows[y].Length; i++)
                {
                    if (amWhite)
                    {
                        switch (rows[y][i])
                            {
                                case 'p':
                                    gamestate.PieceGrid[y, x] = new Pawn(chessGame, false);
                                    break;

                                case 'P':
                                    gamestate.PieceGrid[y, x] = new Pawn(chessGame, true);
                                    break;

                                case 'b':
                                    gamestate.PieceGrid[y, x] = new Bishop(chessGame, false);
                                    break;

                                case 'B':
                                    gamestate.PieceGrid[y, x] = new Bishop(chessGame, true);
                                    break;

                                case 'n':
                                    gamestate.PieceGrid[y, x] = new Knight(chessGame, false);
                                    break;

                                case 'N':
                                    gamestate.PieceGrid[y, x] = new Knight(chessGame, true);
                                    break;

                                case 'k':
                                    gamestate.PieceGrid[y, x] = new King(chessGame, false);
                                    break;

                                case 'K':
                                    gamestate.PieceGrid[y, x] = new King(chessGame, true);
                                    break;

                                case 'r':
                                    gamestate.PieceGrid[y, x] = new Rook(chessGame, false);
                                    break;

                                case 'R':
                                    gamestate.PieceGrid[y, x] = new Rook(chessGame, true);
                                    break;

                                case 'q':
                                    gamestate.PieceGrid[y, x] = new Queen(chessGame, false);
                                    break;

                                case 'Q':
                                    gamestate.PieceGrid[y, x] = new Queen(chessGame, true);
                                    break;

                                default:
                                    x += (int)char.GetNumericValue(rows[y][i]) - 1;
                                    break;
                            }
                    }
                    else
                        {
                            switch (rows[y][i])
                            {
                                case 'p':
                                    gamestate.PieceGrid[7 - y, 7 - x] = new Pawn(chessGame, false);
                                    break;

                                case 'P':
                                    gamestate.PieceGrid[7 - y, 7 - x] = new Pawn(chessGame, true);
                                    break;

                                case 'b':
                                    gamestate.PieceGrid[7 - y, 7 - x] = new Bishop(chessGame, false);
                                    break;

                                case 'B':
                                    gamestate.PieceGrid[7 - y, 7 - x] = new Bishop(chessGame, true);
                                    break;

                                case 'n':
                                    gamestate.PieceGrid[7 - y, 7 - x] = new Knight(chessGame, false);
                                    break;

                                case 'N':
                                    gamestate.PieceGrid[7 - y, 7 - x] = new Knight(chessGame, true);
                                    break;

                                case 'k':
                                    gamestate.PieceGrid[7 - y, 7 - x] = new King(chessGame, false);
                                    break;

                                case 'K':
                                    gamestate.PieceGrid[7 - y, 7 - x] = new King(chessGame, true);
                                    break;

                                case 'r':
                                    gamestate.PieceGrid[7 - y, 7 - x] = new Rook(chessGame, false);
                                    break;

                                case 'R':
                                    gamestate.PieceGrid[7 - y, 7 - x] = new Rook(chessGame, true);
                                    break;

                                case 'q':
                                    gamestate.PieceGrid[7 - y, 7 - x] = new Queen(chessGame, false);
                                    break;

                                case 'Q':
                                    gamestate.PieceGrid[7 - y, 7 - x] = new Queen(chessGame, true);
                                    break;

                                default:
                                    x += (int)char.GetNumericValue(rows[y][i]) - 1;
                                    break;
                            }
                        }

                    x++;
                }
            }




            if (ending[1] == "w")
            {
                gamestate.Whiteturn = true;
            }
            else
            {
                gamestate.Whiteturn = false;
            }

            if (ending[2] == "w")
            {
                gamestate.WhiteInCheck = true;
            }
            else if (ending[2] == "b")
            {
                gamestate.BlackInCheck = true;
            }

            if (ending[3] == "y")
            {
                gamestate.ChoosingPromotion = true;
            }
            else
            {
                gamestate.ChoosingPromotion = false;
            }

            return gamestate;
        }
    }
}