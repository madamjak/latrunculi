using Latrunculi.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;
using WpfApp.Models;

namespace WpfApp.Helpers
{
    public static class BoardImagesHelper
    {
        public static BitmapImage GetImageSource(this UiSquareStatus squareStatus)
        {
            var path = Environment.CurrentDirectory + "\\Content\\BoardImages\\";

            switch (squareStatus)
            {
                case UiSquareStatus.Empty:
                    path += "emptySquare.png";
                    break;
                case UiSquareStatus.Black:
                    path += "black.png";
                    break;
                case UiSquareStatus.White:
                    path += "white.png";
                    break;
                case UiSquareStatus.WhiteSelected:
                    path += "whiteSelected.png";
                    break;
                case UiSquareStatus.BlackSelected:
                    path += "blackSelected.png";
                    break;
                case UiSquareStatus.PossibleMove:
                    path += "possibleMove.png";
                    break;
                case UiSquareStatus.WhiteBestMoveFrom:
                    path += "whiteBestMove.png";
                    break;
                case UiSquareStatus.BlackBestMoveFrom:
                    path += "blackBestMove.png";
                    break;
                case UiSquareStatus.BestPossibleMove:
                    path += "bestMoveHint.png";
                    break;
            }

            return new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));
        }

        public static BitmapImage GetBoardColumnLabelImageSource(int column)
        {
            var path = Environment.CurrentDirectory + "\\Content\\BoardImages\\" + GetColumn(column) + ".png";

            return new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));
        }

        public static BitmapImage GetBoardRowLabelImageSource(int column)
        {
            var path = Environment.CurrentDirectory + "\\Content\\BoardImages\\" + column + ".png";

            return new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));
        }

        private static char GetColumn(int column)
        {
            return (char)(column + 97);
        }

        public static UiSquareStatus GetSelectionStatus(this UiSquareStatus status)
        {
            switch (status)
            {
                case UiSquareStatus.Black:
                    return UiSquareStatus.BlackSelected;
                case UiSquareStatus.White:
                    return UiSquareStatus.WhiteSelected;
                case UiSquareStatus.BlackBestMoveFrom:
                    return UiSquareStatus.BlackSelected;
                case UiSquareStatus.WhiteBestMoveFrom:
                    return UiSquareStatus.WhiteSelected;
            }

            throw new ApplicationException("unsuported");
        }

        public static UiSquareStatus GetUnSelectionStatus(this UiSquareStatus status)
        {
            switch (status)
            {
                case UiSquareStatus.BlackSelected:
                    return UiSquareStatus.Black;
                case UiSquareStatus.WhiteSelected:
                    return UiSquareStatus.White;
            }

            throw new ApplicationException("unsuported");
        }

        public static UiSquareStatus GetBestMoveSelectionStatus(this UiSquareStatus status)
        {
            switch (status)
            {
                case UiSquareStatus.Black:
                    return UiSquareStatus.BlackBestMoveFrom;
                case UiSquareStatus.White:
                    return UiSquareStatus.WhiteBestMoveFrom;
            }

            throw new ApplicationException("unsuported");
        }

        public static UiSquareStatus GetBestMoveUnSelectionStatus(this UiSquareStatus status)
        {
            switch (status)
            {
                case UiSquareStatus.BlackBestMoveFrom:
                    return UiSquareStatus.Black;
                case UiSquareStatus.WhiteBestMoveFrom:
                    return UiSquareStatus.White;
            }

            throw new ApplicationException("unsuported");
        }

        public static UiSquareStatus GetUiSquareStatus(this SquareStatus squareStatus)
        {
            switch (squareStatus)
            {
                case SquareStatus.Empty:
                    return UiSquareStatus.Empty;
                case SquareStatus.Black:
                    return UiSquareStatus.Black;
                case SquareStatus.White:
                    return UiSquareStatus.White;
            }

            throw new ApplicationException("unsuported");
        }
    }
}
