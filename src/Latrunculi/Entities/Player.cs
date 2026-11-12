using Latrunculi.Entities.Enums;
using System;

namespace Latrunculi.Entities
{
    [Serializable]
    public class Player
    {
        public PlayerType Type { get; set; }

        public PlayerLevel Level { get; set; }
    }
}
