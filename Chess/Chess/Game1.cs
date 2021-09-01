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
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public static Texture2D Pixel;

        public static Dictionary<(PieceTypes, bool), Texture2D> Textures;

        public MouseState Lastms;

        public int squaresize;

        public List<Point> HighlightedSquares;

        bool spectating = false;

        PiecePromotion choices;

        (bool promotion, bool IsWhite, Point pawnLocation) promotionInfo;

        GameState currentGameState;



        //HttpClient client = new HttpClient();  //part of Api example


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            //Task.Run(CallApi);  //part of Api example 
        }


        /*public async Task CallApi()
        {
            var result = await client.GetAsync($"https://localhost:44399/game/Other/15");
            var temp = result.Content.ReadAsStringAsync();
            ;
        }*/  //Api example

        //HttpClient client = new HttpClient();
        //public async Task CallApi()
        //{
        //    Person p = new Person() { Fish = 5, Password = "1234", UserName = "abcd" };
        //    string json = JsonSerializer.Serialize<Person>(p);
        //    StringContent s = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        //    var result = await client.PostAsync($"https://localhost:44399/game/Test", s);
        //    var temp = result.Content.ReadAsStringAsync();
        //    var res = temp.Result;


        //}//Jason majic example.

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 800;
            graphics.ApplyChanges();

            squaresize = graphics.PreferredBackBufferWidth / 8;

            HighlightedSquares = new List<Point>();

            //This generates a list of all exceptions:
            var exceptions = Assembly.GetAssembly(typeof(int)).GetTypes().
                             Where(x => x.IsSubclassOf(typeof(Exception))).ToArray();

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

            ApiCalls.ResetBoard().Wait();
            GetGameState().Wait();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.MouseState = Mouse.GetState();

            //Checking if mouse clicked:
            if (!spectating && InputManager.MouseState.LeftButton == ButtonState.Pressed && InputManager.LastMouseState.LeftButton == ButtonState.Released && GraphicsDevice.Viewport.Bounds.Contains(InputManager.MouseState.Position))
            {
                var mouseCell = PositionToCell(InputManager.MouseState.Position);

                //If choosing a piece for promotion:
                if (currentGameState.ChoosingPromotion)
                {
                    promotionInfo = CheckPromotion();
                    choices = new PiecePromotion(promotionInfo.IsWhite, promotionInfo.pawnLocation.X);

                    if (mouseCell == choices.Queen)
                    {
                        ApiCalls.Promote("Queen").Wait();
                    }

                    else if (mouseCell == choices.Rook)
                    {
                        /*Thread resetThread = new Thread(new ThreadStart(async () =>
                        {
                            await ApiCalls.Promote("Rook");
                        }));
                        resetThread.Start();*/
                        ApiCalls.Promote("Rook").Wait();
                    }

                    else if (mouseCell == choices.Bishop)
                    {
                        ApiCalls.Promote("Bishop").Wait();
                    }

                    else if (mouseCell == choices.Knight)
                    {
                        ApiCalls.Promote("Knight").Wait();
                    }

                    GetGameState().Wait();
                    CheckIfGameOver();
                }

                //Deselecting piece:
                if (currentGameState.PieceGrid[mouseCell.Y, mouseCell.X] != null && HighlightedSquares.Count > 0 && mouseCell == HighlightedSquares[0])
                {
                    HighlightedSquares.Clear();
                }

                //Selecting piece:
                else if (currentGameState.PieceGrid[mouseCell.Y, mouseCell.X] != null && currentGameState.PieceGrid[mouseCell.Y, mouseCell.X].IsWhite == currentGameState.Whiteturn)
                {
                    HighlightedSquares.Clear();

                    Point[] moves = ApiCalls.GetMoves(new Point(mouseCell.X, mouseCell.Y)).Result;

                    HighlightedSquares.AddRange(moves);
                }
                //Selecting move:
                else
                {
                    if (HighlightedSquares.Contains(mouseCell) && mouseCell != HighlightedSquares[0])
                    {
                        ApiCalls.Move(HighlightedSquares[0], mouseCell).Wait();


                        HighlightedSquares.Clear();

                        GetGameState().Wait();
                        CheckIfGameOver();
                    }
                }
            }


            InputManager.LastMouseState = InputManager.MouseState;
            base.Update(gameTime);
        }

        void CheckIfGameOver()
        {
            System.Windows.Forms.DialogResult result = System.Windows.Forms.DialogResult.None;

            if (ApiCalls.CheckForNoMoves().Result)
            {
                if (currentGameState.Whiteturn && currentGameState.WhiteInCheck)
                {
                    result = System.Windows.Forms.MessageBox.Show("White in checkmate", "Game over", System.Windows.Forms.MessageBoxButtons.AbortRetryIgnore);
                }
                else if (!currentGameState.Whiteturn && currentGameState.BlackInCheck)
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
                result = System.Windows.Forms.MessageBox.Show("There have been 50 moves and nothing has happened", "Game over", System.Windows.Forms.MessageBoxButtons.AbortRetryIgnore);
            }

            switch (result)
            {
                case System.Windows.Forms.DialogResult.Abort:
                    this.Exit();
                    break;

                case System.Windows.Forms.DialogResult.Retry:
                    ApiCalls.ResetBoard();
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


            


            if (currentGameState.ChoosingPromotion)
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

            for (int x = 0; x < currentGameState.PieceGrid.GetLength(1); x++)
            {
                //Checking for a pawn in the top row:
                if (currentGameState.PieceGrid[0, x] != null && currentGameState.PieceGrid[0, x].PieceType == PieceTypes.Pawn)
                {
                    promotion = true;
                    isWhite = true;
                    pawnLocation = new Point(x, 0);
                }

                //Checking for a pawn in the bottom row:
                if (currentGameState.PieceGrid[currentGameState.PieceGrid.GetLength(0) - 1, x] != null && currentGameState.PieceGrid[currentGameState.PieceGrid.GetLength(0) - 1, x].PieceType == PieceTypes.Pawn)
                {
                    promotion = true;
                    isWhite = false;
                    pawnLocation = new Point(x, 0);
                }
            }

            return (promotion, isWhite, pawnLocation);
        }


        public async Task GetGameState()
        {
            currentGameState = DecodeFEN(await ApiCalls.MakeFEN());
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
                    switch (rows[y][i])
                    {
                        case 'p':
                            gamestate.PieceGrid[y, x] = new Pawn(false);
                            break;

                        case 'P':
                            gamestate.PieceGrid[y, x] = new Pawn(true);
                            break;

                        case 'b':
                            gamestate.PieceGrid[y, x] = new Bishop(false);
                            break;

                        case 'B':
                            gamestate.PieceGrid[y, x] = new Bishop(true);
                            break;

                        case 'n':
                            gamestate.PieceGrid[y, x] = new Knight(false);
                            break;

                        case 'N':
                            gamestate.PieceGrid[y, x] = new Knight(true);
                            break;

                        case 'k':
                            gamestate.PieceGrid[y, x] = new King(false);
                            break;

                        case 'K':
                            gamestate.PieceGrid[y, x] = new King(true);
                            break;

                        case 'r':
                            gamestate.PieceGrid[y, x] = new Rook(false);
                            break;

                        case 'R':
                            gamestate.PieceGrid[y, x] = new Rook(true);
                            break;

                        case 'q':
                            gamestate.PieceGrid[y, x] = new Queen(false);
                            break;

                        case 'Q':
                            gamestate.PieceGrid[y, x] = new Queen(true);
                            break;

                        default:
                            x += (int)char.GetNumericValue(rows[y][i]) - 1;
                            break;
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
            else if(ending[2] == "b")
            {
                gamestate.BlackInCheck = false;
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