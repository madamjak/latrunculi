using Latrunculi.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Latrunculi.Logic.Entities
{
    public class OptionsOfBeingRemoved
    {
        public OptionsOfBeingRemoved()
        {
            CommandsToRemove = new List<MoveStoneCommand>();
            CommandsToTryEscape = new List<MoveStoneCommand>();
        }

        public IList<MoveStoneCommand> CommandsToRemove { get; set; }

        public int NumberOfOptions { get; set; }

        public IList<MoveStoneCommand> CommandsToTryEscape { get; set; }
    }
}
