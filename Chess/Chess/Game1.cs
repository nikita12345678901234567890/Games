using SharedLibrary.Pieces;
using SharedLibrary;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;

using System.Collections.Generic;
using System.Text.Json;

namespace Chess
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public static Texture2D Pixel;

        public static Dictionary<(PieceTypes, bool), Texture2D> Textures;

        
        public MouseState Lastms;

        


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



            Lastms = ms;
            base.Update(gameTime);
        }

        

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            

            spriteBatch.End();

            base.Draw(gameTime);
        }


        public string MakeFEN(Piece[,] PieceGrid)
        {
            return "yeet";
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

            return grid;
        }
    }
}