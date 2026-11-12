using Latrunculi.Entities.Enums;
using System;
using System.Collections.Generic;

namespace Latrunculi.Entities
{
    [Serializable]
    public class Game
    {
        public Game()
        {
            MoveCommandsHistory = new List<MoveCommandHistoryItem>();
        }

        public Player BlackPlayer { get; set; }

        public Player WhitePlayer { get; set; }

        public PlayerColor CurrentPlayer { get; set; }

        public BoardPosition CurrentPosition { get; set; }

        public IList<MoveCommandHistoryItem> MoveCommandsHistory { get; set; }
    }
}
