using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public class NormalGameScreen : Screen
    {
        public override UpdateResult Update(GameTime gameTime, bool IsActive)
        {
            if (InputManager.MouseState.LeftButton == ButtonState.Pressed && InputManager.LastMouseState.LeftButton == ButtonState.Released && GraphicsDevice.Viewport.Bounds.Contains(InputManager.MouseState.Position) && currentGameState.Whiteturn == amWhite && IsActive)
            {
                Window.Title = $"GameID = {gameID}";
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
        }

        void CheckIfGameOver(GameTime gameTime)
        {
            System.Windows.Forms.DialogResult result = System.Windows.Forms.DialogResult.None;

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

        public override void Draw(SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }
    }
}