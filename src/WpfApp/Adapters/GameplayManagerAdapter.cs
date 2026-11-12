using Latrunculi.Entities;
using Latrunculi.Entities.Enums;
using Latrunculi.Extensions;
using Latrunculi.Managers.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using WpfApp.Adapters.Interfaces;
using WpfApp.Helpers;
using WpfApp.Models;

namespace WpfApp.Adapters
{
    public class GameplayManagerAdapter : IGameplayManagerAdapter
    {
        private readonly IGameplayManager _gameplayManager;
        private readonly IGameManager _gameManager;

        public GameplayManagerAdapter(IGameManager gameManager)
        {
            _gameManager = gameManager;
            _gameplayManager = gameManager.PlayGame();
        }

        public GameplayModel GetStartingGamePlayModel(GameplayModel model)
        {
            model = new GameplayModel()
            {
                Board = GetInitialBoard(_gameplayManager.GetCurrentPosition()),
                MovesHistory = MapToUiHistory(_gameplayManager.GetHistory()),
                IsUndoPossible = _gameplayManager.IsUndoPossible(),
                IsRedoPossible = _gameplayManager.IsRedoPossible(),
                ValidPositionSelections = _gameplayManager.GetCurrentPlayerPossibleMoves(),
                GameplayUserMessage = GetUserMessage(_gameplayManager.GetCurrentPosition().Status, _gameplayManager.GetCurrentPlayerColor()),
                BlackPlayer = MapToUiPlayer(_gameManager.GetBlackPlayer(), _gameplayManager.GetCurrentPosition().BlackPlayerStonesCount),
                WhitePlayer = MapToUiPlayer(_gameManager.GetWhitePlayer(), _gameplayManager.GetCurrentPosition().WhitePlayerStonesCount),
                IsBestMovePossible = !IsCurrentPlayerComputer() && IsGameOpen(),
                IsAutoplayPossible = IsCurrentPlayerComputer() && IsGameOpen(),
                IsAutoplayRunning = false,
                NumberOfMovesWithoutRemovedStone = _gameplayManager.GetCurrentPosition().NumberOfMovesWithoutRemovedStoneCounter
            };

            return model;
        }

        private ObservableCollection<UiMoveHistoryItem> MapToUiHistory(IEnumerable<MoveCommandHistoryItem> enumerable)
        {
            var result = new ObservableCollection<UiMoveHistoryItem>();
            var num = 1;

            foreach(var history in enumerable)
            {
                result.Add(MapToUiHistoryItem(num, history));

                num++;
            }

            return result;
        }

        private UiMoveHistoryItem MapToUiHistoryItem(int num, MoveCommandHistoryItem history)
        {
            return new UiMoveHistoryItem()
            {
                MoveNumber = num,
                UndoRedo = history.UndoRedo == UndoRedo.NotSpecified ? "-" : history.UndoRedo.ToString(),
                Black = history.PreviousPlayerColor == PlayerColor.Black ? history.Command.StringCommand : string.Empty,
                White = history.PreviousPlayerColor == PlayerColor.White ? history.Command.StringCommand : string.Empty,
            };
        }

        private PlayerDetails MapToUiPlayer(Player player, int v)
        {
            if(player == null)
            {
                return null;
            }

            return new PlayerDetails()
            {
                Level = player.Level.ToString(),
                Type = player.Type.ToString(),
                RemainingStones = v
            };
        }

        private ObservableCollection<ObservableCollection<Square>> GetInitialBoard(BoardPosition currentPosition)
        {
            var result = new ObservableCollection<ObservableCollection<Square>>();

            for (int i = 0; i < currentPosition.Board.GetLength(0); i++)
            {
                result.Add(new ObservableCollection<Square>());

                for (int j = 0; j < currentPosition.Board.GetLength(1); j++)
                {
                    var sq = new Square()
                    {
                        ImageSource = currentPosition.Board[i, j].GetUiSquareStatus().GetImageSource(),
                        Coords = new Tuple<int, int>(i, j),
                        SquareStatus = currentPosition.Board[i, j],
                        UiSquareStatus = currentPosition.Board[i, j].GetUiSquareStatus()
                    };

                    result[i].Add(sq);
                }
            }

            return result;
        }

