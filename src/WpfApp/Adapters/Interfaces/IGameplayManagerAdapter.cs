using Latrunculi.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using WpfApp.Models;

namespace WpfApp.Adapters.Interfaces
{
    public interface IGameplayManagerAdapter
    {
        GameplayModel GetStartingGamePlayModel(GameplayModel model);

        void PerformMove(MoveStoneCommand command, GameplayModel model, CancellationToken token);

        void Undo(GameplayModel model);

        void Redo(GameplayModel model);

        void Replay(GameplayModel model);

        void DisplayPossibleMoves(GameplayModel model, Square selectedFrom);

        public void HidePreviouslySuggestedPossibleMoves(GameplayModel model);

        public bool IsCurrentPlayerComputer();

        public bool IsGameOpen();

        MoveStoneCommand GetCurrentPlayerBestMove(CancellationToken token);
        
        void DisplayBestPossibleMove(MoveStoneCommand bestMove, GameplayModel model);

        void HidePreviouslyBestPossibleMove(GameplayModel model);

        MoveCommandHistoryItem Autoplay(CancellationToken token);

        void UpdateModel(GameplayModel model, MoveCommandHistoryItem history);
    }
}
