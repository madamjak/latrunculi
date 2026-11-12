using Latrunculi.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Latrunculi.Logic.Entities
{
    public class MyPositionStats
    {
        public MyPositionStats()
        {
            LegalMoves = new List<Tuple<int, int>>();
            CommandsToTryEscape = new List<MoveStoneCommand>();
            CommandsForBeingRemoved = new List<MoveStoneCommand>();
        }

        public Tuple<int, int> MyPosition { get; set; }

        public IList<Tuple<int, int>> LegalMoves { get; set; }

        public int OptionsOfBeingRemoved { get; set; }

        public IList<MoveStoneCommand> CommandsForBeingRemoved { get; set; }

        public IList<MoveStoneCommand> CommandsToTryEscape { get; set; }

        public IList<MoveStoneCommand> CommandsToTryToAttack { get; set; }

        public IList<MoveStoneCommand> CommandsForBeingDeffensive { get; set; }

        public int OptionsForBeingDefensive { get; set; }

        public int OptionsToTryToAttack { get; set; }

        public IList<Tuple<int, int>> MyNeighbours { get; set; }

        public IList<Tuple<int, int>> OponentNeightbours { get; set; }
    }
}