        public void PerformMove(MoveStoneCommand command, GameplayModel model, CancellationToken token)
        {
            var history = _gameplayManager.PerformMove(command, token);

            UpdateModel(model, history);
        }

        public void UpdateModel(GameplayModel model, MoveCommandHistoryItem history)
        {
            var currentPosition = _gameplayManager.GetCurrentPosition();

            for(int i = 0; i < currentPosition.Board.GetLength(0); i++)
            {
                for (int j = 0; j < currentPosition.Board.GetLength(1); j++)
                {
                    if(currentPosition.Board[i, j] != model.Board[i][j].SquareStatus)
                    {
                        model.Board[i][j].SquareStatus = currentPosition.Board[i, j];
                        model.Board[i][j].UiSquareStatus = currentPosition.Board[i, j].GetUiSquareStatus();
                        model.Board[i][j].ImageSource = model.Board[i][j].UiSquareStatus.GetImageSource();
                    }
                }
            }

            model.IsUndoPossible = _gameplayManager.IsUndoPossible();
            model.IsRedoPossible = _gameplayManager.IsRedoPossible();
            model.ValidPositionSelections = _gameplayManager.GetCurrentPlayerPossibleMoves();
            model.GameplayUserMessage = GetUserMessage(_gameplayManager.GetCurrentPosition().Status, _gameplayManager.GetCurrentPlayerColor());
            model.IsAutoplayPossible = IsCurrentPlayerComputer() && IsGameOpen();
            model.IsBestMovePossible = !IsCurrentPlayerComputer() && IsGameOpen();
            model.NumberOfMovesWithoutRemovedStone = _gameplayManager.GetCurrentPosition().NumberOfMovesWithoutRemovedStoneCounter;
            model.BlackPlayer.RemainingStones = _gameplayManager.GetCurrentPosition().BlackPlayerStonesCount;
            model.WhitePlayer.RemainingStones = _gameplayManager.GetCurrentPosition().WhitePlayerStonesCount;

            model.MovesHistory.Add(MapToUiHistoryItem(model.MovesHistory.Count + 1, history));
        }

        private string GetUserMessage(BoardPositionStatus status, PlayerColor playerColor)
        {
            if(status == BoardPositionStatus.Open)
            {
                return string.Format("It's {0}'s turn", playerColor.ToString());
            }

            switch (status)
            {
                case BoardPositionStatus.Draw:
                    return "Game is over. The result is Draw.";
                case BoardPositionStatus.BlackWin:
                    return "Game is over. Black player win.";
                case BoardPositionStatus.WhiteWin:
                    return "Game is over. White player win.";
            }

            return string.Empty;
        }

        public void Redo(GameplayModel model)
        {
            var history = _gameplayManager.Redo();

            UpdateModel(model, history);
        }

        public void Replay(GameplayModel model)
        {
            throw new NotImplementedException();
        }

        public void Undo(GameplayModel model)
        {
            var history = _gameplayManager.Undo();

            UpdateModel(model, history);
        }

        public void DisplayPossibleMoves(GameplayModel model, Square myPosi)
        {
            var possibleMoves = model.ValidPositionSelections[myPosi.Coords];

            var newUiStatus = myPosi.UiSquareStatus.GetSelectionStatus();

            var imageFrom = newUiStatus.GetImageSource();
            var imageTo = UiSquareStatus.PossibleMove.GetImageSource();

            model.Board[myPosi.Coords.Item1][myPosi.Coords.Item2].ImageSource = imageFrom;
            model.Board[myPosi.Coords.Item1][myPosi.Coords.Item2].UiSquareStatus = newUiStatus;

            var suggested = new List<Tuple<int, int>>();
            foreach (var pos in possibleMoves)
            {
                model.Board[pos.Item1][pos.Item2].ImageSource = imageTo;
                suggested.Add(pos);
            }

            model.PreviouslySuggestedPositions = new Tuple<Square, IList<Tuple<int, int>>>(new Square() 
            {
                UiSquareStatus = myPosi.UiSquareStatus,
                Coords = myPosi.Coords
            }, suggested);
        }

