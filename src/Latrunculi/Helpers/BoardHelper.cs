using Latrunculi.Entities;
using Latrunculi.Entities.Enums;
using Latrunculi.Extensions;
using System;
using System.Collections.Generic;

namespace Latrunculi.Helpers
{
    public static class BoardHelper
    {
        public static int LatrunculiBoardWidth = 8;

        public static int LatrunculiBoardHeigth = 7;

        public static SquareStatus[,] GetPreviousBoardPosition(BoardPosition currentPosition, MoveStoneCommand command)
        {
            var updatedBoard = currentPosition.Board;

            var currentColor = updatedBoard[command.To.Item1, command.To.Item2];
            var opponentColor = currentColor.GetOponentColor();

            updatedBoard[command.To.Item1, command.To.Item2] = SquareStatus.Empty;
            updatedBoard[command.From.Item1, command.From.Item2] = currentColor;

            foreach (var oponent in currentPosition.RemovedStonesInLastMove)
            {
                updatedBoard[oponent.Item1, oponent.Item2] = opponentColor;
            }

            return updatedBoard;
        }

        public static SquareStatus[,] GetNextBoardPosition(BoardPosition currentPosition, MoveStoneCommand command)
        {
            var updatedBoard = currentPosition.Board;

            var currentColor = updatedBoard[command.From.Item1, command.From.Item2];

            updatedBoard[command.From.Item1, command.From.Item2] = SquareStatus.Empty;
            updatedBoard[command.To.Item1, command.To.Item2] = currentColor;

            foreach (var oponent in currentPosition.RemovedStonesInLastMove)
            {
                updatedBoard[oponent.Item1, oponent.Item2] = SquareStatus.Empty;
            }

            return updatedBoard;
        }

        public static SquareStatus GetBottomNeighbour(this SquareStatus[,] currentBoard, Tuple<int, int> myPosition, int howFar)
        {
            return currentBoard[myPosition.Item1 + howFar, myPosition.Item2];
        }

        public static SquareStatus GetTopNeighbour(this SquareStatus[,] currentBoard, Tuple<int, int> myPosition, int howFar)
        {
            return currentBoard[myPosition.Item1 - howFar, myPosition.Item2];
        }

        public static SquareStatus GetRightNeighbour(this SquareStatus[,] currentBoard, Tuple<int, int> myPosition, int howFar)
        {
            return currentBoard[myPosition.Item1, myPosition.Item2 + howFar];
        }

        public static SquareStatus GetLeftNeighbour(this SquareStatus[,] currentBoard, Tuple<int, int> myPosition, int howFar)
        {
            return currentBoard[myPosition.Item1, myPosition.Item2 - howFar];
        }

        public static SquareStatus GetBottomRightNeighbour(this SquareStatus[,] currentBoard, Tuple<int, int> myPosition, int howFar)
        {
            return currentBoard[myPosition.Item1 + howFar, myPosition.Item2 + howFar];
        }

        public static SquareStatus GetBottomLeftNeighbour(this SquareStatus[,] currentBoard, Tuple<int, int> myPosition, int howFar)
        {
            return currentBoard[myPosition.Item1 + howFar, myPosition.Item2 - howFar];
        }

        public static SquareStatus GetTopRightNeighbour(this SquareStatus[,] currentBoard, Tuple<int, int> myPosition, int howFar)
        {
            return currentBoard[myPosition.Item1 - howFar, myPosition.Item2 + howFar];
        }

        public static SquareStatus GetTopLeftNeighbour(this SquareStatus[,] currentBoard, Tuple<int, int> myPosition, int howFar)
        {
            return currentBoard[myPosition.Item1 - howFar, myPosition.Item2 - howFar];
        }

        public static Tuple<int, int> GetBottomNeighbourCoords(this SquareStatus[,] currentBoard, Tuple<int, int> myPosition, int howFar)
        {
            return new Tuple<int,int>(myPosition.Item1 + howFar, myPosition.Item2);
        }

        public static Tuple<int, int> GetTopNeighbourCoords(this SquareStatus[,] currentBoard, Tuple<int, int> myPosition, int howFar)
        {
            return new Tuple<int,int>(myPosition.Item1 - howFar, myPosition.Item2);
        }

        public static Tuple<int, int> GetRightNeighbourCoords(this SquareStatus[,] currentBoard, Tuple<int, int> myPosition, int howFar)
        {
            return new Tuple<int,int>(myPosition.Item1, myPosition.Item2 + howFar);
        }

        public static Tuple<int, int> GetLeftNeighbourCoords(this SquareStatus[,] currentBoard, Tuple<int, int> myPosition, int howFar)
        {
            return new Tuple<int,int>(myPosition.Item1, myPosition.Item2 - howFar);
        }

        public static Tuple<int, int> GetBottomRightNeighbourCoords(this SquareStatus[,] currentBoard, Tuple<int, int> myPosition, int howFar)
        {
            return new Tuple<int,int>(myPosition.Item1 + howFar, myPosition.Item2 + howFar);
        }

        public static Tuple<int, int> GetBottomLeftNeighbourCoords(this SquareStatus[,] currentBoard, Tuple<int, int> myPosition, int howFar)
        {
            return new Tuple<int,int>(myPosition.Item1 + howFar, myPosition.Item2 - howFar);
        }

        public static Tuple<int, int> GetTopRightNeighbourCoords(this SquareStatus[,] currentBoard, Tuple<int, int> myPosition, int howFar)
        {
            return new Tuple<int,int>(myPosition.Item1 - howFar, myPosition.Item2 + howFar);
        }

