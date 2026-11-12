using Latrunculi.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Latrunculi.Entities
{
    [Serializable]
    public class MoveCommandHistoryItem
    {
        public MoveStoneCommand Command { get; set; }

        public MoveStoneCommand PreviousCommand { get; set; }

        public PlayerColor PlayerColor { get; set; }

        public PlayerColor PreviousPlayerColor { get; set; }

        public IList<Tuple<int, int>> RemovedOponentStones { get; set; }

        public IList<Tuple<int, int>> PreviousRemovedOponentStones { get; set; }

        public int NumberOfMovesWithoutRemovedStoneCounter { get; set; }

        public int PreviousNumberOfMovesWithoutRemovedStoneCounter { get; set; }

        public BoardPositionStatus BoardStatus { get; set; }

        public BoardPositionStatus PreviousBoardStatus { get; set; }

        public UndoRedo UndoRedo { get; set; }
    }

    public enum UndoRedo
    {
        NotSpecified,
        Undo,
        Redo
    }
}