        public void HidePreviouslySuggestedPossibleMoves(GameplayModel model)
        {
            if(model.PreviouslySuggestedPositions == null)
            {
                return;
            }

            var newUiStatus = model.PreviouslySuggestedPositions.Item1.UiSquareStatus.GetUnSelectionStatus();

            var newMyPosColor = newUiStatus.GetImageSource();
            var imageTo = UiSquareStatus.Empty.GetImageSource();

            model.Board[model.PreviouslySuggestedPositions.Item1.Coords.Item1][model.PreviouslySuggestedPositions.Item1.Coords.Item2].ImageSource = newMyPosColor;
            model.Board[model.PreviouslySuggestedPositions.Item1.Coords.Item1][model.PreviouslySuggestedPositions.Item1.Coords.Item2].UiSquareStatus = newUiStatus;

            foreach (var pos in model.PreviouslySuggestedPositions.Item2)
            {
                model.Board[pos.Item1][pos.Item2].ImageSource = imageTo;
            }

            model.PreviouslySuggestedPositions = null;
        }

        public bool IsCurrentPlayerComputer()
        {
            var currentPlayer = _gameplayManager.GetCurrentPlayer();

            return currentPlayer != null && currentPlayer.Type == PlayerType.Computer;
        }

        public MoveStoneCommand GetCurrentPlayerBestMove(CancellationToken token)
        {
            return _gameplayManager.GetCurrentPlayerBestMoveHint(token);
        }

        public void DisplayBestPossibleMove(MoveStoneCommand bestMove, GameplayModel model)
        {
            var suggestedMoveFrom = model.Board[bestMove.From.Item1][bestMove.From.Item2];

            var newColor = suggestedMoveFrom.UiSquareStatus.GetBestMoveSelectionStatus();
            var imageFrom = newColor.GetImageSource();
            var imageTo = UiSquareStatus.BestPossibleMove.GetImageSource();

            model.Board[bestMove.From.Item1][bestMove.From.Item2].ImageSource = imageFrom;
            model.Board[bestMove.From.Item1][bestMove.From.Item2].UiSquareStatus = newColor;

            model.Board[bestMove.To.Item1][bestMove.To.Item2].ImageSource = imageTo;

            model.PreviousSuggestedBestMove = new Tuple<Square, Tuple<int, int>>(new Square()
            {
                Coords = model.Board[bestMove.From.Item1][bestMove.From.Item2].Coords,
                UiSquareStatus = model.Board[bestMove.From.Item1][bestMove.From.Item2].UiSquareStatus
            }, bestMove.To);
        }

        public void HidePreviouslyBestPossibleMove(GameplayModel model)
        {
            if(model.PreviousSuggestedBestMove == null)
            {
                return;
            }

            var newColor = model.PreviousSuggestedBestMove.Item1.UiSquareStatus.GetBestMoveUnSelectionStatus();
            var imageFrom = newColor.GetImageSource();
            var imageTo = UiSquareStatus.Empty.GetImageSource();

            //because it mighthave been already 'hidden' in HidePreviouslySuggestedPossibleMoves
            if (model.Board[model.PreviousSuggestedBestMove.Item1.Coords.Item1][model.PreviousSuggestedBestMove.Item1.Coords.Item2].UiSquareStatus == model.PreviousSuggestedBestMove.Item1.UiSquareStatus)
            {
                model.Board[model.PreviousSuggestedBestMove.Item1.Coords.Item1][model.PreviousSuggestedBestMove.Item1.Coords.Item2].ImageSource = imageFrom;
                model.Board[model.PreviousSuggestedBestMove.Item1.Coords.Item1][model.PreviousSuggestedBestMove.Item1.Coords.Item2].UiSquareStatus = newColor;

                model.Board[model.PreviousSuggestedBestMove.Item2.Item1][model.PreviousSuggestedBestMove.Item2.Item2].ImageSource = imageTo;
            }

            model.PreviousSuggestedBestMove = null;
        }

        public MoveCommandHistoryItem Autoplay(CancellationToken token)
        {
            return _gameplayManager.PerformMove(null, token);
        }

        public bool IsGameOpen()
        {
            return _gameplayManager.GetCurrentPosition().Status == BoardPositionStatus.Open;
        }
    }
}
