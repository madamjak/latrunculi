using Latrunculi.Entities;
using Latrunculi.Entities.Enums;
using Latrunculi.Extensions;
using Latrunculi.Helpers;
using Latrunculi.Logic;
using Latrunculi.Managers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Latrunculi.UI.Managers
{
    public class GameplayManager : IGameplayManager
    {
        private readonly IGameAi _aiAlgorithm;

        private readonly IGameRules _rules;

        private Stack<MoveCommandHistoryItem> _undoCommands;

        private Stack<MoveCommandHistoryItem> _redoCommands;

        private Game _game;

        public GameplayManager(Game game, IGameRules rules)
        {
            _game = game;
            _rules = rules;
            _aiAlgorithm = new AlphaBetaMinMax(_rules);

            _undoCommands = new Stack<MoveCommandHistoryItem>();
            _redoCommands = new Stack<MoveCommandHistoryItem>();
        }

        public Player GetCurrentPlayer()
        {
            return _game.CurrentPlayer == PlayerColor.White ? _game.WhitePlayer : _game.BlackPlayer;
        }

        public MoveStoneCommand GetCurrentPlayerBestMoveHint(CancellationToken cancellationToken)
        {
            var bestMove = _aiAlgorithm.GetBestPossibleMove(_game.CurrentPosition, _game.CurrentPlayer, GetCurrentPlayer().Level, true, cancellationToken);

            bestMove.StringCommand = bestMove.GetStringCommand();

            return bestMove;
        }

        public BoardPosition GetCurrentPosition()
        {
            return _game.CurrentPosition;
        }

        public IEnumerable<MoveCommandHistoryItem> GetHistory()
        {
            return _game.MoveCommandsHistory;
        }

        public MoveCommandHistoryItem PerformMove(MoveStoneCommand command, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            if (_redoCommands.Count > 0)
            {
                _redoCommands.Clear();
            }

            if(GetCurrentPlayer().Type == PlayerType.Computer)
            {
                command = _aiAlgorithm.GetBestPossibleMove(_game.CurrentPosition, _game.CurrentPlayer, GetCurrentPlayer().Level, false, token);
            }

            if(command.StringCommand == null) 
            {
                command.StringCommand = command.GetStringCommand();
            }

            token.ThrowIfCancellationRequested();

            //these details needs to be from current position
            var historyItem = new MoveCommandHistoryItem()
            {
                PreviousNumberOfMovesWithoutRemovedStoneCounter = _game.CurrentPosition.NumberOfMovesWithoutRemovedStoneCounter,
                PreviousBoardStatus = _game.CurrentPosition.Status,
                PreviousPlayerColor = _game.CurrentPlayer,
                PreviousCommand = _game.CurrentPosition.LastMoveStoneCommand,
                PreviousRemovedOponentStones = _game.CurrentPosition.RemovedStonesInLastMove
            };

            var updatedPosition = _rules.GetUpdatedPosition(_game.CurrentPosition, command);

            _game.CurrentPosition = updatedPosition;
            _game.CurrentPlayer = _rules.GetPlayerOnTurn(updatedPosition);
            _game.CurrentPosition.WhitePlayerStonesCount -= historyItem.PreviousPlayerColor == PlayerColor.Black ? _game.CurrentPosition.RemovedStonesInLastMove.Count : 0;
            _game.CurrentPosition.BlackPlayerStonesCount -= historyItem.PreviousPlayerColor == PlayerColor.White ? _game.CurrentPosition.RemovedStonesInLastMove.Count : 0;

            //and now get updated
            historyItem.Command = command;
            historyItem.RemovedOponentStones = _game.CurrentPosition.RemovedStonesInLastMove;
            historyItem.PlayerColor = _game.CurrentPlayer;
            historyItem.BoardStatus = _game.CurrentPosition.Status;
            historyItem.NumberOfMovesWithoutRemovedStoneCounter = _game.CurrentPosition.NumberOfMovesWithoutRemovedStoneCounter;

            _game.MoveCommandsHistory.Add(historyItem);

            _undoCommands.Push(historyItem);

            return historyItem;
        }

        public MoveCommandHistoryItem Redo()
        {
            var historyItem = _redoCommands.Pop().CopyObject<MoveCommandHistoryItem>();

            _undoCommands.Push(historyItem);

            _game.CurrentPosition.LastMoveStoneCommand = historyItem.Command;
            _game.CurrentPosition.RemovedStonesInLastMove = historyItem.RemovedOponentStones;
            _game.CurrentPosition.NumberOfMovesWithoutRemovedStoneCounter = historyItem.NumberOfMovesWithoutRemovedStoneCounter;
            _game.CurrentPosition.Status = historyItem.BoardStatus;
            _game.CurrentPlayer = historyItem.PlayerColor;
            _game.CurrentPosition.Board = BoardHelper.GetNextBoardPosition(_game.CurrentPosition, historyItem.Command);
            _game.CurrentPosition.WhitePlayerStonesCount -= historyItem.PreviousPlayerColor == PlayerColor.Black ? _game.CurrentPosition.RemovedStonesInLastMove.Count : 0;
            _game.CurrentPosition.BlackPlayerStonesCount -= historyItem.PreviousPlayerColor == PlayerColor.White ? _game.CurrentPosition.RemovedStonesInLastMove.Count : 0;

            historyItem.UndoRedo = UndoRedo.Redo;
            _game.MoveCommandsHistory.Add(historyItem);

            return historyItem;
        }

        public MoveCommandHistoryItem Undo()
        {
            var historyItem = _undoCommands.Pop().CopyObject<MoveCommandHistoryItem>();

            _redoCommands.Push(historyItem);

            _game.CurrentPosition.Board = BoardHelper.GetPreviousBoardPosition(_game.CurrentPosition, historyItem.Command);
            _game.CurrentPosition.LastMoveStoneCommand = historyItem.PreviousCommand;
            _game.CurrentPosition.RemovedStonesInLastMove = historyItem.PreviousRemovedOponentStones;
            _game.CurrentPosition.NumberOfMovesWithoutRemovedStoneCounter = historyItem.PreviousNumberOfMovesWithoutRemovedStoneCounter;
            _game.CurrentPosition.Status = historyItem.PreviousBoardStatus;
            _game.CurrentPlayer = historyItem.PreviousPlayerColor;
            _game.CurrentPosition.WhitePlayerStonesCount += historyItem.PreviousPlayerColor == PlayerColor.Black ? historyItem.RemovedOponentStones.Count : 0;
            _game.CurrentPosition.BlackPlayerStonesCount += historyItem.PreviousPlayerColor == PlayerColor.White ? historyItem.RemovedOponentStones.Count : 0;

            historyItem.UndoRedo = UndoRedo.Undo;
            _game.MoveCommandsHistory.Add(historyItem);

            return historyItem;
        }

        public PlayerColor GetCurrentPlayerColor()
        {
            return _game.CurrentPlayer;
        }

        public bool IsValidMove(MoveStoneCommand command)
        {
            return _rules.IsValidPlayerMove(_game.CurrentPosition, command, GetCurrentPlayerColor());
        }

        public bool IsUndoPossible()
        {
            return _undoCommands.Count > 0;
        }

        public bool IsRedoPossible()
        {
            return _redoCommands.Count > 0;
        }

        public IDictionary<Tuple<int, int>, HashSet<Tuple<int, int>>> GetCurrentPlayerPossibleMoves()
        {
            return _rules.GetCurrentPlayerPossibleMoves(_game.CurrentPosition, _game.CurrentPlayer);
        }
    }
}
