using Latrunculi.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Latrunculi.Logic.Entities
{
    public class OptionsToAttack
    {
        public int NumberOfOptions { get; set; }

        public IList<MoveStoneCommand> Commands { get; set; }
    }
}