        public static Tuple<int, int> GetTopLeftNeighbourCoords(this SquareStatus[,] currentBoard, Tuple<int, int> myPosition, int howFar)
        {
            return new Tuple<int,int>(myPosition.Item1 - howFar, myPosition.Item2 - howFar);
        }

        public static Tuple<int, int> GetTopLeftNeighbour(this SquareStatus[,] currentBoard, Tuple<int, int> myPosition, int howFar, SquareStatus color)
        {
            var a = myPosition.Item1 - howFar;
            var b = myPosition.Item2 - howFar;

            return a > 0 && b > 0 &&
                currentBoard[a, b] == color ? new Tuple<int, int>(a, b) : null;
        }

        public static Tuple<int, int> GetBottomLeftNeighbour(this SquareStatus[,] currentBoard, Tuple<int, int> myPosition, int howFar, SquareStatus color)
        {
            var a = myPosition.Item1 + howFar;
            var b = myPosition.Item2 - howFar;

            return a < LatrunculiBoardHeigth && b > 0 &&
                currentBoard[a, b] == color ? new Tuple<int, int>(a, b) : null;
        }

        public static Tuple<int, int> GetLeftNeighbour(this SquareStatus[,] currentBoard, Tuple<int, int> myPosition, int howFar, SquareStatus color)
        {
            var b = myPosition.Item2 - howFar;

            return b > 0 &&
                currentBoard[myPosition.Item1, b] == color ? new Tuple<int, int>(myPosition.Item1, b) : null;
        }

        public static Tuple<int, int> GetTopNeighbour(this SquareStatus[,] currentBoard, Tuple<int, int> myPosition, int howFar, SquareStatus color)
        {
            var a = myPosition.Item1 - howFar;

            return a > 0 && 
                currentBoard[a, myPosition.Item2] == color ? new Tuple<int, int>(a, myPosition.Item2) : null;
        }

        public static Tuple<int, int> GetBottomNeighbour(this SquareStatus[,] currentBoard, Tuple<int, int> myPosition, int howFar, SquareStatus color)
        {
            var a = myPosition.Item1 + howFar;

            return a < LatrunculiBoardHeigth &&
                currentBoard[a, myPosition.Item2] == color ? new Tuple<int, int>(a, myPosition.Item2) : null;
        }

        public static Tuple<int, int> GetTopRightNeighbour(this SquareStatus[,] currentBoard, Tuple<int, int> myPosition, int howFar, SquareStatus color)
        {
            var a = myPosition.Item1 - howFar;
            var b = myPosition.Item2 + howFar;

            return a > 0 && b < LatrunculiBoardWidth &&
                currentBoard[a, b] == color ? new Tuple<int, int>(a, b) : null;
        }

        public static Tuple<int, int> GetBottomRightNeighbour(this SquareStatus[,] currentBoard, Tuple<int, int> myPosition, int howFar, SquareStatus color)
        {
            var a = myPosition.Item1 + howFar;
            var b = myPosition.Item2 + howFar;

            return a < LatrunculiBoardHeigth && b < LatrunculiBoardWidth &&
                currentBoard[a, b] == color ? new Tuple<int, int>(a, b) : null;
        }

        public static Tuple<int, int> GetRightNeighbour(this SquareStatus[,] currentBoard, Tuple<int, int> myPosition, int howFar, SquareStatus color)
        {
            var b = myPosition.Item2 + howFar;

            return b < LatrunculiBoardWidth &&
                currentBoard[myPosition.Item1, b] == color ? new Tuple<int, int>(myPosition.Item1, b) : null;
        }

        public static IList<Tuple<int, int>> GetNeighbours(this SquareStatus[,] currentBoard, Tuple<int, int> myPosition, SquareStatus colorToSearch)
        {
            var result = new List<Tuple<int, int>>();

            //left neighbour
            if (myPosition.Item2 - 1 >= 0 && currentBoard[myPosition.Item1, myPosition.Item2 - 1] == colorToSearch)
            {
                result.Add(new Tuple<int, int>(myPosition.Item1, myPosition.Item2 - 1));
            }

            //right neighbour
            if (myPosition.Item2 + 1 < LatrunculiBoardWidth && currentBoard[myPosition.Item1, myPosition.Item2 + 1] == colorToSearch)
            {
                result.Add(new Tuple<int, int>(myPosition.Item1, myPosition.Item2 + 1));
            }

            //top neighbour
            if (myPosition.Item1 - 1 >= 0 && currentBoard[myPosition.Item1 - 1, myPosition.Item2] == colorToSearch)
            {
                result.Add(new Tuple<int, int>(myPosition.Item1 - 1, myPosition.Item2));
            }

            //bottom neighbour
            if (myPosition.Item1 + 1 < LatrunculiBoardHeigth && currentBoard[myPosition.Item1 + 1, myPosition.Item2] == colorToSearch)
            {
                result.Add(new Tuple<int, int>(myPosition.Item1 + 1, myPosition.Item2));
            }

            return result;
        }

        public static bool DictionaryByRankContainsMove(this Dictionary<int, HashSet<MoveStoneCommand>> dic, int rank, MoveStoneCommand command)
        {
            if (!dic.ContainsKey(rank))
            {
                return false;
            }

            if (dic[rank].Contains(command))
            {
                return true;
            }

            return false;
        }
    }
}
