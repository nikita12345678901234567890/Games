using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary
{
    public struct PiecePromotion
    {
        public Square Queen;
        public Square Rook;
        public Square Bishop;
        public Square Knight;
        public bool white;

        public PiecePromotion(bool white, int x)
        {
            this.white = white;
            //if (white)
            //{
                Queen = new Square(x, 0);
                Rook = new Square(x, 1);
                Bishop = new Square(x, 2);
                Knight = new Square(x, 3);
            //}
            //else
            //{
            //    Queen = new Square(x, 7);
            //    Rook = new Square(x, 6);
            //    Bishop = new Square(x, 5);
            //    Knight = new Square(x, 4);
            //}
        }
    }
}