using Latrunculi.Entities;
using Latrunculi.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Latrunculi.Managers.Interfaces
{
    public interface IGameplayManager
    {
        Player GetCurrentPlayer();

        PlayerColor GetCurrentPlayerColor();

        BoardPosition GetCurrentPosition();

        bool IsUndoPossible();

        MoveCommandHistoryItem Undo();

        bool IsRedoPossible();

        MoveCommandHistoryItem Redo();

        IEnumerable<MoveCommandHistoryItem> GetHistory();

        MoveStoneCommand GetCurrentPlayerBestMoveHint(CancellationToken cancellationToken);

        bool IsValidMove(MoveStoneCommand command);

        MoveCommandHistoryItem PerformMove(MoveStoneCommand command, CancellationToken token);

        IDictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> GetCurrentPlayerPossibleMoves();
    }
}
