using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary
{
    public enum PieceTypes
    {
        Pawn,
        Knight,
        Bishop,
        Rook,
        Queen,
        King
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