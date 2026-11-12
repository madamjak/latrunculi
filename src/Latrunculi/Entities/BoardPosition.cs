using Latrunculi.Entities.Enums;
using System;
using System.Collections.Generic;

namespace Latrunculi.Entities
{
    [Serializable]
    public class BoardPosition
    {
        public BoardPosition()
        {
            RemovedStonesInLastMove = new List<Tuple<int, int>>();
        }

        public SquareStatus[,] Board { get; set; }

        public BoardPositionStatus Status { get; set; }

        public int NumberOfMovesWithoutRemovedStoneCounter { get; set; }

        public IList<Tuple<int, int>> RemovedStonesInLastMove { get; set; }

        public SquareStatus LastPlayerColor { get; set; }

        public MoveStoneCommand LastMoveStoneCommand { get; set; }

        public int BlackPlayerStonesCount { get; set; }

        public int WhitePlayerStonesCount { get; set; }
    }
}
