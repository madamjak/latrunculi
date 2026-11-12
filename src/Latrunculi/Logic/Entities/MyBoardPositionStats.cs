using System;
using System.Collections.Generic;
using System.Text;

namespace Latrunculi.Logic.Entities
{
    public class MyBoardPositionStats
    {
        public IList<MyPositionStats> PositionsStats { get; set; }

        public IList<MyPositionStats> OponentStats { get; set; }

        public int MyCount { get; set; }

        public int OponentCount { get; set; }

        public int EmptySpaces { get; set; }
    }
}
