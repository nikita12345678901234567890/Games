using System;
using System.Collections.Generic;
using System.Text;

namespace Chess
{
    public enum PieceTypes
    {
        Pawn,
        Rook,
        Bishop,
        Knight,
        King,
        Queen
    };

    public enum MoveTypes
    { 
        None,
        Normal,
        EnPassant,
        CastleRight,
        CastleLeft,
        Promotion
    };

    public enum Directions
    { 
        Up,
        Down,
        Left,
        Right,
        UpRight,
        UpLeft,
        DownRight,
        DownLeft
    };
}