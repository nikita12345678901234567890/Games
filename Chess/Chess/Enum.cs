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

    public enum MoveType
    { 
        None,
        Normal,
        EnPassant
    };
}