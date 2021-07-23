using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chess
{
    public struct PiecePromotion
    {
        public Point Queen;
        public Point Rook;
        public Point Bishop;
        public Point Knight;
        public bool white;

        public PiecePromotion(bool white, int x)
        {
            this.white = white;
            if (white)
            {
                Queen = new Point(x, 0);
                Rook = new Point(x, 1);
                Bishop = new Point(x, 2);
                Knight = new Point(x, 3);
            }
            else
            {
                Queen = new Point(x, 7);
                Rook = new Point(x, 6);
                Bishop = new Point(x, 5);
                Knight = new Point(x, 4);
            }
        }
    }
}

/*
 * FizzBuzz but no conditions
 */