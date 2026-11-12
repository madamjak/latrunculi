using Latrunculi.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Latrunculi.Helpers
{
    public static class BoardColorsHelper
    {
        public static SquareStatus GetOponentColor(this SquareStatus current)
        {
            return current == SquareStatus.Black ? SquareStatus.White : SquareStatus.Black;
        }

        public static SquareStatus GetBoardColor(this PlayerColor playerColor)
        {
            return playerColor == PlayerColor.Black ? SquareStatus.Black : SquareStatus.White;
        }
    }
}
