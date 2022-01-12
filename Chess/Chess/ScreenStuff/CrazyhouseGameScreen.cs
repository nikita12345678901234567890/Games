using Chess.HackyStuff;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using SharedLibrary;
using SharedLibrary.Pieces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public class CrazyhouseGameScreen : Screen
    {
        ContentManager content;
        GraphicsDeviceManager graphics;

        SpriteFont font2;

        public static Dictionary<(PieceTypes, bool), Texture2D> Textures;

        public static Texture2D Pixel;

        public int squaresize;

        public List<Point> HighlightedSquares;

        bool spectating = false;

        PiecePromotion choices;

        GameState currentGameState;

        Guid playerID;

        Guid gameID = Guid.Empty;

        bool amWhite;

        const int delayMillis = 250;

        TimeSpan prevTime;

        string lastFEN = String.Empty;

        bool dragging = false;
        Piece draggingPiece;

        public Dictionary<bool, Dictionary<PieceTypes, Stack<Piece>>> AvailablePieces = new Dictionary<bool, Dictionary<PieceTypes, Stack<Piece>>>();


        public CrazyhouseGameScreen(ContentManager content, GraphicsDeviceManager graphics, UpdateResult options)
        {
            this.content = content;
            this.graphics = graphics;

            graphics.PreferredBackBufferWidth = 900;
            graphics.ApplyChanges();

            squaresize = graphics.PreferredBackBufferHeight / 8;

            HighlightedSquares = new List<Point>();

            //Loading piece textures:
            Textures = new Dictionary<(PieceTypes, bool), Texture2D>();
            Textures.Add((PieceTypes.Pawn, true), content.Load<Texture2D>("whitepawn"));
            Textures.Add((PieceTypes.Pawn, false), content.Load<Texture2D>("blackpawn"));
            Textures.Add((PieceTypes.Rook, true), content.Load<Texture2D>("whiterook"));
            Textures.Add((PieceTypes.Rook, false), content.Load<Texture2D>("blackrook"));
            Textures.Add((PieceTypes.Knight, true), content.Load<Texture2D>("whiteknight"));
            Textures.Add((PieceTypes.Knight, false), content.Load<Texture2D>("blackknight"));
            Textures.Add((PieceTypes.Bishop, true), content.Load<Texture2D>("whitebishop"));
            Textures.Add((PieceTypes.Bishop, false), content.Load<Texture2D>("blackbishop"));
            Textures.Add((PieceTypes.King, true), content.Load<Texture2D>("whiteking"));
            Textures.Add((PieceTypes.King, false), content.Load<Texture2D>("blackking"));
            Textures.Add((PieceTypes.Queen, true), content.Load<Texture2D>("whitequeen"));
            Textures.Add((PieceTypes.Queen, false), content.Load<Texture2D>("blackqueen"));

            Pixel = new Texture2D(graphics.GraphicsDevice, 1, 1);
            Pixel.SetData(new[] { Color.White });


            font2 = content.Load<SpriteFont>("font2");

            //Initializing the Dictionary of Dictionary of Stack of Piece:
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


            //Actually starting the game:
            if (options.startNewGame)
            {
                if (options.startSpectating)
                {
                    System.Windows.Forms.MessageBox.Show("You can't do that.", "", System.Windows.Forms.MessageBoxButtons.OK);
                }
                else
                {
                    gameID = Guid.NewGuid();

                    GameIDNotifierForm.Instance.SetGameID(gameID);
                    if (!GameIDNotifierForm.Instance.Visible)
                    {
                        GameIDNotifierForm.Instance.Show();
                    }
                }
            }
            else
            {
                GameIDEntryForm.Instance.ShowDialog();
                gameID = GameIDEntryForm.Instance.GameID;
                GameIDEntryForm.Instance.Close();
            }

            playerID = Task.Run(async () => await ApiCalls.GetPlayerId(gameID, options.startPlayingCrazyhouse)).Result;

            if (options.startSpectating)
            {
                spectating = true;

                Task.Run(async () => await GetGameState()).Wait();
            }
            else
            {
                bool? color = Task.Run(async () => await ApiCalls.GetGameColor(gameID, playerID, options.startPlayingWhite)).Result;
                if (color == null)
                {
                    var BoxResult = System.Windows.Forms.MessageBox.Show("The game is full", "Press ok the spectate and cancel to go back.", System.Windows.Forms.MessageBoxButtons.OKCancel);

                    switch (BoxResult)
                    {
                        case System.Windows.Forms.DialogResult.OK:
                            spectating = true;
                            break;

                        case System.Windows.Forms.DialogResult.Cancel:
                            spectating = false;
                            throw new Exception("This should go back to menu");
                            break;
                    }
                }
                else
                {
                    amWhite = (bool)color;

                    Task.Run(async () => await GetGameState()).Wait();
                }
            }


        }

        UpdateResult CheckIfGameOver(GameTime gameTime)
        {
            UpdateResult finalResult = new UpdateResult();

            System.Windows.Forms.DialogResult result = System.Windows.Forms.DialogResult.None;

            var checkForMoveResults = Task.Run(async () => await ApiCalls.CheckForNoMoves(gameID)).Result;
            if (checkForMoveResults)
            {
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
                result = System.Windows.Forms.MessageBox.Show("There have been 50 moves and nothing has happened", "Game over", System.Windows.Forms.MessageBoxButtons.AbortRetryIgnore);
            }

            switch (result)
            {
                case System.Windows.Forms.DialogResult.Abort:
                    finalResult.exitScreen = true;
                    break;

                case System.Windows.Forms.DialogResult.Retry:
                    finalResult.startNewGame = true;
                    break;

                case System.Windows.Forms.DialogResult.Ignore:
                    spectating = true;
                    break;
            }

            return finalResult;
        }

        public override UpdateResult Update(GameTime gameTime, bool IsActive)
        {
            UpdateResult result = new UpdateResult();

            UpdateResult GameOverResult;


            prevTime += gameTime.ElapsedGameTime;
            if (prevTime.TotalMilliseconds >= delayMillis)
            {
                Task.Run(async () => await GetGameState()).Wait();
            }


            if (spectating)
            {

            }


            //Checking if mouse clicked:
            else if (((InputManager.MouseState.LeftButton == ButtonState.Pressed && InputManager.LastMouseState.LeftButton == ButtonState.Released) || dragging)
                  && graphics.GraphicsDevice.Viewport.Bounds.Contains(InputManager.MouseState.Position)
                  && currentGameState.Whiteturn == amWhite && IsActive)
            {
                if (InputManager.MouseState.Position.X < 800 && !dragging)
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
                        GameOverResult = CheckIfGameOver(gameTime);
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


                            GameOverResult = CheckIfGameOver(gameTime);
                        }
                    }
                }
                else
                {
                    if (dragging)
                    {
                        if (InputManager.MouseState.LeftButton == ButtonState.Released && InputManager.MouseState.Position.X < 800)
                        {
                            dragging = !Task.Run(async () => await ApiCalls.PlacePiece(gameID, playerID, draggingPiece.PieceType, PositionToCell(InputManager.MouseState.Position))).Result;
                        }
                    }
                    else
                    {
                        if (InputManager.MouseState.Position.Y > 393)
                        {
                            if (InputManager.MouseState.Position.Y <= 473 && AvailablePieces[amWhite][PieceTypes.Pawn].Count > 0)
                            {
                                draggingPiece = new Pawn(amWhite);
                                dragging = true;
                            }
                            else if (InputManager.MouseState.Position.Y <= 554 && AvailablePieces[amWhite][PieceTypes.Knight].Count > 0)
                            {
                                draggingPiece = new Knight(amWhite);
                                dragging = true;
                            }
                            else if (InputManager.MouseState.Position.Y <= 635 && AvailablePieces[amWhite][PieceTypes.Bishop].Count > 0)
                            {
                                draggingPiece = new Bishop(amWhite);
                                dragging = true;
                            }
                            else if (InputManager.MouseState.Position.Y <= 712 && AvailablePieces[amWhite][PieceTypes.Rook].Count > 0)
                            {
                                draggingPiece = new Rook(amWhite);
                                dragging = true;
                            }
                            else if (InputManager.MouseState.Position.Y <= 800 && AvailablePieces[amWhite][PieceTypes.Queen].Count > 0)
                            {
                                draggingPiece = new Queen(amWhite);
                                dragging = true;
                            }
                        }
                    }
                }
            }

            return result;
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

        public override void Draw(SpriteBatch spriteBatch)
        {
            graphics.GraphicsDevice.Clear(Color.Purple);

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


            //Drawing promotion choices:
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

            //Drawing available pieces:
            int offset = 7;
            Texture2D pieceTexture;
            for (int i = 0; i < 5; i++)
            {
                pieceTexture = Textures[(PieceTypes.Queen - i, !amWhite)];

                if (i == 4)
                {
                    spriteBatch.Draw(pieceTexture, CellCenter(new Point(8, i)) - new Vector2(0, (20 * (i)) + offset), null, Color.White, 0, new Vector2(pieceTexture.Width / 2, pieceTexture.Height / 2), 0.8f, SpriteEffects.None, 0);
                }
                else
                {
                    spriteBatch.Draw(pieceTexture, CellCenter(new Point(8, i)) - new Vector2(0, (20 * (i)) + offset), null, Color.White, 0, new Vector2(pieceTexture.Width / 2, pieceTexture.Height / 2), 0.4f, SpriteEffects.None, 0);
                }
            }

            for (int i = 4; i >= 0; i--)
            {
                pieceTexture = Textures[(PieceTypes.Pawn + i, amWhite)];

                if (i == 0)
                {
                    spriteBatch.Draw(pieceTexture, CellCenter(new Point(8, i + 4)) - new Vector2(0, (20 * (i)) + offset), null, Color.White, 0, new Vector2(pieceTexture.Width / 2, pieceTexture.Height / 2), 0.8f, SpriteEffects.None, 0);
                }
                else
                {
                    spriteBatch.Draw(pieceTexture, CellCenter(new Point(8, i + 4)) - new Vector2(0, (20 * (i)) + offset), null, Color.White, 0, new Vector2(pieceTexture.Width / 2, pieceTexture.Height / 2), 0.4f, SpriteEffects.None, 0);
                }
            }

            //Drawing the numbers of pieces available:
            spriteBatch.Draw(Pixel, new Vector2(880, 60), null, Color.Chocolate, 0, new Vector2(0, 0), Vector2.One * 20, SpriteEffects.None, 0);
            spriteBatch.Draw(Pixel, new Vector2(880, 132), null, Color.Chocolate, 0, new Vector2(0, 0), Vector2.One * 20, SpriteEffects.None, 0);
            spriteBatch.Draw(Pixel, new Vector2(880, 216), null, Color.Chocolate, 0, new Vector2(0, 0), Vector2.One * 20, SpriteEffects.None, 0);
            spriteBatch.Draw(Pixel, new Vector2(880, 295), null, Color.Chocolate, 0, new Vector2(0, 0), Vector2.One * 20, SpriteEffects.None, 0);
            spriteBatch.Draw(Pixel, new Vector2(880, 373), null, Color.Chocolate, 0, new Vector2(0, 0), Vector2.One * 20, SpriteEffects.None, 0);

            spriteBatch.Draw(Pixel, new Vector2(880, 453), null, Color.Chocolate, 0, new Vector2(0, 0), Vector2.One * 20, SpriteEffects.None, 0);
            spriteBatch.Draw(Pixel, new Vector2(880, 534), null, Color.Chocolate, 0, new Vector2(0, 0), Vector2.One * 20, SpriteEffects.None, 0);
            spriteBatch.Draw(Pixel, new Vector2(880, 615), null, Color.Chocolate, 0, new Vector2(0, 0), Vector2.One * 20, SpriteEffects.None, 0);
            spriteBatch.Draw(Pixel, new Vector2(880, 692), null, Color.Chocolate, 0, new Vector2(0, 0), Vector2.One * 20, SpriteEffects.None, 0);
            spriteBatch.Draw(Pixel, new Vector2(880, 775), null, Color.Chocolate, 0, new Vector2(0, 0), Vector2.One * 20, SpriteEffects.None, 0);

            int[] topArray = new int[5];
            int[] bottomArray = new int[5];
            if (amWhite)
            {
                topArray[0] = AvailablePieces[false][PieceTypes.Pawn].Count();
                topArray[1] = AvailablePieces[false][PieceTypes.Knight].Count();
                topArray[2] = AvailablePieces[false][PieceTypes.Bishop].Count();
                topArray[3] = AvailablePieces[false][PieceTypes.Rook].Count();
                topArray[4] = AvailablePieces[false][PieceTypes.Queen].Count();

                bottomArray[0] = AvailablePieces[true][PieceTypes.Pawn].Count();
                bottomArray[1] = AvailablePieces[true][PieceTypes.Knight].Count();
                bottomArray[2] = AvailablePieces[true][PieceTypes.Bishop].Count();
                bottomArray[3] = AvailablePieces[true][PieceTypes.Rook].Count();
                bottomArray[4] = AvailablePieces[true][PieceTypes.Queen].Count();
            }
            else
            {
                topArray[0] = AvailablePieces[true][PieceTypes.Pawn].Count();
                topArray[1] = AvailablePieces[true][PieceTypes.Knight].Count();
                topArray[2] = AvailablePieces[true][PieceTypes.Bishop].Count();
                topArray[3] = AvailablePieces[true][PieceTypes.Rook].Count();
                topArray[4] = AvailablePieces[true][PieceTypes.Queen].Count();

                bottomArray[0] = AvailablePieces[false][PieceTypes.Pawn].Count();
                bottomArray[1] = AvailablePieces[false][PieceTypes.Knight].Count();
                bottomArray[2] = AvailablePieces[false][PieceTypes.Bishop].Count();
                bottomArray[3] = AvailablePieces[false][PieceTypes.Rook].Count();
                bottomArray[4] = AvailablePieces[false][PieceTypes.Queen].Count();
            }

            spriteBatch.DrawString(font2, topArray[4].ToString(), new Vector2(883, 57), Color.Black);
            spriteBatch.DrawString(font2, topArray[3].ToString(), new Vector2(883, 129), Color.Black);
            spriteBatch.DrawString(font2, topArray[2].ToString(), new Vector2(883, 213), Color.Black);
            spriteBatch.DrawString(font2, topArray[1].ToString(), new Vector2(883, 292), Color.Black);
            spriteBatch.DrawString(font2, topArray[0].ToString(), new Vector2(883, 370), Color.Black);
            spriteBatch.DrawString(font2, bottomArray[0].ToString(), new Vector2(883, 450), Color.Black);
            spriteBatch.DrawString(font2, bottomArray[1].ToString(), new Vector2(883, 531), Color.Black);
            spriteBatch.DrawString(font2, bottomArray[2].ToString(), new Vector2(883, 612), Color.Black);
            spriteBatch.DrawString(font2, bottomArray[3].ToString(), new Vector2(883, 689), Color.Black);
            spriteBatch.DrawString(font2, bottomArray[4].ToString(), new Vector2(883, 772), Color.Black);

            //Drawing dragging piece:
            if (dragging)
            {
                if (draggingPiece.PieceType == PieceTypes.Pawn)
                {
                    spriteBatch.Draw(Textures[(draggingPiece.PieceType, amWhite)], InputManager.MouseState.Position.ToVector2(), null, Color.White, 0, new Vector2(Textures[(draggingPiece.PieceType, amWhite)].Width / 2, Textures[(draggingPiece.PieceType, amWhite)].Height / 2), 0.8f, SpriteEffects.None, 0);
                }
                else
                {
                    spriteBatch.Draw(Textures[(draggingPiece.PieceType, amWhite)], InputManager.MouseState.Position.ToVector2(), null, Color.White, 0, new Vector2(Textures[(draggingPiece.PieceType, amWhite)].Width / 2, Textures[(draggingPiece.PieceType, amWhite)].Height / 2), 0.4f, SpriteEffects.None, 0);
                }
            }
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
                    }
                    else
                    {
                        switch (rows[y][i])
                        {
                            case 'p':
                                gamestate.PieceGrid[7 - y, 7 - x] = new Pawn(false);
                                break;

                            case 'P':
                                gamestate.PieceGrid[7 - y, 7 - x] = new Pawn(true);
                                break;

                            case 'b':
                                gamestate.PieceGrid[7 - y, 7 - x] = new Bishop(false);
                                break;

                            case 'B':
                                gamestate.PieceGrid[7 - y, 7 - x] = new Bishop(true);
                                break;

                            case 'n':
                                gamestate.PieceGrid[7 - y, 7 - x] = new Knight(false);
                                break;

                            case 'N':
                                gamestate.PieceGrid[7 - y, 7 - x] = new Knight(true);
                                break;

                            case 'k':
                                gamestate.PieceGrid[7 - y, 7 - x] = new King(false);
                                break;

                            case 'K':
                                gamestate.PieceGrid[7 - y, 7 - x] = new King(true);
                                break;

                            case 'r':
                                gamestate.PieceGrid[7 - y, 7 - x] = new Rook(false);
                                break;

                            case 'R':
                                gamestate.PieceGrid[7 - y, 7 - x] = new Rook(true);
                                break;

                            case 'q':
                                gamestate.PieceGrid[7 - y, 7 - x] = new Queen(false);
                                break;

                            case 'Q':
                                gamestate.PieceGrid[7 - y, 7 - x] = new Queen(true);
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

            var availablePieces = ending[4].Split(',');

            UpdateAvailablePieces(true, PieceTypes.Pawn, int.Parse(availablePieces[0]));
            UpdateAvailablePieces(true, PieceTypes.Knight, int.Parse(availablePieces[1]));
            UpdateAvailablePieces(true, PieceTypes.Bishop, int.Parse(availablePieces[2]));
            UpdateAvailablePieces(true, PieceTypes.Rook, int.Parse(availablePieces[3]));
            UpdateAvailablePieces(true, PieceTypes.Queen, int.Parse(availablePieces[4]));

            UpdateAvailablePieces(false, PieceTypes.Pawn, int.Parse(availablePieces[5]));
            UpdateAvailablePieces(false, PieceTypes.Knight, int.Parse(availablePieces[6]));
            UpdateAvailablePieces(false, PieceTypes.Bishop, int.Parse(availablePieces[7]));
            UpdateAvailablePieces(false, PieceTypes.Rook, int.Parse(availablePieces[8]));
            UpdateAvailablePieces(false, PieceTypes.Queen, int.Parse(availablePieces[9]));

            return gamestate;
        }

        private void UpdateAvailablePieces(bool color, PieceTypes piece, int number)
        {
            while (AvailablePieces[color][piece].Count < number)
            {
                switch (piece)
                {
                    case PieceTypes.Pawn:
                        AvailablePieces[color][piece].Push(new Pawn(color));
                        break;

                    case PieceTypes.Knight:
                        AvailablePieces[color][piece].Push(new Knight(color));
                        break;

                    case PieceTypes.Bishop:
                        AvailablePieces[color][piece].Push(new Bishop(color));
                        break;

                    case PieceTypes.Rook:
                        AvailablePieces[color][piece].Push(new Rook(color));
                        break;

                    case PieceTypes.Queen:
                        AvailablePieces[color][piece].Push(new Queen(color));
                        break;
                }
            }

            while (AvailablePieces[color][piece].Count > number)
            {
                AvailablePieces[color][piece].Pop();
            }
        }
    }
}